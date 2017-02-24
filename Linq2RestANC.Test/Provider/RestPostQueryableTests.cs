// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestPostQueryableTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestPostQueryableTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Provider
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Linq.Expressions;
	using Linq2Rest.Provider;
	using Linq2Rest.Tests.Fakes;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class RestPostQueryableTests
	{
		private RestPostQueryable<FakeItem> _postQueryable;
		private Mock<IRestClient> _mockClient;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			var mockResolver = new Mock<IMemberNameResolver>();
			Expression<Func<FakeItem, bool>> expression = x => true;
			_mockClient = new Mock<IRestClient>();
			_mockClient.SetupGet(x => x.ServiceBase).Returns(new Uri("http://localhost"));
			_mockClient.Setup(x => x.Post(It.IsAny<Uri>(), It.IsAny<Stream>())).Returns("[]".ToStream());
			_postQueryable = new RestPostQueryable<FakeItem>(_mockClient.Object, new TestSerializerFactory(mockResolver.Object), expression, "[]".ToStream(), typeof(FakeItem));
		}

		[Test]
		public void ElementTypeIsSameAsGenericParameter()
		{
			Assert.AreEqual(typeof(FakeItem), _postQueryable.ElementType);
		}

		[Test]
		public void WhenDisposingThenDisposesClient()
		{
			_postQueryable.Dispose();

			_mockClient.Verify(x => x.Dispose());
		}

		[Test]
		public void WhenPosttingNonGenericEnumeratorThenDoesNotReturnNull()
		{
			Assert.NotNull((_postQueryable as IEnumerable).GetEnumerator());
		}
	}
}