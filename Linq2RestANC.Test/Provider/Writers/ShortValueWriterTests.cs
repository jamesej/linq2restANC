// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShortValueWriterTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ShortValueWriterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Provider.Writers
{
	using Linq2Rest.Provider.Writers;
	using NUnit.Framework;

	[TestFixture]
	public class ShortValueWriterTests
	{
		private ShortValueWriter _writer;

		[SetUp]
		public void Setup()
		{
			_writer = new ShortValueWriter();
		}

		[Test]
		public void WhenWritingShortValueThenWritesString()
		{
			var result = _writer.Write((short)123);

			Assert.AreEqual("123", result);
		}
	}
}