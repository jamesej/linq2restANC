// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectExpressionFactoryTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SelectExpressionFactoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Parser
{
	using System.Linq;
	using Linq2Rest.Parser;
	using NUnit.Framework;

	[TestFixture]
	public class SelectExpressionFactoryTests
	{
		[Test]
		public void WhenCreatingSelectExpressionFromDataMemberOnFieldThenGetsFieldValue()
		{
			var nameResolver = new MemberNameResolver();
			var factory = new SelectExpressionFactory<FakeItem>(nameResolver, new RuntimeTypeProvider(nameResolver));
			var items = new[] { new FakeItem { StringValue = "test" } };

			var expression = factory.Create("Text");

			dynamic result = items.AsQueryable().Select(expression).First();

			Assert.AreEqual("test", result.Text);
		}

		[Test]
		public void WhenCreatingSelectExpressionFromXmlMemberOnPropertyThenGetsPropertyValue()
		{
			var nameResolver = new MemberNameResolver();
			var factory = new SelectExpressionFactory<FakeItem>(nameResolver, new RuntimeTypeProvider(nameResolver));
			var items = new[] { new FakeItem { IntValue = 2 } };

			var expression = factory.Create("Number");

			dynamic result = items.AsQueryable().Select(expression).First();

			Assert.AreEqual(2, result.Number);
		}
	}
}
