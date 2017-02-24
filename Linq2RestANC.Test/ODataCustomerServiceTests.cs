// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ODataCustomerServiceTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ODataCustomerServiceTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests
{
	using System;
	using System.Linq;
	using Linq2Rest.Implementations;
	using Linq2Rest.Provider;
	using Linq2Rest.Tests.Fakes;
	using NUnit.Framework;

	[TestFixture]
	public class ODataCustomerServiceTests
	{
		private RestContext<NorthwindCustomer> _customerContext;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			// Tests against the sample OData service.
			_customerContext = new RestContext<NorthwindCustomer>(
				new JsonRestClient(new Uri("http://services.odata.org/Northwind/Northwind.svc/Customers")), 
				new TestODataSerializerFactory());
		}

		[Test]
		public void WhenAsyncRequestingCustomerByNameEndsWithThenLoadsCustomer()
		{
			var task = _customerContext.Query.Where(x => x.CompanyName.EndsWith("Futterkiste")).ExecuteAsync();

			task.Wait();
			var results = task.Result.ToArray();

			Assert.Less(0, results.Length);
		}

		[Test]
		public void WhenAsyncRequestingCustomerByNameLengthThenLoadsCustomer()
		{
			var task = _customerContext.Query.Where(x => x.CompanyName.Length > 10).ExecuteAsync();

			task.Wait();
			var results = task.Result.ToArray();

			Assert.Less(0, results.Length);
		}

		[Test]
		public void WhenAsyncRequestingCustomerByNameStartsWithThenLoadsCustomer()
		{
			var task = _customerContext.Query.Where(x => x.CompanyName.StartsWith("Alfr")).ExecuteAsync();

			task.Wait();
			var results = task.Result.ToArray();

			Assert.Less(0, results.Length);
		}

		[Test]
		public void WhenAsyncRequestingCustomerByNameThenLoadsCustomer()
		{
			var task = _customerContext.Query.Where(x => x.CompanyName.IndexOf("Alfreds") > -1).ExecuteAsync();

			task.Wait();
			var results = task.Result.ToArray();

			Assert.Less(0, results.Length);
		}

		[Test]
		public void WhenRequestingCustomerByNameEndsWithThenLoadsCustomer()
		{
			var results = _customerContext.Query.Where(x => x.CompanyName.EndsWith("Futterkiste")).ToArray();

			Assert.Less(0, results.Length);
		}

		[Test]
		public void WhenRequestingCustomerByNameLengthThenLoadsCustomer()
		{
			var results = _customerContext.Query.Where(x => x.CompanyName.Length > 10).ToArray();

			Assert.Less(0, results.Length);
		}

		[Test]
		public void WhenRequestingCustomerByNameStartsWithThenLoadsCustomer()
		{
			var results = _customerContext.Query.Where(x => x.CompanyName.StartsWith("Alfr")).ToArray();

			Assert.Less(0, results.Length);
		}

		[Test]
		public void WhenRequestingCustomerByNameThenLoadsCustomer()
		{
			var results = _customerContext.Query.Where(x => x.CompanyName.IndexOf("Alfreds") > -1).ToArray();

			Assert.Less(0, results.Length);
		}

		[Test]
		public void WhenRequestingCustomerCountByNameStartsWithThenReturnsCount()
		{
			var result = _customerContext.Query.Count(x => x.CompanyName.StartsWith("Alfr"));

			Assert.Less(0, result);
		}
	}
}