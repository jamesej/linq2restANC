// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestGetQueryableTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestGetQueryableTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Provider
{
	using System;
	using System.Collections;
	using System.Linq;
	using Linq2Rest.Provider;
	using Linq2Rest.Provider.Writers;
	using Linq2Rest.Tests.Fakes;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class RestGetQueryableTests
	{
		private RestGetQueryable<FakeItem> _getQueryable;
		private Mock<IRestClient> _mockClient;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			var mockResolver = new MemberNameResolver();
			_mockClient = new Mock<IRestClient>();
			_mockClient.SetupGet(x => x.ServiceBase).Returns(new Uri("http://localhost"));
			_mockClient.Setup(x => x.Get(It.IsAny<Uri>())).Returns("[]".ToStream());
			_getQueryable = new RestGetQueryable<FakeItem>(_mockClient.Object, new TestSerializerFactory(mockResolver), mockResolver, Enumerable.Empty<IValueWriter>(), typeof(FakeItem));
		}

		[Test]
		public void ElementTypeIsSameAsGenericParameter()
		{
			Assert.AreEqual(typeof(FakeItem), _getQueryable.ElementType);
		}

		[Test]
		public void WhenDisposingThenDisposesClient()
		{
			_getQueryable.Dispose();

			_mockClient.Verify(x => x.Dispose());
		}

		[Test]
		public void WhenGettingNonGenericEnumeratorThenDoesNotReturnNull()
		{
			Assert.NotNull((_getQueryable as IEnumerable).GetEnumerator());
		}
	}
}
