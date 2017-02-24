// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestComplexSerializer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TestComplexSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Fakes
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.Serialization.Json;
	using Linq2Rest.Provider;
	using Linq2Rest.Tests.Provider;

	public class TestComplexSerializer : ISerializer<ComplexDto>
	{
		private readonly DataContractJsonSerializer _innerListSerializer = new DataContractJsonSerializer(typeof(List<ComplexDto>));
		private readonly DataContractJsonSerializer _innerSerializer = new DataContractJsonSerializer(typeof(ComplexDto));

		public ComplexDto Deserialize(Stream input)
		{
			return (ComplexDto)_innerSerializer.ReadObject(input);
		}

		public IEnumerable<ComplexDto> DeserializeList(Stream input)
		{
			return (List<ComplexDto>)_innerListSerializer.ReadObject(input);
		}

		public Stream Serialize(ComplexDto item)
		{
			throw new NotImplementedException();
		}
	}
}