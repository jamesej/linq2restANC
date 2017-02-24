// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanValueWriterTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the BooleanValueWriterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Provider.Writers
{
	using Linq2Rest.Provider.Writers;
	using NUnit.Framework;

	[TestFixture]
	public class BooleanValueWriterTests
	{
		private BooleanValueWriter _writer;

		[SetUp]
		public void Setup()
		{
			_writer = new BooleanValueWriter();
		}

		[Test]
		public void WhenWritingBooleanThenEnclosesInSingleQuote()
		{
			var result = _writer.Write(true);

			Assert.AreEqual("true", result);
		}
	}
}