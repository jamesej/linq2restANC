// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LongExpressionFactoryTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the LongExpressionFactoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Parser.Readers
{
	using System;
	using Linq2Rest.Parser.Readers;
	using NUnit.Framework;

	[TestFixture]
	public class LongExpressionFactoryTests
	{
		private LongExpressionFactory _factory;

		[SetUp]
		public void Setup()
		{
			_factory = new LongExpressionFactory();
		}

		[Test]
		public void WhenFilterIncludesLongParameterThenReturnedExpressionContainsLong()
		{
			var expression = _factory.Convert("123");

			Assert.IsAssignableFrom<long>(expression.Value);
		}

		[Test]
		public void WhenFilterIsIncorrectFormatThenReturnsThrows()
		{
			const string Parameter = "blah";

			Assert.Throws<FormatException>(() => _factory.Convert(Parameter));
		}
	}
}