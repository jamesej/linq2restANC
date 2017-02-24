// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestFactoryWithCertificateTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the HttpRequestFactoryWithCertificateTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

//namespace Linq2Rest.Tests.Implementations
//{
//	using System;
//	using System.Security.Cryptography.X509Certificates;
//	using Linq2Rest.Implementations;
//	using Linq2Rest.Provider;
//	using NUnit.Framework;

//	[TestFixture]
//	public class HttpRequestFactoryWithCertificateTests
//	{
//		private const string AcceptMimeType = "text/html";

//		private IHttpRequestFactory _httpRequestFactory;
//		private Uri _uri;

//		[SetUp]
//		public void SetupFixture()
//		{
//			_uri = new Uri("http://test.com/");
//			_httpRequestFactory = new HttpRequestFactoryWithCertificate(new X509Certificate());
//		}

//		[TestCase(HttpMethod.Get, null, ExpectedResult = "http://test.com/")]
//		[TestCase(HttpMethod.Post, "text/json", ExpectedResult = "http://test.com/")]
//		public string CreateShouldReturnHttpRequestWithCorrectUri(HttpMethod httpMethod, string contentType)
//		{
//			var httpRequest = _httpRequestFactory.Create(_uri, httpMethod, AcceptMimeType, contentType);

//			var httpWebRequestAdapter = (HttpWebRequestAdapter)httpRequest;
//			var actualHttpWebRequest = httpWebRequestAdapter.HttpWebRequest;

//			return actualHttpWebRequest.RequestUri.ToString();
//		}

//		[TestCase(HttpMethod.Get, null, ExpectedResult = HttpMethod.Get)]
//		[TestCase(HttpMethod.Post, "text/json", ExpectedResult = HttpMethod.Post)]
//		public HttpMethod CreateShouldReturnHttpRequestWithCorrectHttpMethod(HttpMethod httpMethod, string contentType)
//		{
//			var httpRequest = _httpRequestFactory.Create(_uri, httpMethod, AcceptMimeType, contentType);

//			var httpWebRequestAdapter = (HttpWebRequestAdapter)httpRequest;
//			var actualHttpWebRequest = httpWebRequestAdapter.HttpWebRequest;

//			return (HttpMethod)Enum.Parse(typeof(HttpMethod), actualHttpWebRequest.Method, true);
//		}

//		[TestCase(HttpMethod.Get, null, ExpectedResult = AcceptMimeType)]
//		[TestCase(HttpMethod.Post, "text/json", ExpectedResult = AcceptMimeType)]
//		public string CreateShouldReturnHttpRequestWithCorrectAccept(HttpMethod httpMethod, string contentType)
//		{
//			var httpRequest = _httpRequestFactory.Create(_uri, httpMethod, AcceptMimeType, contentType);

//			var httpWebRequestAdapter = (HttpWebRequestAdapter)httpRequest;
//			var actualHttpWebRequest = httpWebRequestAdapter.HttpWebRequest;

//			return actualHttpWebRequest.Accept;
//		}

//		[TestCase(HttpMethod.Get, null, ExpectedResult = null)]
//		[TestCase(HttpMethod.Post, "text/json", ExpectedResult = "text/json")]
//		public string CreateShouldReturnHttpRequestWithCorrectContentType(HttpMethod httpMethod, string contentType)
//		{
//			var httpRequest = _httpRequestFactory.Create(_uri, httpMethod, AcceptMimeType, contentType);

//			var httpWebRequestAdapter = (HttpWebRequestAdapter)httpRequest;
//			var actualHttpWebRequest = httpWebRequestAdapter.HttpWebRequest;

//			return actualHttpWebRequest.ContentType;
//		}

//		[TestCase(HttpMethod.Get, null, ExpectedResult = 1)]
//		[TestCase(HttpMethod.Post, "text/json", ExpectedResult = 1)]
//		public int CreateShouldReturnHttpRequestWithCorrectClientCertificateCount(HttpMethod httpMethod, string contentType)
//		{
//			var httpRequest = _httpRequestFactory.Create(_uri, httpMethod, AcceptMimeType, contentType);

//			var httpWebRequestAdapter = (HttpWebRequestAdapter)httpRequest;
//			var actualHttpWebRequest = httpWebRequestAdapter.HttpWebRequest;

//			return actualHttpWebRequest.ClientCertificates.Count;
//		}

//		[TestCase(HttpMethod.Post, "text/xml", "text/json", HttpMethod.Get, AcceptMimeType, null)]
//		public void TwoCreatesShouldReturnHttpRequestsWithCorrectUri(HttpMethod httpMethod1, string accept1, string contentType1, HttpMethod httpMethod2, string accept2, string contentType2)
//		{
//			var httpRequest1 = _httpRequestFactory.Create(_uri, httpMethod1, accept1, contentType1);
//			var httpRequest2 = _httpRequestFactory.Create(_uri, httpMethod2, accept2, contentType2);

//			var httpWebRequestAdapter1 = (HttpWebRequestAdapter)httpRequest1;
//			var actualHttpWebRequest1 = httpWebRequestAdapter1.HttpWebRequest;

//			var httpWebRequestAdapter2 = (HttpWebRequestAdapter)httpRequest2;
//			var actualHttpWebRequest2 = httpWebRequestAdapter2.HttpWebRequest;

//			Assert.AreEqual(_uri.ToString(), actualHttpWebRequest1.RequestUri.ToString());
//			Assert.AreEqual(_uri.ToString(), actualHttpWebRequest2.RequestUri.ToString());
//		}

//		[TestCase(HttpMethod.Post, "text/xml", "text/json", HttpMethod.Get, AcceptMimeType, null)]
//		public void TwoCreatesShouldReturnHttpRequestsWithCorrectHttpMethod(HttpMethod httpMethod1, string accept1, string contentType1, HttpMethod httpMethod2, string accept2, string contentType2)
//		{
//			var httpRequest1 = _httpRequestFactory.Create(_uri, httpMethod1, accept1, contentType1);
//			var httpRequest2 = _httpRequestFactory.Create(_uri, httpMethod2, accept2, contentType2);

//			var httpWebRequestAdapter1 = (HttpWebRequestAdapter)httpRequest1;
//			var actualHttpWebRequest1 = httpWebRequestAdapter1.HttpWebRequest;

//			var httpWebRequestAdapter2 = (HttpWebRequestAdapter)httpRequest2;
//			var actualHttpWebRequest2 = httpWebRequestAdapter2.HttpWebRequest;

//			var actualHttpMethod1 = (HttpMethod)Enum.Parse(typeof(HttpMethod), actualHttpWebRequest1.Method, true);
//			var actualHttpMethod2 = (HttpMethod)Enum.Parse(typeof(HttpMethod), actualHttpWebRequest2.Method, true);

//			Assert.AreEqual(httpMethod1, actualHttpMethod1);
//			Assert.AreEqual(httpMethod2, actualHttpMethod2);
//		}

//		[TestCase(HttpMethod.Post, "text/xml", "text/json", HttpMethod.Get, AcceptMimeType, null)]
//		public void TwoCreatesShouldReturnHttpRequestsWithCorrectAccept(HttpMethod httpMethod1, string accept1, string contentType1, HttpMethod httpMethod2, string accept2, string contentType2)
//		{
//			var httpRequest1 = _httpRequestFactory.Create(_uri, httpMethod1, accept1, contentType1);
//			var httpRequest2 = _httpRequestFactory.Create(_uri, httpMethod2, accept2, contentType2);

//			var httpWebRequestAdapter1 = (HttpWebRequestAdapter)httpRequest1;
//			var httpWebRequestAdapter2 = (HttpWebRequestAdapter)httpRequest2;

//			var actualHttpWebRequest1 = httpWebRequestAdapter1.HttpWebRequest;
//			var actualHttpWebRequest2 = httpWebRequestAdapter2.HttpWebRequest;

//			Assert.AreEqual(accept1, actualHttpWebRequest1.Accept);
//			Assert.AreEqual(accept2, actualHttpWebRequest2.Accept);
//		}

//		[TestCase(HttpMethod.Post, "text/xml", "text/json", HttpMethod.Get, AcceptMimeType, null)]
//		public void TwoCreatesShouldReturnHttpRequestsWithCorrectContentType(HttpMethod httpMethod1, string accept1, string contentType1, HttpMethod httpMethod2, string accept2, string contentType2)
//		{
//			var httpRequest1 = _httpRequestFactory.Create(_uri, httpMethod1, accept1, contentType1);
//			var httpRequest2 = _httpRequestFactory.Create(_uri, httpMethod2, accept2, contentType2);

//			var httpWebRequestAdapter1 = (HttpWebRequestAdapter)httpRequest1;
//			var httpWebRequestAdapter2 = (HttpWebRequestAdapter)httpRequest2;

//			var actualHttpWebRequest1 = httpWebRequestAdapter1.HttpWebRequest;
//			var actualHttpWebRequest2 = httpWebRequestAdapter2.HttpWebRequest;

//			Assert.AreEqual(contentType1, actualHttpWebRequest1.ContentType);
//			Assert.AreEqual(contentType2, actualHttpWebRequest2.ContentType);
//		}

//		[TestCase(HttpMethod.Post, "text/xml", "text/json", HttpMethod.Get, AcceptMimeType, null)]
//		public void TwoCreatesShouldReturnHttpRequestsWithCorrectClientCertificateCount(HttpMethod httpMethod1, string accept1, string contentType1, HttpMethod httpMethod2, string accept2, string contentType2)
//		{
//			var httpRequest1 = _httpRequestFactory.Create(_uri, httpMethod1, accept1, contentType1);
//			var httpRequest2 = _httpRequestFactory.Create(_uri, httpMethod2, accept2, contentType2);

//			var httpWebRequestAdapter1 = (HttpWebRequestAdapter)httpRequest1;
//			var httpWebRequestAdapter2 = (HttpWebRequestAdapter)httpRequest2;

//			var actualHttpWebRequest1 = httpWebRequestAdapter1.HttpWebRequest;
//			var actualHttpWebRequest2 = httpWebRequestAdapter2.HttpWebRequest;

//			Assert.AreEqual(1, actualHttpWebRequest1.ClientCertificates.Count);
//			Assert.AreEqual(1, actualHttpWebRequest2.ClientCertificates.Count);
//		}
//	}
//}
