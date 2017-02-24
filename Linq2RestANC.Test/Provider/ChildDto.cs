// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChildDto.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ChildDto type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Provider
{
	using System;

	public class ChildDto
	{
		public int ID { get; set; }

		public string Name { get; set; }

		public Guid GlobalID { get; set; }
	}
}