// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryableExtensionsTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the QueryableExtensionsTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Provider
{
	using System.Linq;
	using System.Threading;
	using Linq2Rest.Provider;
	using NUnit.Framework;

	[TestFixture]
	public class QueryableExtensionsTests
	{
		[Test]
		public void WhenExecutingQueryAsynchronouslyThenDoesNotExecuteOnTestThread()
		{
			var testThreadId = Thread.CurrentThread.ManagedThreadId;
			var source = new[] { 1, 2, 3, 4, 5 };
			var queryableTask = source
				.AsQueryable()
				.Select(x => new { ThreadId = Thread.CurrentThread.ManagedThreadId })
				.ExecuteAsync();

			queryableTask.Wait();

			var queryableResult = queryableTask.Result.First();
		
			Assert.AreNotEqual(testThreadId, queryableResult.ThreadId);
		}
	}
}
