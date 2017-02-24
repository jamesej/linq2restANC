// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecimalValueWriterTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DecimalValueWriterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Provider.Writers
{
	using Linq2Rest.Provider.Writers;
	using NUnit.Framework;

	[TestFixture]
	public class DecimalValueWriterTests
	{
		private DecimalValueWriter _writer;

		[SetUp]
		public void Setup()
		{
			_writer = new DecimalValueWriter();
		}

		[Test]
		public void WhenWritingDecimalValueThenWritesString()
		{
			var result = _writer.Write(1.23m);

			Assert.AreEqual("1.23m", result);
		}
	}
}
