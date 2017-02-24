// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonDataContractSerializerFactoryTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the JsonDataContractSerializerFactoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Implementations
{
	using System;
	using System.Linq;
	using Linq2Rest.Implementations;
	using NUnit.Framework;

	[TestFixture]
	public class JsonDataContractSerializerFactoryTests
	{
		private JsonDataContractSerializerFactory _factory;

		[SetUp]
		public void Setup()
		{
			_factory = new JsonDataContractSerializerFactory(Type.EmptyTypes);
		}

		[Test]
		public void CreatedSerializerCanDeserializeDataContractType()
		{
			const string Json = "{\"Value\": 2, \"Text\":\"test\"}";

			var serializer = _factory.Create<SimpleContractItem>();

			var deserializedResult = serializer.Deserialize(Json.ToStream());

			Assert.AreEqual(2, deserializedResult.Value);
			Assert.AreEqual("test", deserializedResult.SomeString);
		}

		[Test]
		public void CreatedSerializerCanDeserializeListOfDataContractType()
		{
			const string Json = "[{\"Value\": 2, \"Text\":\"test\"}]";

			var serializer = _factory.Create<SimpleContractItem>();

			var deserializedResult = serializer.DeserializeList(Json.ToStream());

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
