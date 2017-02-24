// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestContextInMemoryTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestContextInMemoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Provider
{
	using System.Linq;
	using Linq2Rest.Implementations;
	using Linq2Rest.Provider;
	using Linq2Rest.Tests.Fakes;
	using NUnit.Framework;

	[TestFixture]
	public class RestContextInMemoryTests
	{
		private InMemoryJsonRestClient<FakeItem> _mockClient;
		private RestContext<FakeItem> _restContext;

		[SetUp]
		public void TestSetup()
		{
			var knownTypes = new[] { typeof(FakeItem), typeof(FakeChildItem), typeof(FakeGrandChildItem) };
			ISerializerFactory serializerFactory = new JsonDataContractSerializerFactory(knownTypes);
			var data = new[]
					   {
						   new FakeItem
						   {
							   StringValue = "2", 
							   Children =
							   {
								   new FakeChildItem { ChildStringValue = "1" }, 
								   new FakeChildItem { ChildStringValue = "2" }, 
								   new FakeChildItem { ChildStringValue = "3" }
							   }
						   }, 
						   new FakeItem
						   {
							   StringValue = "1", 
							   Children =
							   {
								   new FakeChildItem { ChildStringValue = "2" }, 
								   new FakeChildItem { ChildStringValue = "3" }, 
								   new FakeChildItem { ChildStringValue = "4" }
							   }
						   }, 
						   new FakeItem
						   {
							   StringValue = "3", 
							   Children =
							   {
								   new FakeChildItem { ChildStringValue = "3" }, 
								   new FakeChildItem { ChildStringValue = "4" }, 
								   new FakeChildItem { ChildStringValue = "5" }
							   }
						   }, 
						   new FakeItem
						   {
							   StringValue = "4", 
							   Children =
							   {
								   new FakeChildItem { ChildStringValue = "6" }, 
								   new FakeChildItem { ChildStringValue = "6" }, 
								   new FakeChildItem { ChildStringValue = "6" }
							   }
						   }, 
						   new FakeItem
						   {
							   StringValue = "74", 
							   Children =
							   {
								   new FakeChildItem { ChildStringValue = "7" }, 
								   new FakeChildItem { ChildStringValue = "7" }, 
								   new FakeChildItem { ChildStringValue = "7" }
							   }
						   }, 
					   };
			_mockClient = new InMemoryJsonRestClient<FakeItem>(data, knownTypes);
			_restContext = new RestContext<FakeItem>(_mockClient, serializerFactory);
		}

		[Test]
		public void WhenFilteringWithAllUsingEqualityOnFunctionThenOnlyMatchingItemsReturned()
		{
			var results = _restContext.Query.Where(x => x.Children.All(y => x.StringValue.StartsWith(y.ChildStringValue))).ToList();
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("74", results.First().StringValue);
		}

		[Test]
		public void WhenFilteringWithAllUsingEqualityThenOnlyMatchingItemsReturned()
		{
			var results = _restContext.Query.Where(x => x.Children.All(y => y.ChildStringValue == "6")).ToList();
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("4", results.First().StringValue);
		}

		[Test]
		public void WhenFilteringWithAnyUsingEqualityThenOnlyMatchingItemsReturned()
		{
			var results = _restContext.Query.Where(x => x.Children.Any(y => y.ChildStringValue == "5")).ToList();
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("3", results.First().StringValue);
		}

		[Test]
		public void WhenFilteringWithIntegerEqualityThenOnlyMatchingItemsReturned()
		{
			var results = _restContext.Query.Where(x => x.StringValue == "2").ToList();
			Assert.AreEqual(1, results.Count);
			Assert.AreEqual("2", results.First().StringValue);
		}
	}
}