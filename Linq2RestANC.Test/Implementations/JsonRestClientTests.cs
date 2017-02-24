// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonRestClientTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the JsonRestClientTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

//namespace Linq2Rest.Tests.Implementations
//{
//	using System;
//	using System.IO;
//	using System.Net;
//	using Linq2Rest.Implementations;
//	using Moq;
//	using NUnit.Framework;

//	public class JsonRestClientTests
//	{
//		private const string JsonHeader = "application/json";
//		private JsonRestClient _jsonClient;
//		//private Mock<HttpWebRequest> _mockRequest;

//		[SetUp]
//		public void Setup()
//		{
//			var mockResponse = new Mock<WebResponse>();
//			mockResponse.Setup(x => x.GetResponseStream()).Returns(new MemoryStream());

//			//_mockRequest = new Mock<HttpWebRequest>();
//			//_mockRequest.Setup(x => x.GetRequestStream()).Returns(new MemoryStream());
//			//_mockRequest.Setup(x => x.GetResponse()).Returns(() => mockResponse.Object);

//			var mockWebRequestCreate = new Mock<IWebRequestCreate>();
//			mockWebRequestCreate.Setup(x => x.Create(It.IsAny<Uri>())).Returns(() => _mockRequest.Object);
//			HttpWebRequest.RegisterPrefix("http://localhost", mockWebRequestCreate.Object);
//			_mockRequest.Object.Headers = new WebHeaderCollection();

//			_jsonClient = new JsonRestClient(new Uri("http://localhost"));
//		}

//		[Test]
//		[Ignore("Cannot set accept header in mock.")]
//		public void WhenPerformingGetRequestThenSetsAcceptHeader()
//		{
//			_jsonClient.Get(new Uri("http://localhost?$orderby=Value"));

//			_mockRequest.Object.Headers[HttpRequestHeader.Accept] = JsonHeader;
//		}

//		[Test]
//		[Ignore("Cannot set accept header in mock.")]
//		public void WhenPerformingPostRequestThenSetsAcceptHeader()
//		{
//			_jsonClient.Post(new Uri("http://localhost?$orderby=Value"), "[]".ToStream());

//			_mockRequest.Object.Headers[HttpRequestHeader.Accept] = JsonHeader;
//		}

//		[Test]
//		[Ignore("Cannot set accept header in mock.")]
//		public void WhenPerformingPutRequestThenSetsAcceptHeader()
//		{
//			_jsonClient.Put(new Uri("http://localhost?$orderby=Value"), "[]".ToStream());

//			_mockRequest.Object.Headers[HttpRequestHeader.Accept] = JsonHeader;
//		}

//		[Test]
//		[Ignore("Cannot set accept header in mock.")]
//		public void WhenPerformingDeleteRequestThenSetsAcceptHeader()
//		{
//			_jsonClient.Delete(new Uri("http://localhost?$orderby=Value"));

//			_mockRequest.Object.Headers[HttpRequestHeader.Accept] = JsonHeader;
//		}

//		[Test]
//		[Ignore("Cannot set accept header in mock.")]
//		public void WhenDisposingThenDoesNotThrow()
//		{
//			Assert.DoesNotThrow(() => _jsonClient.Dispose());
//		}

//		[Test]
//		[Ignore("Cannot set accept header in mock.")]
//		public void WhenPerformingPostRequestThenSetsPostMethod()
//		{
//			_jsonClient.Post(new Uri("http://localhost?$orderby=Value"), "[]".ToStream());

//			_mockRequest.VerifySet(x => x.Method = "POST");
//		}

//		[Test]
//		[Ignore("Cannot set accept header in mock.")]
//		public void WhenPerformingPutRequestThenSetsPutMethod()
//		{
//			_jsonClient.Put(new Uri("http://localhost?$orderby=Value"), "[]".ToStream());

//			_mockRequest.VerifySet(x => x.Method = "PUT");
//		}

//		[Test]
//		[Ignore("Cannot set accept header in mock.")]
//		public void WhenPerformingDeleteRequestThenSetsDeleteMethod()
//		{
//			_jsonClient.Delete(new Uri("http://localhost?$orderby=Value"));

//			_mockRequest.VerifySet(x => x.Method = "DELETE");
//		}
//	}
//}
