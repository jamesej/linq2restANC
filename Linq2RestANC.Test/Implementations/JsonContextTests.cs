// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonContextTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the JsonContextTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Implementations
{
	using System;
	using Linq2Rest.Implementations;
	using NUnit.Framework;

	[TestFixture]
	public class JsonContextTests
	{
		[Test]
		public void CanCreateInstance()
		{
			Assert.DoesNotThrow(() => { var instance = new JsonContext<FakeItem>(new Uri("http://server")); });
		}
	}
}
