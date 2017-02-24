// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestFactoryTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the HttpRequestFactoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

//namespace Linq2Rest.Tests.Implementations
//{
//	using System;
//	using System.Net;
//	using Linq2Rest.Implementations;
//	using Linq2Rest.Provider;
//	using NUnit.Framework;

//	[TestFixture]
//	public class HttpRequestFactoryTests
//	{
//		private IHttpRequestFactory _httpRequestFactory;

//		[SetUp]
//		public void SetupFixture()
//		{
//			_httpRequestFactory = new HttpRequestFactory();
//		}

//		[TestCase("http://test.com/", HttpMethod.Get, "text/html", null, ExpectedResult = "http://test.com/")]
//		[TestCase("http://test.com/", HttpMethod.Post, "text/html", "text/json", ExpectedResult = "http://test.com/")]
//		public string CreateShouldReturnHttpRequestWithCorrectUri(string uriString, HttpMethod httpMethod, string accept, string contentType)
//		{
//			var httpRequest = _httpRequestFactory.Create(new Uri(uriString), httpMethod, accept, contentType);

//			var httpWebRequestAdapter = (HttpWebRequestAdapter)httpRequest;
//			var actualHttpWebRequest = (HttpWebRequest)httpWebRequestAdapter.HttpWebRequest;

//			return actualHttpWebRequest.RequestUri.ToString();
//		}

//		[TestCase("http://test.com/", HttpMethod.Get, "text/html", null, ExpectedResult = HttpMethod.Get)]
//		[TestCase("http://test.com/", HttpMethod.Post, "text/html", "text/json", ExpectedResult = HttpMethod.Post)]
//		public HttpMethod CreateShouldReturnHttpRequestWithCorrectHttpMethod(string uriString, HttpMethod httpMethod, string accept, string contentType)
//		{
//			var httpRequest = _httpRequestFactory.Create(new Uri(uriString), httpMethod, accept, contentType);

//			var httpWebRequestAdapter = (HttpWebRequestAdapter)httpRequest;
//			var actualHttpWebRequest = (HttpWebRequest)httpWebRequestAdapter.HttpWebRequest;

//			return (HttpMethod)Enum.Parse(typeof(HttpMethod), actualHttpWebRequest.Method, true);
//		}

//		[TestCase("http://test.com/", HttpMethod.Get, "text/html", null, ExpectedResult = "text/html")]
//		[TestCase("http://test.com/", HttpMethod.Post, "text/html", "text/json", ExpectedResult = "text/html")]
//		public string CreateShouldReturnHttpRequestWithCorrectAccept(string uriString, HttpMethod httpMethod, string accept, string contentType)
//		{
//			var httpRequest = _httpRequestFactory.Create(new Uri(uriString), httpMethod, accept, contentType);

//			var httpWebRequestAdapter = (HttpWebRequestAdapter)httpRequest;
//			var actualHttpWebRequest = (HttpWebRequest)httpWebRequestAdapter.HttpWebRequest;

//			return actualHttpWebRequest.Accept;
//		}

//		[TestCase("http://test.com/", HttpMethod.Get, "text/html", null, ExpectedResult = null)]
//		[TestCase("http://test.com/", HttpMethod.Post, "text/html", "text/json", ExpectedResult = "text/json")]
//		public string CreateShouldReturnHttpRequestWithCorrectContentType(string uriString, HttpMethod httpMethod, string accept, string contentType)
//		{
//			var httpRequest = _httpRequestFactory.Create(new Uri(uriString), httpMethod, accept, contentType);

//			var httpWebRequestAdapter = (HttpWebRequestAdapter)httpRequest;
//			var actualHttpWebRequest = (HttpWebRequest)httpWebRequestAdapter.HttpWebRequest;

//			return actualHttpWebRequest.ContentType;
//		}

//		[TestCase("http://test.com/", HttpMethod.Get, "text/html", null, ExpectedResult = 0)]
//		[TestCase("http://test.com/", HttpMethod.Post, "text/html", "text/json", ExpectedResult = 0)]
//		public int CreateShouldReturnHttpRequestWithCorrectClientCertificateCount(string uriString, HttpMethod httpMethod, string accept, string contentType)
//		{
//			var httpRequest = _httpRequestFactory.Create(new Uri(uriString), httpMethod, accept, contentType);

//			var httpWebRequestAdapter = (HttpWebRequestAdapter)httpRequest;
//			var actualHttpWebRequest = (HttpWebRequest)httpWebRequestAdapter.HttpWebRequest;

//			return actualHttpWebRequest.ClientCertificates.Count;
//		}

//		[TestCase("http://test.com/", HttpMethod.Post, "text/xml", "text/json", "http://test.com/", HttpMethod.Get, "text/html", null)]
//		public void TwoCreatesShouldReturnHttpRequestsWithCorrectUri(
//			string uriString1,
//			HttpMethod httpMethod1,
//			string accept1,
//			string contentType1,
//			string uriString2,
//			HttpMethod httpMethod2,
//			string accept2,
//			string contentType2)
//		{
//			var httpRequest1 = _httpRequestFactory.Create(new Uri(uriString1), httpMethod1, accept1, contentType1);
//			var httpRequest2 = _httpRequestFactory.Create(new Uri(uriString2), httpMethod2, accept2, contentType2);

//			var httpWebRequestAdapter1 = (HttpWebRequestAdapter)httpRequest1;
//			var actualHttpWebRequest1 = (HttpWebRequest)httpWebRequestAdapter1.HttpWebRequest;

//			var httpWebRequestAdapter2 = (HttpWebRequestAdapter)httpRequest2;
//			var actualHttpWebRequest2 = (HttpWebRequest)httpWebRequestAdapter2.HttpWebRequest;

//			Assert.AreEqual(uriString1, actualHttpWebRequest1.RequestUri.ToString());
//			Assert.AreEqual(uriString2, actualHttpWebRequest2.RequestUri.ToString());
//		}

//		[TestCase("http://test.com/", HttpMethod.Post, "text/xml", "text/json", "http://test.com/", HttpMethod.Get, "text/html", null)]
//		public void TwoCreatesShouldReturnHttpRequestsWithCorrectHttpMethod(
//			string uriString1,
//			HttpMethod httpMethod1,
//			string accept1,
//			string contentType1,
//			string uriString2,
//			HttpMethod httpMethod2,
//			string accept2,
//			string contentType2)
//		{
//			var httpRequest1 = _httpRequestFactory.Create(new Uri(uriString1), httpMethod1, accept1, contentType1);
//			var httpRequest2 = _httpRequestFactory.Create(new Uri(uriString2), httpMethod2, accept2, contentType2);

//			var httpWebRequestAdapter1 = (HttpWebRequestAdapter)httpRequest1;
//			var actualHttpWebRequest1 = (HttpWebRequest)httpWebRequestAdapter1.HttpWebRequest;

//			var httpWebRequestAdapter2 = (HttpWebRequestAdapter)httpRequest2;
//			var actualHttpWebRequest2 = (HttpWebRequest)httpWebRequestAdapter2.HttpWebRequest;

//			var actualHttpMethod1 = (HttpMethod)Enum.Parse(typeof(HttpMethod), actualHttpWebRequest1.Method, true);
//			var actualHttpMethod2 = (HttpMethod)Enum.Parse(typeof(HttpMethod), actualHttpWebRequest2.Method, true);

//			Assert.AreEqual(httpMethod1, actualHttpMethod1);
//			Assert.AreEqual(httpMethod2, actualHttpMethod2);
//		}

//		[TestCase("http://test.com/", HttpMethod.Post, "text/xml", "text/json", "http://test.com/", HttpMethod.Get, "text/html", null)]
//		public void TwoCreatesShouldReturnHttpRequestsWithCorrectAccept(
//			string uriString1,
//			HttpMethod httpMethod1,
//			string accept1,
//			string contentType1,
//			string uriString2,
//			HttpMethod httpMethod2,
//			string accept2,
//			string contentType2)
//		{
//			var httpRequest1 = _httpRequestFactory.Create(new Uri(uriString1), httpMethod1, accept1, contentType1);
//			var httpRequest2 = _httpRequestFactory.Create(new Uri(uriString2), httpMethod2, accept2, contentType2);

//			var httpWebRequestAdapter1 = (HttpWebRequestAdapter)httpRequest1;
//			var httpWebRequestAdapter2 = (HttpWebRequestAdapter)httpRequest2;

//			var actualHttpWebRequest1 = (HttpWebRequest)httpWebRequestAdapter1.HttpWebRequest;
//			var actualHttpWebRequest2 = (HttpWebRequest)httpWebRequestAdapter2.HttpWebRequest;

//			Assert.AreEqual(accept1, actualHttpWebRequest1.Accept);
//			Assert.AreEqual(accept2, actualHttpWebRequest2.Accept);
//		}

//		[TestCase("http://test.com/", HttpMethod.Post, "text/xml", "text/json", "http://test.com/", HttpMethod.Get, "text/html", null)]
//		public void TwoCreatesShouldReturnHttpRequestsWithCorrectContentType(
//			string uriString1,
//			HttpMethod httpMethod1,
//			string accept1,
//			string contentType1,
//			string uriString2,
//			HttpMethod httpMethod2,
//			string accept2,
//			string contentType2)
//		{
//			var httpRequest1 = _httpRequestFactory.Create(new Uri(uriString1), httpMethod1, accept1, contentType1);
//			var httpRequest2 = _httpRequestFactory.Create(new Uri(uriString2), httpMethod2, accept2, contentType2);

//			var httpWebRequestAdapter1 = (HttpWebRequestAdapter)httpRequest1;
//			var httpWebRequestAdapter2 = (HttpWebRequestAdapter)httpRequest2;

//			var actualHttpWebRequest1 = (HttpWebRequest)httpWebRequestAdapter1.HttpWebRequest;
//			var actualHttpWebRequest2 = (HttpWebRequest)httpWebRequestAdapter2.HttpWebRequest;

//			Assert.AreEqual(contentType1, actualHttpWebRequest1.ContentType);
//			Assert.AreEqual(contentType2, actualHttpWebRequest2.ContentType);
//		}

//		[TestCase("http://test.com/", HttpMethod.Post, "text/xml", "text/json", "http://test.com/", HttpMethod.Get, "text/html", null)]
//		public void TwoCreatesShouldReturnHttpRequestsWithCorrectClientCertificateCount(
//			string uriString1,
//			HttpMethod httpMethod1,
//			string accept1,
//			string contentType1,
//			string uriString2,
//			HttpMethod httpMethod2,
//			string accept2,
//			string contentType2)
//		{
//			var httpRequest1 = _httpRequestFactory.Create(new Uri(uriString1), httpMethod1, accept1, contentType1);
//			var httpRequest2 = _httpRequestFactory.Create(new Uri(uriString2), httpMethod2, accept2, contentType2);

//			var httpWebRequestAdapter1 = (HttpWebRequestAdapter)httpRequest1;
//			var httpWebRequestAdapter2 = (HttpWebRequestAdapter)httpRequest2;

//			var actualHttpWebRequest1 = (HttpWebRequest)httpWebRequestAdapter1.HttpWebRequest;
//			var actualHttpWebRequest2 = (HttpWebRequest)httpWebRequestAdapter2.HttpWebRequest;

//			Assert.AreEqual(0, actualHttpWebRequest1.ClientCertificates.Count);
//			Assert.AreEqual(0, actualHttpWebRequest2.ClientCertificates.Count);
//		}
//	}
//}