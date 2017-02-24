// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestGetQueryProviderTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestGetQueryProviderTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Provider
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using Linq2Rest.Provider;
	using Linq2Rest.Provider.Writers;
	using Linq2Rest.Tests.Fakes;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class RestGetQueryProviderTests
	{
		private RestGetQueryProvider<FakeItem> _provider;
		private Mock<IRestClient> _mockClient;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			_mockClient = new Mock<IRestClient>();
			_mockClient.SetupGet(x => x.ServiceBase).Returns(new Uri("http://localhost"));
			var memberNameResolver = new MemberNameResolver();
			_provider = new RestGetQueryProvider<FakeItem>(_mockClient.Object, new TestSerializerFactory(memberNameResolver), new ExpressionProcessor(new ExpressionWriter(memberNameResolver, Enumerable.Empty<IValueWriter>()), memberNameResolver), memberNameResolver, Enumerable.Empty<IValueWriter>(), typeof(FakeItem));
		}

		[Test]
		public void WhenCreatingGenericQueryThenReturnsQueryableWithPassedExpression()
		{
			Expression expression = Expression.New(typeof(FakeItem));

			var queryable = _provider.CreateQuery<FakeItem>(expression);

			Assert.AreSame(expression, queryable.Expression);
		}

		[Test]
		public void WhenCreatingGenericQueryWithNullExpressionThenThrows()
		{
			Assert.Throws<ArgumentNullException>(() => _provider.CreateQuery<FakeItem>(null));
		}

		[Test]
		public void WhenCreatingNonGenericQueryThenReturnsQueryableWithPassedExpression()
		{
			Expression expression = Expression.New(typeof(FakeItem));

			var queryable = _provider.CreateQuery(expression);

			Assert.AreSame(expression, queryable.Expression);
		}

		[Test]
		public void WhenCreatingNonGenericQueryWithNullExpressionThenThrows()
		{
			Assert.Throws<ArgumentNullException>(() => _provider.CreateQuery(null));
		}

		[Test]
		public void WhenDisposingThenDisposesClient()
		{
			_provider.Dispose();

			_mockClient.Verify(x => x.Dispose());
		}
	}
}