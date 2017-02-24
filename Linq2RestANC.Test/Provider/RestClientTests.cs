// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestClientTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestClientTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Provider
{
	using System;
	using Linq2Rest.Implementations;
	using NUnit.Framework;

	[TestFixture]
	public class RestClientTests
	{
		[Test]
		public void WhenCreatingRestClientThenSetsServiceBase()
		{
			var uri = new Uri("http://localhost");

			var client = new JsonRestClient(uri);

			Assert.AreEqual(uri, client.ServiceBase);
		}
	}
}
