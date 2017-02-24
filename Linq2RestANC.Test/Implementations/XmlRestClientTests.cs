// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlRestClientTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the XmlRestClientTests type.
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

//	public class XmlRestClientTests
//	{
//		private const string XmlHeader = "application/xml";
//		private Mock<HttpWebRequest> _mockRequest;
//		private XmlRestClient _xmlClient;

//		[SetUp]
//		public void Setup()
//		{
//			var headerCollection = new WebHeaderCollection();

//			var mockResponse = new Mock<WebResponse>();
//			mockResponse.Setup(x => x.GetResponseStream()).Returns(new MemoryStream());

//			_mockRequest = new Mock<HttpWebRequest>();
//			_mockRequest.SetupGet(x => x.Headers).Returns(headerCollection);
//			_mockRequest.Setup(x => x.GetRequestStream()).Returns(new MemoryStream());
//			_mockRequest.Setup(x => x.GetResponse()).Returns(() => mockResponse.Object);

//			var mockWebRequestCreate = new Mock<IWebRequestCreate>();
//			mockWebRequestCreate.Setup(x => x.Create(It.IsAny<Uri>())).Returns(() => _mockRequest.Object);
//			WebRequest.RegisterPrefix("http://localhost", mockWebRequestCreate.Object);

//			_xmlClient = new XmlRestClient(new Uri("http://localhost"));
//		}

//		[Test]
//		[Ignore("Cannot set accept header in mock.")]
//		public void WhenPerformingGetRequestThenSetsAcceptHeader()
//		{
//			_xmlClient.Get(new Uri("http://localhost?$orderby=Value"));

//			_mockRequest.Object.Headers[HttpRequestHeader.Accept] = XmlHeader;
//		}

//		[Test]
//		[Ignore("Cannot set accept header in mock.")]
//		public void WhenPerformingPostRequestThenSetsAcceptHeader()
//		{
//			_xmlClient.Post(new Uri("http://localhost?$orderby=Value"), "[]".ToStream());

//			_mockRequest.Object.Headers[HttpRequestHeader.Accept] = XmlHeader;
//		}

//		[Test]
//		[Ignore("Cannot set accept header in mock.")]
//		public void WhenPerformingPutRequestThenSetsAcceptHeader()
//		{
//			_xmlClient.Put(new Uri("http://localhost?$orderby=Value"), "[]".ToStream());

//			_mockRequest.Object.Headers[HttpRequestHeader.Accept] = XmlHeader;
//		}

//		[Test]
//		[Ignore("Cannot set accept header in mock.")]
//		public void WhenPerformingDeleteRequestThenSetsAcceptHeader()
//		{
//			_xmlClient.Delete(new Uri("http://localhost?$orderby=Value"));

//			_mockRequest.Object.Headers[HttpRequestHeader.Accept] = XmlHeader;
//		}

//		[Test]
//		[Ignore("Cannot set accept header in mock.")]
//		public void WhenDisposingThenDoesNotThrow()
//		{
//			Assert.DoesNotThrow(() => _xmlClient.Dispose());
//		}

//		[Test]
//		[Ignore("Cannot set accept header in mock.")]
//		public void WhenPerformingPostRequestThenSetsPostMethod()
//		{
//			_xmlClient.Post(new Uri("http://localhost?$orderby=Value"), "[]".ToStream());

//			_mockRequest.VerifySet(x => x.Method = "POST");
//		}

//		[Test]
//		[Ignore("Cannot set accept header in mock.")]
//		public void WhenPerformingPutRequestThenSetsPutMethod()
//		{
//			_xmlClient.Put(new Uri("http://localhost?$orderby=Value"), "[]".ToStream());

//			_mockRequest.VerifySet(x => x.Method = "PUT");
//		}

//		[Test]
//		[Ignore("Cannot set accept header in mock.")]
//		public void WhenPerformingDeleteRequestThenSetsDeleteMethod()
//		{
//			_xmlClient.Delete(new Uri("http://localhost?$orderby=Value"));

//			_mockRequest.VerifySet(x => x.Method = "DELETE");
//		}
//	}
//}