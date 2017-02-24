// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleContractItem.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SimpleContractItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Implementations
{
	using System.Runtime.Serialization;
	using System.Xml.Serialization;

	[DataContract]
	public class SimpleContractItem
	{
		[DataMember]
		public int Value { get; set; }

		[XmlElement(ElementName = "Text")]
		[DataMember(Name = "Text")]
		public string SomeString { get; set; }
	}
}