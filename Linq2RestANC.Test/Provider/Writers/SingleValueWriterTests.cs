// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleValueWriterTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SingleValueWriterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Provider.Writers
{
	using Linq2Rest.Provider.Writers;
	using NUnit.Framework;

	[TestFixture]
	public class SingleValueWriterTests
	{
		private SingleValueWriter _writer;

		[SetUp]
		public void Setup()
		{
			_writer = new SingleValueWriter();
		}

		[Test]
		public void WhenWritingSingleValueThenWritesString()
		{
			var result = _writer.Write(1.23f);

			Assert.AreEqual("1.23f", result);
		}
	}
}