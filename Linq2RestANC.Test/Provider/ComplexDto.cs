// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComplexDto.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ComplexDto type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Provider
{
	using System;

	public class ComplexDto
	{
		public int ID { get; set; }

		public string Content { get; set; }

		public double Value { get; set; }

		public DateTime Date { get; set; }

		public Choice Choice { get; set; }

		public ChildDto Child { get; set; }
	}
}