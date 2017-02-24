// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestContextTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestContextTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Provider
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Linq.Expressions;
	using Linq2Rest.Provider;
	using Linq2Rest.Tests.Fakes;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class RestContextTests
	{
		[TestFixture]
		public class SimpleDtoRestContextTests
		{
			private RestContext<SimpleDto> _provider;
			private RestContext<ComplexDto> _complexProvider;
			private RestContext<CollectionDto> _collectionProvider;
			private Mock<IRestClient> _mockClient;
			private Mock<IRestClient> _mockComplexClient;
			private Mock<IRestClient> _mockCollectionClient;
			private string _singleResponse;

			[SetUp]
			public void TestSetup()
			{
				var nameResolver = new MemberNameResolver();

				_singleResponse = "[{\"Value\" : 2, \"Content\" : \"blah\" }]";
				var baseUri = new Uri("http://localhost");
				var serializerFactory = new TestSerializerFactory(nameResolver);

				_mockClient = new Mock<IRestClient>();
				_mockClient.SetupGet(x => x.ServiceBase)
					.Returns(baseUri);
				_mockClient.Setup(x => x.Get(It.IsAny<Uri>()))
					.Callback<Uri>(u => Console.WriteLine(u.ToString()))
					.Returns(() => _singleResponse.ToStream());
				_mockClient.Setup(x => x.Post(It.IsAny<Uri>(), It.IsAny<Stream>()))
					.Callback<Uri, Stream>((u, s) => Console.WriteLine(u.ToString()))
					.Returns(() => _singleResponse.ToStream());
				_mockClient.Setup(x => x.Put(It.IsAny<Uri>(), It.IsAny<Stream>()))
					.Callback<Uri, Stream>((u, s) => Console.WriteLine(u.ToString()))
					.Returns(() => _singleResponse.ToStream());
				_mockClient.Setup(x => x.Delete(It.IsAny<Uri>()))
					.Callback<Uri>(u => Console.WriteLine(u.ToString()))
					.Returns(() => _singleResponse.ToStream());

				_provider = new RestContext<SimpleDto>(_mockClient.Object, serializerFactory);

				_mockComplexClient = new Mock<IRestClient>();
				_mockComplexClient.SetupGet(x => x.ServiceBase)
					.Returns(baseUri);
				_mockComplexClient.Setup(x => x.Get(It.IsAny<Uri>()))
					.Callback<Uri>(u => Console.WriteLine(u.ToString()))
					.Returns("[{\"Value\" : 2, \"Content\" : \"blah\", \"Child\" : {\"ID\" : 2, \"Name\" : \"Foo\"}}]".ToStream());
				_complexProvider = new RestContext<ComplexDto>(_mockComplexClient.Object, serializerFactory);

				_mockCollectionClient = new Mock<IRestClient>();
				_mockCollectionClient.SetupGet(x => x.ServiceBase)
					.Returns(baseUri);
				_mockCollectionClient.Setup(x => x.Get(It.IsAny<Uri>()))
					.Callback<Uri>(u => Console.WriteLine(u.ToString()))
					.Returns("[{\"Value\" : 2, \"Content\" : \"blah\", \"Children\" : [{\"ID\" : 1, \"Name\" : \"Foo\"}, {\"ID\" : 2, \"Name\" : \"Bar\"}]}]".ToStream());

				_collectionProvider = new RestContext<CollectionDto>(_mockCollectionClient.Object, serializerFactory);
			}

			[Test]
			public void WhenAnyExpressionRequiresEagerEvaluationThenCallsRestServiceWithExistingFilterParameter()
			{
				var result = _provider
					.Query
					.Where(x => x.Value <= 3)
					.Any(x => x.Value.Equals(3d));

				var uri = new Uri("http://localhost/?$filter=Value+le+3");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingAllOperationsThenCallsRestServiceWithAllParametersSet()
			{
				var result = _provider.Query
					.Where(x => x.Value <= 3)
					.Select(x => new { x.Value, x.Content })
					.OrderBy(x => x.Value)
					.Skip(1)
					.Take(1)
					.Count();

				var uri = new Uri("http://localhost/?$filter=Value+le+3&$select=Value,Content&$skip=1&$top=1&$orderby=Value");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingCeilingExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => Math.Ceiling(x.Value) == 10d, "http://localhost/?$filter=ceiling(Value)+eq+10");
			}

			[Test]
			public void WhenApplyingCompareGreaterExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.Content.CompareTo("text") > 0, "http://localhost/?$filter=Content+gt+'text'");
			}

			[Test]
			public void WhenApplyingCompareLesserExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.Content.CompareTo("text") <= 0, "http://localhost/?$filter=Content+le+'text'");
			}

			[Test]
			public void WhenApplyingContainsQueryThenCallsRestServiceWithFilter()
			{
				VerifyCall(x => x.Content.Contains("blah"), "http://localhost/?$filter=substringof('blah'%2c+Content)");
			}

			[Test]
			public void WhenApplyingDayExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.Date.Day == 10, "http://localhost/?$filter=day(Date)+eq+10");
			}

			[Test]
			public void WhenApplyingDeleteQueryThenCallsRestServiceOnce()
			{
				var result = _provider.Query
					.Where(x => x.Value <= 3)
					.Delete()
					.Count(x => x.ID != 0);

				_mockClient.Verify(x => x.Delete(It.IsAny<Uri>()), Times.Once());
			}

			[Test]
			public void WhenApplyingEmptyAnyOperatorThenSendEmptyPredicate()
			{
				var result = _collectionProvider.Query
					.Where(x => x.Children.Any())
					.ToList();

				_mockCollectionClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == "http://localhost/?$filter=Children%2fany(Param_0:+true)")), Times.Once());
			}

			[Test]
			public void WhenApplyingEndsWithExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.Content.EndsWith("text"), "http://localhost/?$filter=endswith(Content%2c+'text')");
			}

			[Test]
			public void WhenApplyingEqualityExpressionForCatpuredVariablePropertyThenCallsRestServiceWithFilterParameter()
			{
				const string Variable = "blah";
				VerifyCall(x => x.Value == Variable.Length, "http://localhost/?$filter=Value+eq+4");
			}

			[Test]
			public void WhenApplyingEqualityExpressionForCatpuredVariableThenCallsRestServiceWithFilterParameter()
			{
				const double Variable = 2.0;
				VerifyCall(x => x.Value == Variable, "http://localhost/?$filter=Value+eq+2");
			}

			[Test]
			public void WhenApplyingEqualityExpressionForFlagsEnumThenCallsRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.Choice == Choice.That, "http://localhost/?$filter=Choice+eq+Linq2Rest.Tests.Choice'That'");
			}

			[Test]
			public void WhenApplyingEqualsQueryOnDateTimeOffsetThenCallsRestServiceWithFilter()
			{
				var result = _provider
					.Query
					.Count(x => x.PointInTime == new DateTimeOffset(2012, 5, 6, 18, 10, 0, TimeSpan.FromHours(2)));

				const string Uri = "http://localhost/?$filter=PointInTime+eq+datetimeoffset'2012-05-06T18:10:00%2b02:00'";
				_mockClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == Uri)), Times.Once());
			}

			[Test]
			public void WhenApplyingEqualsQueryOnDateTimeThenCallsRestServiceWithFilter()
			{
				var result = _provider.Query
					.Count(x => x.Date == new DateTime(2012, 5, 6, 16, 11, 00, DateTimeKind.Utc));

				var uri = new Uri("http://localhost/?$filter=Date+eq+datetime'2012-05-06T16:11:00Z'");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingEqualsQueryOnTimeSpanThenCallsRestServiceWithFilter()
			{
				var result = _provider.Query
					.Count(x => x.Duration == new TimeSpan(2, 15, 0));

				var uri = new Uri("http://localhost/?$filter=Duration+eq+time'PT2H15M'");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingEqualsQueryThenCallsRestServiceWithFilter()
			{
				var result = _provider.Query
					.Count(x => x.Value.Equals(3));

				var uri = new Uri("http://localhost/?$filter=Value+eq+3");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingExpandThenCallsRestServiceWithExpandParameterSet()
			{
				var result = _provider.Query
					.Expand("Foo,Bar/Qux")
					.ToList();

				var uri = new Uri("http://localhost/?$expand=Foo,Bar/Qux");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingExpandWithOrderByThenCallsRestServiceWithExpandParameterSet()
			{
				var result = _provider.Query
					.Expand("Foo,Bar/Qux")
					.OrderBy(x => x.Date)
					.ToList();

				var uri = new Uri("http://localhost/?$orderby=Date&$expand=Foo,Bar/Qux");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingExpandsOnSubPropertyUsingExpressionQueryThenCallsRestServiceWithFilter()
			{
				var nameResolver = new MemberNameResolver();
				var result = _provider.Query
					.Expand(nameResolver, x => x.Date.Year)
					.ToArray();

				var uri = new Uri("http://localhost/?$expand=Date/Year");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingExpandsQueryThenCallsRestServiceWithFilter()
			{
				var result = _provider.Query
					.Expand("Value")
					.ToArray();

				var uri = new Uri("http://localhost/?$expand=Value");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingExpandsUsingExpressionQueryThenCallsRestServiceWithFilter()
			{
				var nameResolver = new MemberNameResolver();
				var result = _provider.Query
					.Expand(nameResolver, x => x.Value)
					.ToArray();

				var uri = new Uri("http://localhost/?$expand=Value");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingExpandsUsingMultipleExpressionQueryThenCallsRestServiceWithFilter()
			{
				var nameResolver = new MemberNameResolver();
				var result = _provider.Query
					.Expand(nameResolver, x => x.Value, x => x.Content)
					.ToArray();

				var uri = new Uri("http://localhost/?$expand=Value,Content");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingFilterWithAllOnRootCollectionAndFunctionThenCallsRestServiceWithAllSyntax()
			{
				var result = _collectionProvider.Query
					.Where(x => x.Children.All(y => y.ID == 2 + x.ID))
					.ToList();

				_mockCollectionClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == "http://localhost/?$filter=Children%2fall(y:+y%2fID+eq+2+add+ID)")), Times.Once());
			}

			[Test]
			public void WhenApplyingFilterWithAllOnRootCollectionThenCallsRestServiceWithAllSyntax()
			{
				var result = _collectionProvider.Query
					.Where(x => x.Children.All(y => y.ID == 2))
					.ToList();

				_mockCollectionClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == "http://localhost/?$filter=Children%2fall(y:+y%2fID+eq+2)")), Times.Once());
			}

			[Test]
			public void WhenApplyingFilterWithAnyOnRootCollectionThenCallsRestServiceWithAnySyntax()
			{
				var result = _collectionProvider.Query
					.Where(x => x.Children.Any(y => y.ID == 2))
					.ToList();

				_mockCollectionClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == "http://localhost/?$filter=Children%2fany(y:+y%2fID+eq+2)")), Times.Once());
			}

			[Test]
			public void WhenApplyingFirstOrDefaultThenCallsRestServiceOnce()
			{
				_singleResponse = "[]";
				Assert.Null(_provider.Query.FirstOrDefault(x => x.Value <= 3));
			}

			[Test]
			public void WhenApplyingFloorExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => Math.Floor(x.Value) == 10d, "http://localhost/?$filter=floor(Value)+eq+10");
			}

			[Test]
			public void WhenApplyingHourExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.Date.Hour == 10, "http://localhost/?$filter=hour(Date)+eq+10");
			}

			[Test]
			public void WhenApplyingIndexOfExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.Content.IndexOf("text") > -1, "http://localhost/?$filter=indexof(Content%2c+'text')+gt+-1");
			}

			[Test]
			public void WhenApplyingLengthExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.Content.Length > 32, "http://localhost/?$filter=length(Content)+gt+32");
			}

			[Test]
			public void WhenApplyingMinuteExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.Date.Minute == 10, "http://localhost/?$filter=minute(Date)+eq+10");
			}

			[Test]
			public void WhenApplyingMonthExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.Date.Month == 10, "http://localhost/?$filter=month(Date)+eq+10");
			}

			[Test]
			public void WhenApplyingMultipleProjectionsThenReturnsFinalProjection()
			{
				var result = _provider.Query
					.Select(x => new { x.Content })
					.Select(x => new ChildDto { Name = x.Content })
					.ToArray();

				Assert.True(
					typeof(ChildDto) == result.First()
											.GetType());
			}

			[Test]
			public void WhenApplyingMultipleProjectionsThenUsesFirst()
			{
				var result = _provider.Query
					.Select(x => new { x.Content })
					.Select(x => new ChildDto { Name = x.Content })
					.ToArray();

				var uri = new Uri("http://localhost/?$select=Content");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingNegateQueryThenCallsRestServiceWithFilter()
			{
				var result = _provider.Query
					.Count(x => -x.Value < 3);

				var uri = new Uri("http://localhost/?$filter=-Value+lt+3");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingNotExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => !(x.Value <= 3), "http://localhost/?$filter=not(Value+le+3)");
			}

			[Test]
			public void WhenApplyingPostQueryInMiddleThenCallsRestServiceOnceWithFullQuery()
			{
				var result = _provider.Query
					.Where(x => x.Value <= 3)
					.Post(new SimpleDto { ID = 1 })
					.Count(x => x.ID != 0);

				var uri = new Uri("http://localhost/?$filter=(Value+le+3)+and+(ID+ne+0)");
				_mockClient.Verify(x => x.Post(uri, It.IsAny<Stream>()), Times.Once());
			}

			[Test]
			public void WhenApplyingPostQueryThenCallsRestServiceOnce()
			{
				var result = _provider.Query
					.Where(x => x.Value <= 3)
					.Post(new SimpleDto { ID = 1 })
					.Count(x => x.ID != 0);

				_mockClient.Verify(x => x.Post(It.IsAny<Uri>(), It.IsAny<Stream>()), Times.Once());
			}

			[Test]
			public void WhenApplyingPutQueryInMiddleThenCallsRestServiceOnceWithFullQuery()
			{
				var result = _provider.Query
					.Where(x => x.Value <= 3)
					.Put(new SimpleDto { ID = 1 })
					.Count(x => x.ID != 0);

				var uri = new Uri("http://localhost/?$filter=(Value+le+3)+and+(ID+ne+0)");
				_mockClient.Verify(x => x.Put(uri, It.IsAny<Stream>()), Times.Once());
			}

			[Test]
			public void WhenApplyingPutQueryThenCallsRestServiceOnce()
			{
				var result = _provider.Query
					.Where(x => x.Value <= 3)
					.Put(new SimpleDto { ID = 1 })
					.Count(x => x.ID != 0);

				_mockClient.Verify(x => x.Put(It.IsAny<Uri>(), It.IsAny<Stream>()), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryThenCallsRestServiceOnce()
			{
				var result = _provider.Query
					.Where(x => x.Value <= 3)
					.Count(x => x.ID != 0);

				_mockClient.Verify(x => x.Get(It.IsAny<Uri>()), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithCountFilterThenCallsRestServiceWithFilterParameter()
			{
				var result = _provider
					.Query
					.Count(x => x.Value <= 3);

				var uri = new Uri("http://localhost/?$filter=Value+le+3");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithFirstFilterThenCallsRestServiceWithFilterParameter()
			{
				var result = _provider
					.Query
					.First(x => x.Value <= 3);

				var uri = new Uri("http://localhost/?$filter=Value+le+3&$top=1");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithFirstOrDefaultFilterThenCallsRestServiceWithFilterParameter()
			{
				var result = _provider
					.Query
					.FirstOrDefault(x => x.Value <= 3);

				var uri = new Uri("http://localhost/?$filter=Value+le+3&$top=1");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithLastFilterThenCallsRestServiceWithFilterParameter()
			{
				var result = _provider
					.Query
					.Last(x => x.Value <= 3);

				var uri = new Uri("http://localhost/?$filter=Value+le+3");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithLastOrDefaultFilterThenCallsRestServiceWithFilterParameter()
			{
				var result = _provider
					.Query
					.LastOrDefault(x => x.Value <= 3);

				var uri = new Uri("http://localhost/?$filter=Value+le+3");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithMultipleFiltersThenCallsRestServiceWithSingleFilterParameter()
			{
				var result = _provider
					.Query
					.Where(x => x.Value <= 3)
					.Where(x => x.Content == "blah")
					.ToArray();

				var uri = new Uri("http://localhost/?$filter=(Value+le+3)+and+(Content+eq+'blah')");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithMultipleSelectionsThenCallsRestServiceWithSelectParameter()
			{
				var result = _provider.Query
					.Select(x => new { x.Value, x.Content })
					.Count();

				var uri = new Uri("http://localhost/?$select=Value,Content");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithNoFilterThenCallsRestServiceOnce()
			{
				var result = _provider.Query.ToList();

				var uri = new Uri("http://localhost/");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithOrderingThenCallsRestServiceWithOrderParameter()
			{
				var result = _provider.Query
					.OrderBy(x => x.Value)
					.Count();

				var uri = new Uri("http://localhost/?$orderby=Value");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithSelectionThenCallsRestServiceWithSelectParameter()
			{
				var result = _provider.Query
					.Select(x => new { x.Value })
					.Count();

				var uri = new Uri("http://localhost/?$select=Value");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithSingleFilterThenCallsRestServiceWithFilterParameter()
			{
				var result = _provider
					.Query
					.First(x => x.Value <= 3);

				var uri = new Uri("http://localhost/?$filter=Value+le+3&$top=1");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithSingleOrDefaultFilterOnNavigationPropertyThenCallsRestServiceWithFilterParameter()
			{
				var result = _complexProvider
					.Query
					.FirstOrDefault(x => x.Child.Name == "Foo");

				var uri = new Uri("http://localhost/?$filter=Child%2fName+eq+'Foo'&$top=1");

				_mockComplexClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithSingleOrDefaultFilterThenCallsRestServiceWithFilterParameter()
			{
				var result = _provider
					.Query
					.FirstOrDefault(x => x.Value <= 3);

				var uri = new Uri("http://localhost/?$filter=Value+le+3&$top=1");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithSkipThenCallsRestServiceWithSkipParameter()
			{
				var result = _provider.Query
					.Skip(1)
					.Count();

				var uri = new Uri("http://localhost/?$skip=1");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithTakeThenCallsRestServiceWithTakeParameter()
			{
				var result = _provider.Query
					.Take(1)
					.Count();

				var uri = new Uri("http://localhost/?$top=1");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingRoundExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => Math.Round(x.Value) == 10d, "http://localhost/?$filter=round(Value)+eq+10");
			}

			[Test]
			public void WhenApplyingSecondExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.Date.Second == 10, "http://localhost/?$filter=second(Date)+eq+10");
			}

			[Test]
			public void WhenApplyingStartsWithExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.Content.StartsWith("text"), "http://localhost/?$filter=startswith(Content%2c+'text')");
			}

			[Test]
			public void WhenApplyingToLowerExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.Content.ToLower() == "text", "http://localhost/?$filter=tolower(Content)+eq+'text'");
			}

			[Test]
			public void WhenApplyingToLowerInvariantExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.Content.ToLowerInvariant() == "text", "http://localhost/?$filter=tolower(Content)+eq+'text'");
			}

			[Test]
			public void WhenApplyingToUpperExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.Content.ToUpper() == "text", "http://localhost/?$filter=toupper(Content)+eq+'text'");
			}

			[Test]
			public void WhenApplyingToUpperInvariantExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.Content.ToUpperInvariant() == "text", "http://localhost/?$filter=toupper(Content)+eq+'text'");
			}

			[Test]
			public void WhenApplyingTrimExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.Content.Trim() == "text", "http://localhost/?$filter=trim(Content)+eq+'text'");
			}

			[Test]
			public void WhenApplyingYearExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.Date.Year == 10, "http://localhost/?$filter=year(Date)+eq+10");
			}

			[Test]
			public void WhenBaseUriHasQueryParametersThenTheyArePreservedInTheRequest()
			{
				var mockResolver = new Mock<IMemberNameResolver>();
				var client = new Mock<IRestClient>();
				client.SetupGet(x => x.ServiceBase)
					.Returns(new Uri("http://localhost?abc=123"));
				client.Setup(x => x.Get(It.IsAny<Uri>()))
					.Callback<Uri>(u => Console.WriteLine(u.ToString()))
					.Returns(() => _singleResponse.ToStream());
				var provider = new RestContext<SimpleDto>(client.Object, new TestSerializerFactory(mockResolver.Object));

				var result = provider
					.Query
					.Where(x => x.Value == 5)
					.ToArray();

				var uri = new Uri("http://localhost/?abc=123&$filter=Value+eq+5");
				client.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenCapturingVariableHeldInArrayThenResolvesReference()
			{
				var names = new[] { "foo", "bar" };
				var result = _collectionProvider.Query
					.Where(c => c.Content == names[0] || c.Content == names[1])
					.ToList();

				_mockCollectionClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == "http://localhost/?$filter=Content+eq+'foo'+or+Content+eq+'bar'")), Times.Once());
			}

			[Test]
			public void WhenDisposingThenDoesNotThrow()
			{
				Assert.DoesNotThrow(() => _provider.Dispose());
			}

			[Test]
			public void WhenGroupByExpressionRequiresEagerEvaluationThenCallsRestServiceWithExistingFilterParameter()
			{
				var result = _provider
					.Query
					.Where(x => x.Value <= 3)
					.GroupBy(x => x.Content)
					.ToArray();

				var uri = new Uri("http://localhost/?$filter=Value+le+3");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenMainExpressionIsContainedInIsTrueExpressionThenUsesOperandExpression()
			{
				var parameter = Expression.Parameter(typeof(SimpleDto), "x");
				var trueExpression =
					Expression.IsTrue(
						Expression.LessThanOrEqual(Expression.Property(parameter, "Value"), Expression.Constant(3d)));

				var result =
					_provider
						.Query
						.Where(Expression.Lambda<Func<SimpleDto, bool>>(trueExpression, parameter))
						.Count(x => x.ID != 0);

				var uri = new Uri("http://localhost/?$filter=(Value+le+3)+and+(ID+ne+0)");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenMaxExpressionRequiresEagerEvaluationThenCallsRestServiceWithExistingFilterParameter()
			{
				var result = _provider
					.Query
					.Where(x => x.Value <= 3)
					.Max(x => x.Value);

				var uri = new Uri("http://localhost/?$filter=Value+le+3");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenMinExpressionRequiresEagerEvaluationThenCallsRestServiceWithExistingFilterParameter()
			{
				var result = _provider
					.Query
					.Where(x => x.Value <= 3)
					.Min(x => x.Value);

				var uri = new Uri("http://localhost/?$filter=Value+le+3");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenSelectQueryProjectsIntoMemberWithDifferentNameThenThrows()
			{
				Assert.Throws<InvalidOperationException>(
					() => _provider.Query.Select(x => new { Something = x.Value })
							  .Count());
			}

			[Test]
			public void WhenUsingArrayLengthExpressionThenResolvesValue()
			{
				var names = new[] { "foo", "bar" };
				var result = _collectionProvider.Query
					.Where(c => c.Value == names.Length)
					.ToList();

				_mockCollectionClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == "http://localhost/?$filter=Value+eq+2")), Times.Once());
			}

			[Test]
			public void WhenUsingCoalesceExpressionThenResolvesValue()
			{
				var names = new[] { null, "foo", "bar" };
				var result = _collectionProvider.Query
					.Where(c => c.Content == (names[0] ?? names[1]) || c.Content == names[2])
					.ToList();

				_mockCollectionClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == "http://localhost/?$filter=Content+eq+'foo'+or+Content+eq+'bar'")), Times.Once());
			}

			[Test]
			public void WhenUsingConditionalExpressionThenResolvesValue()
			{
				var names = new[] { "foo", "bar" };
				var result = _collectionProvider.Query
					.Where(c => c.Content == (names[0].Length < 10 ? names[0] : names[1]) || c.Content == names[1])
					.ToList();

				_mockCollectionClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == "http://localhost/?$filter=Content+eq+'foo'+or+Content+eq+'bar'")), Times.Once());
			}

			[Test]
			public void WhenValueExpressionContainsCastingThenResolvesValue()
			{
				object value = "hello";
				Expression<Func<SimpleDto, bool>> expression = x => x.Value == ((string)value).Length;
				var result =
					_provider
						.Query
						.Where(expression)
						.ToArray();

				var uri = new Uri("http://localhost/?$filter=Value+eq+5");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenValueExpressionContainsSafeCastingThenResolvesValue()
			{
				object value = "hello";
				Expression<Func<SimpleDto, bool>> expression = x => x.Value == (value as string).Length;
				var result =
					_provider
						.Query
						.Where(expression)
						.ToArray();

				var uri = new Uri("http://localhost/?$filter=Value+eq+5");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			private void VerifyCall(Expression<Func<SimpleDto, bool>> selection, string expectedUri)
			{
				var result = _provider.Query
					.Where(selection)
					.Count();

				_mockClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == expectedUri)), Times.Once());
			}
		}
		[TestFixture]
		public class AliasDtoRestContextTests
		{
			private RestContext<AliasDto> _provider;
			private RestContext<ComplexDto> _complexProvider;
			private RestContext<CollectionDto> _collectionProvider;
			private Mock<IRestClient> _mockClient;
			private Mock<IRestClient> _mockComplexClient;
			private Mock<IRestClient> _mockCollectionClient;
			private string _singleResponse;

			[SetUp]
			public void TestSetup()
			{
				var resolver = new MemberNameResolver();
				_singleResponse = "[{\"Value\" : 2, \"Content\" : \"blah\" }]";
				var baseUri = new Uri("http://localhost");
				var serializerFactory = new TestSerializerFactory(resolver);

				_mockClient = new Mock<IRestClient>();
				_mockClient.SetupGet(x => x.ServiceBase)
					.Returns(baseUri);
				_mockClient.Setup(x => x.Get(It.IsAny<Uri>()))
					.Callback<Uri>(u => Console.WriteLine(u.ToString()))
					.Returns(() => _singleResponse.ToStream());
				_mockClient.Setup(x => x.Post(It.IsAny<Uri>(), It.IsAny<Stream>()))
					.Callback<Uri, Stream>((u, s) => Console.WriteLine(u.ToString()))
					.Returns(() => _singleResponse.ToStream());
				_mockClient.Setup(x => x.Put(It.IsAny<Uri>(), It.IsAny<Stream>()))
					.Callback<Uri, Stream>((u, s) => Console.WriteLine(u.ToString()))
					.Returns(() => _singleResponse.ToStream());
				_mockClient.Setup(x => x.Delete(It.IsAny<Uri>()))
					.Callback<Uri>(u => Console.WriteLine(u.ToString()))
					.Returns(() => _singleResponse.ToStream());

				_provider = new RestContext<AliasDto>(_mockClient.Object, serializerFactory);
				
				_mockComplexClient = new Mock<IRestClient>();
				_mockComplexClient.SetupGet(x => x.ServiceBase)
					.Returns(baseUri);
				_mockComplexClient.Setup(x => x.Get(It.IsAny<Uri>()))
					.Callback<Uri>(u => Console.WriteLine(u.ToString()))
					.Returns("[{\"Value\" : 2, \"Content\" : \"blah\", \"Child\" : {\"ID\" : 2, \"Name\" : \"Foo\"}}]".ToStream());
				
				_mockCollectionClient = new Mock<IRestClient>();
				_mockCollectionClient.SetupGet(x => x.ServiceBase)
					.Returns(baseUri);
				_mockCollectionClient.Setup(x => x.Get(It.IsAny<Uri>()))
					.Callback<Uri>(u => Console.WriteLine(u.ToString()))
					.Returns("[{\"Value\" : 2, \"Content\" : \"blah\", \"Children\" : [{\"ID\" : 1, \"Name\" : \"Foo\"}, {\"ID\" : 2, \"Name\" : \"Bar\"}]}]".ToStream());

				_complexProvider = new RestContext<ComplexDto>(_mockComplexClient.Object, serializerFactory);

				_collectionProvider = new RestContext<CollectionDto>(_mockCollectionClient.Object, serializerFactory);
			}

			[Test]
			public void WhenAnyExpressionRequiresEagerEvaluationThenCallsRestServiceWithExistingFilterParameter()
			{
				var result = _provider
					.Query
					.Where(x => x.AliasValue <= 3)
					.Any(x => x.AliasValue.Equals(3d));

				var uri = new Uri("http://localhost/?$filter=Value+le+3");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingAllOperationsThenCallsRestServiceWithAllParametersSet()
			{
				var result = _provider.Query
					.Where(x => x.AliasValue <= 3)
					.Select(x => new { x.AliasValue, x.AliasContent })
					.OrderBy(x => x.AliasValue)
					.Skip(1)
					.Take(1)
					.Count();

				var uri = new Uri("http://localhost/?$filter=Value+le+3&$select=Value,Content&$skip=1&$top=1&$orderby=Value");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingCeilingExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => Math.Ceiling(x.AliasValue) == 10d, "http://localhost/?$filter=ceiling(Value)+eq+10");
			}

			[Test]
			public void WhenApplyingCompareGreaterExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.AliasContent.CompareTo("text") > 0, "http://localhost/?$filter=Content+gt+'text'");
			}

			[Test]
			public void WhenApplyingCompareLesserExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.AliasContent.CompareTo("text") <= 0, "http://localhost/?$filter=Content+le+'text'");
			}

			[Test]
			public void WhenApplyingContainsQueryThenCallsRestServiceWithFilter()
			{
				VerifyCall(x => x.AliasContent.Contains("blah"), "http://localhost/?$filter=substringof('blah'%2c+Content)");
			}

			[Test]
			public void WhenApplyingDayExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.AliasDate.Day == 10, "http://localhost/?$filter=day(Date)+eq+10");
			}

			[Test]
			public void WhenApplyingDeleteQueryThenCallsRestServiceOnce()
			{
				var result = _provider.Query
					.Where(x => x.AliasValue <= 3)
					.Delete()
					.Count(x => x.AliasID != 0);

				_mockClient.Verify(x => x.Delete(It.IsAny<Uri>()), Times.Once());
			}

			[Test]
			public void WhenApplyingEmptyAnyOperatorThenSendEmptyPredicate()
			{
				var result = _collectionProvider.Query
					.Where(x => x.Children.Any())
					.ToList();

				_mockCollectionClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == "http://localhost/?$filter=Children%2fany(Param_0:+true)")), Times.Once());
			}

			[Test]
			public void WhenApplyingEndsWithExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.AliasContent.EndsWith("text"), "http://localhost/?$filter=endswith(Content%2c+'text')");
			}

			[Test]
			public void WhenApplyingEqualityExpressionForCatpuredVariablePropertyThenCallsRestServiceWithFilterParameter()
			{
				const string Variable = "blah";
				VerifyCall(x => x.AliasValue == Variable.Length, "http://localhost/?$filter=Value+eq+4");
			}

			[Test]
			public void WhenApplyingEqualityExpressionForCatpuredVariableThenCallsRestServiceWithFilterParameter()
			{
				const double Variable = 2.0;
				VerifyCall(x => x.AliasValue == Variable, "http://localhost/?$filter=Value+eq+2");
			}

			[Test]
			public void WhenApplyingEqualityExpressionForFlagsEnumThenCallsRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.AliasChoice == Choice.That, "http://localhost/?$filter=Choice+eq+Linq2Rest.Tests.Choice'That'");
			}

			[Test]
			public void WhenApplyingEqualsQueryOnDateTimeOffsetThenCallsRestServiceWithFilter()
			{
				var result = _provider
					.Query
					.Count(x => x.AliasPointInTime == new DateTimeOffset(2012, 5, 6, 18, 10, 0, TimeSpan.FromHours(2)));

				const string Uri = "http://localhost/?$filter=PointInTime+eq+datetimeoffset'2012-05-06T18:10:00%2b02:00'";
				_mockClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == Uri)), Times.Once());
			}

			[Test]
			public void WhenApplyingEqualsQueryOnDateTimeThenCallsRestServiceWithFilter()
			{
				var result = _provider.Query
					.Count(x => x.AliasDate == new DateTime(2012, 5, 6, 16, 11, 00, DateTimeKind.Utc));

				var uri = new Uri("http://localhost/?$filter=Date+eq+datetime'2012-05-06T16:11:00Z'");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingEqualsQueryOnTimeSpanThenCallsRestServiceWithFilter()
			{
				var result = _provider.Query
					.Count(x => x.AliasDuration == new TimeSpan(2, 15, 0));

				var uri = new Uri("http://localhost/?$filter=Duration+eq+time'PT2H15M'");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingEqualsQueryThenCallsRestServiceWithFilter()
			{
				var result = _provider.Query
					.Count(x => x.AliasValue.Equals(3));

				var uri = new Uri("http://localhost/?$filter=Value+eq+3");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingExpandThenCallsRestServiceWithExpandParameterSet()
			{
				var result = _provider.Query
					.Expand("Foo,Bar/Qux")
					.ToList();

				var uri = new Uri("http://localhost/?$expand=Foo,Bar/Qux");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingExpandWithOrderByThenCallsRestServiceWithExpandParameterSet()
			{
				var result = _provider.Query
					.Expand("Foo,Bar/Qux")
					.OrderBy(x => x.AliasDate)
					.ToList();

				var uri = new Uri("http://localhost/?$orderby=Date&$expand=Foo,Bar/Qux");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingExpandsOnSubPropertyUsingExpressionQueryThenCallsRestServiceWithFilter()
			{
				var nameResolver = new MemberNameResolver();
				var result = _provider.Query
					.Expand(nameResolver, x => x.AliasDate.Year)
					.ToArray();

				var uri = new Uri("http://localhost/?$expand=Date/Year");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingExpandsQueryThenCallsRestServiceWithFilter()
			{
				var result = _provider.Query
					.Expand("Value")
					.ToArray();

				var uri = new Uri("http://localhost/?$expand=Value");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingExpandsUsingExpressionQueryThenCallsRestServiceWithFilter()
			{
				var nameResolver = new MemberNameResolver();
				var result = _provider.Query
					.Expand(nameResolver, x => x.AliasValue)
					.ToArray();

				var uri = new Uri("http://localhost/?$expand=Value");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingExpandsUsingMultipleExpressionQueryThenCallsRestServiceWithFilter()
			{
				var nameResolver = new MemberNameResolver();
				var result = _provider.Query
					.Expand(nameResolver, x => x.AliasValue, x => x.AliasContent)
					.ToArray();

				var uri = new Uri("http://localhost/?$expand=Value,Content");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingFilterWithAllOnRootCollectionAndFunctionThenCallsRestServiceWithAllSyntax()
			{
				var result = _collectionProvider.Query
					.Where(x => x.Children.All(y => y.ID == 2 + x.ID))
					.ToList();

				_mockCollectionClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == "http://localhost/?$filter=Children%2fall(y:+y%2fID+eq+2+add+ID)")), Times.Once());
			}

			[Test]
			public void WhenApplyingFilterWithAllOnRootCollectionThenCallsRestServiceWithAllSyntax()
			{
				var result = _collectionProvider.Query
					.Where(x => x.Children.All(y => y.ID == 2))
					.ToList();

				_mockCollectionClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == "http://localhost/?$filter=Children%2fall(y:+y%2fID+eq+2)")), Times.Once());
			}

			[Test]
			public void WhenApplyingFilterWithAnyOnRootCollectionThenCallsRestServiceWithAnySyntax()
			{
				var result = _collectionProvider.Query
					.Where(x => x.Children.Any(y => y.ID == 2))
					.ToList();

				_mockCollectionClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == "http://localhost/?$filter=Children%2fany(y:+y%2fID+eq+2)")), Times.Once());
			}

			[Test]
			public void WhenApplyingFirstOrDefaultThenCallsRestServiceOnce()
			{
				_singleResponse = "[]";
				Assert.Null(_provider.Query.FirstOrDefault(x => x.AliasValue <= 3));
			}

			[Test]
			public void WhenApplyingFloorExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => Math.Floor(x.AliasValue) == 10d, "http://localhost/?$filter=floor(Value)+eq+10");
			}

			[Test]
			public void WhenApplyingHourExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.AliasDate.Hour == 10, "http://localhost/?$filter=hour(Date)+eq+10");
			}

			[Test]
			public void WhenApplyingIndexOfExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.AliasContent.IndexOf("text") > -1, "http://localhost/?$filter=indexof(Content%2c+'text')+gt+-1");
			}

			[Test]
			public void WhenApplyingLengthExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.AliasContent.Length > 32, "http://localhost/?$filter=length(Content)+gt+32");
			}

			[Test]
			public void WhenApplyingMinuteExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.AliasDate.Minute == 10, "http://localhost/?$filter=minute(Date)+eq+10");
			}

			[Test]
			public void WhenApplyingMonthExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.AliasDate.Month == 10, "http://localhost/?$filter=month(Date)+eq+10");
			}

			[Test]
			public void WhenApplyingMultipleProjectionsThenReturnsFinalProjection()
			{
				var result = _provider.Query
					.Select(x => new { x.AliasContent })
					.Select(x => new ChildDto { Name = x.AliasContent })
					.ToArray();

				Assert.True(
					typeof(ChildDto) == result.First()
											.GetType());
			}

			[Test]
			public void WhenApplyingMultipleProjectionsThenUsesFirst()
			{
				var result = _provider.Query
					.Select(x => new { x.AliasContent })
					.Select(x => new ChildDto { Name = x.AliasContent })
					.ToArray();

				var uri = new Uri("http://localhost/?$select=Content");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingNegateQueryThenCallsRestServiceWithFilter()
			{
				var result = _provider.Query
					.Count(x => -x.AliasValue < 3);

				var uri = new Uri("http://localhost/?$filter=-Value+lt+3");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingNotExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => !(x.AliasValue <= 3), "http://localhost/?$filter=not(Value+le+3)");
			}

			[Test]
			public void WhenApplyingPostQueryInMiddleThenCallsRestServiceOnceWithFullQuery()
			{
				var result = _provider.Query
					.Where(x => x.AliasValue <= 3)
					.Post(new AliasDto { AliasID = 1 })
					.Count(x => x.AliasID != 0);

				var uri = new Uri("http://localhost/?$filter=(Value+le+3)+and+(ID+ne+0)");
				_mockClient.Verify(x => x.Post(uri, It.IsAny<Stream>()), Times.Once());
			}

			[Test]
			public void WhenApplyingPostQueryThenCallsRestServiceOnce()
			{
				var result = _provider.Query
					.Where(x => x.AliasValue <= 3)
					.Post(new AliasDto { AliasID = 1 })
					.Count(x => x.AliasID != 0);

				_mockClient.Verify(x => x.Post(It.IsAny<Uri>(), It.IsAny<Stream>()), Times.Once());
			}

			[Test]
			public void WhenApplyingPutQueryInMiddleThenCallsRestServiceOnceWithFullQuery()
			{
				var result = _provider.Query
					.Where(x => x.AliasValue <= 3)
					.Put(new AliasDto { AliasID = 1 })
					.Count(x => x.AliasID != 0);

				var uri = new Uri("http://localhost/?$filter=(Value+le+3)+and+(ID+ne+0)");
				_mockClient.Verify(x => x.Put(uri, It.IsAny<Stream>()), Times.Once());
			}

			[Test]
			public void WhenApplyingPutQueryThenCallsRestServiceOnce()
			{
				var result = _provider.Query
					.Where(x => x.AliasValue <= 3)
					.Put(new AliasDto { AliasID = 1 })
					.Count(x => x.AliasID != 0);

				_mockClient.Verify(x => x.Put(It.IsAny<Uri>(), It.IsAny<Stream>()), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryThenCallsRestServiceOnce()
			{
				var result = _provider.Query
					.Where(x => x.AliasValue <= 3)
					.Count(x => x.AliasID != 0);

				_mockClient.Verify(x => x.Get(It.IsAny<Uri>()), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithCountFilterThenCallsRestServiceWithFilterParameter()
			{
				var result = _provider
					.Query
					.Count(x => x.AliasValue <= 3);

				var uri = new Uri("http://localhost/?$filter=Value+le+3");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithFirstFilterThenCallsRestServiceWithFilterParameter()
			{
				var result = _provider
					.Query
					.First(x => x.AliasValue <= 3);

				var uri = new Uri("http://localhost/?$filter=Value+le+3&$top=1");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithFirstOrDefaultFilterThenCallsRestServiceWithFilterParameter()
			{
				var result = _provider
					.Query
					.FirstOrDefault(x => x.AliasValue <= 3);

				var uri = new Uri("http://localhost/?$filter=Value+le+3&$top=1");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithLastFilterThenCallsRestServiceWithFilterParameter()
			{
				var result = _provider
					.Query
					.Last(x => x.AliasValue <= 3);

				var uri = new Uri("http://localhost/?$filter=Value+le+3");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithLastOrDefaultFilterThenCallsRestServiceWithFilterParameter()
			{
				var result = _provider
					.Query
					.LastOrDefault(x => x.AliasValue <= 3);

				var uri = new Uri("http://localhost/?$filter=Value+le+3");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithMultipleFiltersThenCallsRestServiceWithSingleFilterParameter()
			{
				var result = _provider
					.Query
					.Where(x => x.AliasValue <= 3)
					.Where(x => x.AliasContent == "blah")
					.ToArray();

				var uri = new Uri("http://localhost/?$filter=(Value+le+3)+and+(Content+eq+'blah')");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithMultipleSelectionsThenCallsRestServiceWithSelectParameter()
			{
				var result = _provider.Query
					.Select(x => new { x.AliasValue, x.AliasContent })
					.Count();

				var uri = new Uri("http://localhost/?$select=Value,Content");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithNoFilterThenCallsRestServiceOnce()
			{
				var result = _provider.Query.ToList();

				var uri = new Uri("http://localhost/");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithOrderingThenCallsRestServiceWithOrderParameter()
			{
				var result = _provider.Query
					.OrderBy(x => x.AliasValue)
					.Count();

				var uri = new Uri("http://localhost/?$orderby=Value");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithSelectionThenCallsRestServiceWithSelectParameter()
			{
				var result = _provider.Query
					.Select(x => new { x.AliasValue })
					.Count();

				var uri = new Uri("http://localhost/?$select=Value");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithSingleFilterThenCallsRestServiceWithFilterParameter()
			{
				var result = _provider
					.Query
					.First(x => x.AliasValue <= 3);

				var uri = new Uri("http://localhost/?$filter=Value+le+3&$top=1");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithSingleOrDefaultFilterOnNavigationPropertyThenCallsRestServiceWithFilterParameter()
			{
				var result = _complexProvider
					.Query
					.FirstOrDefault(x => x.Child.Name == "Foo");

				var uri = new Uri("http://localhost/?$filter=Child%2fName+eq+'Foo'&$top=1");

				_mockComplexClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithSingleOrDefaultFilterThenCallsRestServiceWithFilterParameter()
			{
				var result = _provider
					.Query
					.FirstOrDefault(x => x.AliasValue <= 3);

				var uri = new Uri("http://localhost/?$filter=Value+le+3&$top=1");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithSkipThenCallsRestServiceWithSkipParameter()
			{
				var result = _provider.Query
					.Skip(1)
					.Count();

				var uri = new Uri("http://localhost/?$skip=1");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingQueryWithTakeThenCallsRestServiceWithTakeParameter()
			{
				var result = _provider.Query
					.Take(1)
					.Count();

				var uri = new Uri("http://localhost/?$top=1");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenApplyingRoundExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => Math.Round(x.AliasValue) == 10d, "http://localhost/?$filter=round(Value)+eq+10");
			}

			[Test]
			public void WhenApplyingSecondExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.AliasDate.Second == 10, "http://localhost/?$filter=second(Date)+eq+10");
			}

			[Test]
			public void WhenApplyingStartsWithExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.AliasContent.StartsWith("text"), "http://localhost/?$filter=startswith(Content%2c+'text')");
			}

			[Test]
			public void WhenApplyingToLowerExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.AliasContent.ToLower() == "text", "http://localhost/?$filter=tolower(Content)+eq+'text'");
			}

			[Test]
			public void WhenApplyingToLowerInvariantExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.AliasContent.ToLowerInvariant() == "text", "http://localhost/?$filter=tolower(Content)+eq+'text'");
			}

			[Test]
			public void WhenApplyingToUpperExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.AliasContent.ToUpper() == "text", "http://localhost/?$filter=toupper(Content)+eq+'text'");
			}

			[Test]
			public void WhenApplyingToUpperInvariantExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.AliasContent.ToUpperInvariant() == "text", "http://localhost/?$filter=toupper(Content)+eq+'text'");
			}

			[Test]
			public void WhenApplyingTrimExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.AliasContent.Trim() == "text", "http://localhost/?$filter=trim(Content)+eq+'text'");
			}

			[Test]
			public void WhenApplyingYearExpressionThenCallRestServiceWithFilterParameter()
			{
				VerifyCall(x => x.AliasDate.Year == 10, "http://localhost/?$filter=year(Date)+eq+10");
			}

			[Test]
			public void WhenBaseUriHasQueryParametersThenTheyArePreservedInTheRequest()
			{
				var mockResolver = new Mock<IMemberNameResolver>();

				var client = new Mock<IRestClient>();
				client.SetupGet(x => x.ServiceBase)
					.Returns(new Uri("http://localhost?abc=123"));
				client.Setup(x => x.Get(It.IsAny<Uri>()))
					.Callback<Uri>(u => Console.WriteLine(u.ToString()))
					.Returns(() => _singleResponse.ToStream());
				var provider = new RestContext<AliasDto>(client.Object, new TestSerializerFactory(mockResolver.Object));

				var result = provider
					.Query
					.Where(x => x.AliasValue == 5)
					.ToArray();

				var uri = new Uri("http://localhost/?abc=123&$filter=Value+eq+5");
				client.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenCapturingVariableHeldInArrayThenResolvesReference()
			{
				var names = new[] { "foo", "bar" };
				var result = _collectionProvider.Query
					.Where(c => c.Content == names[0] || c.Content == names[1])
					.ToList();

				_mockCollectionClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == "http://localhost/?$filter=Content+eq+'foo'+or+Content+eq+'bar'")), Times.Once());
			}

			[Test]
			public void WhenDisposingThenDoesNotThrow()
			{
				Assert.DoesNotThrow(() => _provider.Dispose());
			}

			[Test]
			public void WhenGroupByExpressionRequiresEagerEvaluationThenCallsRestServiceWithExistingFilterParameter()
			{
				var result = _provider
					.Query
					.Where(x => x.AliasValue <= 3)
					.GroupBy(x => x.AliasContent)
					.ToArray();

				var uri = new Uri("http://localhost/?$filter=Value+le+3");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenMainExpressionIsContainedInIsTrueExpressionThenUsesOperandExpression()
			{
				var parameter = Expression.Parameter(typeof(AliasDto), "x");
				var trueExpression =
					Expression.IsTrue(
						Expression.LessThanOrEqual(Expression.Property(parameter, "AliasValue"), Expression.Constant(3d)));

				var result =
					_provider
						.Query
						.Where(Expression.Lambda<Func<AliasDto, bool>>(trueExpression, parameter))
						.Count(x => x.AliasID != 0);

				var uri = new Uri("http://localhost/?$filter=(Value+le+3)+and+(ID+ne+0)");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenMaxExpressionRequiresEagerEvaluationThenCallsRestServiceWithExistingFilterParameter()
			{
				var result = _provider
					.Query
					.Where(x => x.AliasValue <= 3)
					.Max(x => x.AliasValue);

				var uri = new Uri("http://localhost/?$filter=Value+le+3");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenMinExpressionRequiresEagerEvaluationThenCallsRestServiceWithExistingFilterParameter()
			{
				var result = _provider
					.Query
					.Where(x => x.AliasValue <= 3)
					.Min(x => x.AliasValue);

				var uri = new Uri("http://localhost/?$filter=Value+le+3");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenSelectQueryProjectsIntoMemberWithDifferentNameThenThrows()
			{
				Assert.Throws<InvalidOperationException>(
					() => _provider.Query.Select(x => new { Something = x.AliasValue })
							  .Count());
			}

			[Test]
			public void WhenUsingArrayLengthExpressionThenResolvesValue()
			{
				var names = new[] { "foo", "bar" };
				var result = _collectionProvider.Query
					.Where(c => c.Value == names.Length)
					.ToList();

				_mockCollectionClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == "http://localhost/?$filter=Value+eq+2")), Times.Once());
			}

			[Test]
			public void WhenUsingCoalesceExpressionThenResolvesValue()
			{
				var names = new[] { null, "foo", "bar" };
				var result = _collectionProvider.Query
					.Where(c => c.Content == (names[0] ?? names[1]) || c.Content == names[2])
					.ToList();

				_mockCollectionClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == "http://localhost/?$filter=Content+eq+'foo'+or+Content+eq+'bar'")), Times.Once());
			}

			[Test]
			public void WhenUsingConditionalExpressionThenResolvesValue()
			{
				var names = new[] { "foo", "bar" };
				var result = _collectionProvider.Query
					.Where(c => c.Content == (names[0].Length < 10 ? names[0] : names[1]) || c.Content == names[1])
					.ToList();

				_mockCollectionClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == "http://localhost/?$filter=Content+eq+'foo'+or+Content+eq+'bar'")), Times.Once());
			}

			[Test]
			public void WhenValueExpressionContainsCastingThenResolvesValue()
			{
				object value = "hello";
				Expression<Func<AliasDto, bool>> expression = x => x.AliasValue == ((string)value).Length;
				var result =
					_provider
						.Query
						.Where(expression)
						.ToArray();

				var uri = new Uri("http://localhost/?$filter=Value+eq+5");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			[Test]
			public void WhenValueExpressionContainsSafeCastingThenResolvesValue()
			{
				object value = "hello";
				Expression<Func<AliasDto, bool>> expression = x => x.AliasValue == (value as string).Length;
				var result =
					_provider
						.Query
						.Where(expression)
						.ToArray();

				var uri = new Uri("http://localhost/?$filter=Value+eq+5");
				_mockClient.Verify(x => x.Get(uri), Times.Once());
			}

			private void VerifyCall(Expression<Func<AliasDto, bool>> selection, string expectedUri)
			{
				var result = _provider.Query
					.Where(selection)
					.Count();

				_mockClient.Verify(x => x.Get(It.Is<Uri>(u => u.ToString() == expectedUri)), Times.Once());
			}
		}
	}
}