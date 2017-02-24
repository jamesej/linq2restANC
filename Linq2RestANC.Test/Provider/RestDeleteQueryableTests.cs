// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestDeleteQueryableTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestDeleteQueryableTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Provider
{
	using System;
	using System.Collections;
	using System.Linq;
	using System.Linq.Expressions;
	using Linq2Rest.Provider;
	using Linq2Rest.Provider.Writers;
	using Linq2Rest.Tests.Fakes;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class RestDeleteQueryableTests
	{
		private RestDeleteQueryable<FakeItem> _deleteQueryable;
		private Mock<IRestClient> _mockClient;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			var mockResolver = new MemberNameResolver();
			Expression<Func<FakeItem, bool>> expression = x => true;
			_mockClient = new Mock<IRestClient>();
			_mockClient.SetupGet(x => x.ServiceBase).Returns(new Uri("http://localhost"));
			_mockClient.Setup(x => x.Delete(It.IsAny<Uri>())).Returns("[]".ToStream());
			_deleteQueryable = new RestDeleteQueryable<FakeItem>(_mockClient.Object, new TestSerializerFactory(mockResolver), mockResolver, Enumerable.Empty<IValueWriter>() , expression, typeof(FakeItem));
		}

		[Test]
		public void ElementTypeIsSameAsGenericParameter()
		{
			Assert.AreEqual(typeof(FakeItem), _deleteQueryable.ElementType);
		}

		[Test]
		public void WhenDeletetingNonGenericEnumeratorThenDoesNotReturnNull()
		{
			Assert.NotNull((_deleteQueryable as IEnumerable).GetEnumerator());
		}

		[Test]
		public void WhenDisposingThenDisposesClient()
		{
			_deleteQueryable.Dispose();

			_mockClient.Verify(x => x.Dispose());
		}
	}
}