// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeValueWriterTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DateTimeValueWriterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Provider.Writers
{
	using System;
	using Linq2Rest.Provider.Writers;
	using NUnit.Framework;

	[TestFixture]
	public class DateTimeValueWriterTests
	{
		private DateTimeValueWriter _writer;

		[SetUp]
		public void Setup()
		{
			_writer = new DateTimeValueWriter();
		}

		[Test]
		public void WhenWritingDateTimeValueThenWritesString()
		{
			var value = new DateTime(2012, 5, 6, 16, 11, 00, DateTimeKind.Utc);
			var result = _writer.Write(value);

			Assert.AreEqual("datetime'2012-05-06T16:11:00Z'", result);
		}

		[Test]
		public void WhenWritingDateTimeValueWithMillisecondsThenWritesString()
		{
			var value = new DateTime(2012, 5, 6, 16, 11, 00, 11, DateTimeKind.Utc);
			var result = _writer.Write(value);

			Assert.AreEqual("datetime'2012-05-06T16:11:00.011Z'", result);
		}
	}
}