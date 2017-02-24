// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestDeleteQueryProviderTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestDeleteQueryProviderTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Provider
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using Linq2Rest.Parser;
	using Linq2Rest.Provider;
	using Linq2Rest.Provider.Writers;
	using Linq2Rest.Tests.Fakes;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class RestDeleteQueryProviderTests
	{
		private RestDeleteQueryProvider<FakeItem> _provider;
		private Mock<IRestClient> _mockClient;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			_mockClient = new Mock<IRestClient>();
			_mockClient.SetupGet(x => x.ServiceBase).Returns(new Uri("http://localhost"));
			_mockClient.Setup(x => x.Delete(It.IsAny<Uri>())).Returns("[]".ToStream());
			var memberNameResolver = new MemberNameResolver();
			_provider = new RestDeleteQueryProvider<FakeItem>(_mockClient.Object, new TestSerializerFactory(memberNameResolver), new ExpressionProcessor(new ExpressionWriter(memberNameResolver, Enumerable.Empty<IValueWriter>()), memberNameResolver), memberNameResolver, Enumerable.Empty<IValueWriter>(), typeof(FakeItem));
		}

		[Test]
		public void WhenExecutingQueryThenDeletesToRestClient()
		{
			Expression<Func<FakeItem, bool>> expression = x => true;
			_provider.Execute(expression);

			_mockClient.Verify(x => x.Delete(It.IsAny<Uri>()));
		}
	}
}