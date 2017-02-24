// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualityFlags.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the QualityFlags type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Fakes
{
	public struct QualityFlags
	{
		private readonly int _value;

		public QualityFlags(int value)
		{
			_value = value;
		}

		public int Value
		{
			get
			{
				return _value;
			}
		}
	}
}