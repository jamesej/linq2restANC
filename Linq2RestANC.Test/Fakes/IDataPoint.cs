// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataPoint.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IDataPoint type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Fakes
{
	public interface IDataPoint : ITimeSeriesValue
	{
		object Extras { get; set; }
	}
}