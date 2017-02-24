// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlDataContractSerializerFactoryTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the XmlDataContractSerializerFactoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Implementations
{
	using System;
	using System.Linq;
	using Linq2Rest.Implementations;
	using NUnit.Framework;

	[TestFixture]
	public class XmlDataContractSerializerFactoryTests
	{
		private XmlDataContractSerializerFactory _factory;

		[SetUp]
		public void Setup()
		{
			_factory = new XmlDataContractSerializerFactory(Type.EmptyTypes);
		}

		[Test]
		public void CreatedSerializerCanDeserializeDataContractType()
		{
			const string Xml = "<SimpleContractItem xmlns=\"http://schemas.datacontract.org/2004/07/Linq2Rest.Tests.Implementations\"><Text>test</Text><Value>2</Value></SimpleContractItem>";

			var serializer = _factory.Create<SimpleContractItem>();

			var deserializedResult = serializer.Deserialize(Xml.ToStream());

			Assert.AreEqual(2, deserializedResult.Value);
			Assert.AreEqual("test", deserializedResult.SomeString);
		}

		[Test]
		public void CreatedSerializerCanDeserializeListOfDataContractType()
		{
			const string Xml = "<ArrayOfSimpleContractItem xmlns=\"http://schemas.datacontract.org/2004/07/Linq2Rest.Tests.Implementations\"><SimpleContractItem><Text>test</Text><Value>2</Value></SimpleContractItem></ArrayOfSimpleContractItem>";

			var serializer = _factory.Create<SimpleContractItem>();

			var deserializedResult = serializer.DeserializeList(Xml.ToStream());

			Assert.AreEqual(1, deserializedResult.Count());
		}

		[Test]
		public void CreatedSerializerCanSerializeDataContractType()
		{
			var serializer = _factory.Create<SimpleContractItem>();

			var deserializedResult = serializer.Serialize(new SimpleContractItem());

			Assert.NotNull(deserializedResult);
		}

		[Test]
		public void WhenCreatingSerializerThenDoesNotReturnNull()
		{
			Assert.NotNull(_factory.Create<SimpleContractItem>());
		}
	}
}