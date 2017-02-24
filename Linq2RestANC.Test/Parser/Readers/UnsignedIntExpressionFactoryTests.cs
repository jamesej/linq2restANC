// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnsignedIntExpressionFactoryTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnsignedIntExpressionFactoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Parser.Readers
{
	using System;
	using Linq2Rest.Parser.Readers;
	using NUnit.Framework;

	[TestFixture]
	public class UnsignedIntExpressionFactoryTests
	{
		private UnsignedIntExpressionFactory _factory;

		[SetUp]
		public void Setup()
		{
			_factory = new UnsignedIntExpressionFactory();
		}

		[Test]
		public void WhenFilterIncludesUnsignedIntParameterThenReturnedExpressionContainsUnsignedInt()
		{
			var expression = _factory.Convert("123");

			Assert.IsAssignableFrom<uint>(expression.Value);
		}

		[Test]
		public void WhenFilterIsIncorrectFormatThenThrows()
		{
			const string Parameter = "blah";

			Assert.Throws<FormatException>(() => _factory.Convert(Parameter));
		}
	}
}