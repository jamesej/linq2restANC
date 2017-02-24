// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ODataResult.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ODataResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Fakes
{
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	[DataContract]
	public class ODataResult<T>
	{
		[DataMember(Name = "value")]
		public IEnumerable<T> Results { get; set; }
	}
}