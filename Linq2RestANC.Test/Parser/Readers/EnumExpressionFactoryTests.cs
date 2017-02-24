// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumExpressionFactoryTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EnumExpressionFactoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Parser.Readers
{
	using System;
	using Linq2Rest.Parser.Readers;
	using NUnit.Framework;

	[TestFixture]
	public class EnumExpressionFactoryTests
	{
		private const string EnumString = "Linq2Rest.Tests.Choice'That'";
		private EnumExpressionFactory _factory;

		[SetUp]
		public void Setup()
		{
			_factory = new EnumExpressionFactory();
		}

		[Test]
		public void WhenFilterIncludesCorrectEnumValueThenReturnedExpressionContainsEnumValue()
		{
			var expression = _factory.Convert(EnumString);

			Assert.IsAssignableFrom<Choice>(expression.Value);
		}

		[Test]
		public void WhenFilterIsIncorrectFormatThenThrows()
		{
			const string Parameter = "blah";

			Assert.Throws<FormatException>(() => _factory.Convert(Parameter));
		}
	}
}