// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSerializer.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TestSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Fakes
{
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Runtime.Serialization.Json;
	using Linq2Rest.Provider;

	public class TestSerializer<T> : ISerializer<T>
	{
		private readonly DataContractJsonSerializer _innerListSerializer = new DataContractJsonSerializer(typeof(List<T>));
		private readonly DataContractJsonSerializer _innerSerializer = new DataContractJsonSerializer(typeof(T));

		public T Deserialize(Stream input)
		{
			return (T)_innerSerializer.ReadObject(input);
		}

		public IEnumerable<T> DeserializeList(Stream input)
		{
			return (IEnumerable<T>)_innerListSerializer.ReadObject(input);
		}

		public Stream Serialize(T item)
		{
			return "[]".ToStream();
		}
	}
}