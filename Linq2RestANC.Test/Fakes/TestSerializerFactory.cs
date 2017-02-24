// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSerializerFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TestSerializerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Fakes
{
	using Linq2Rest.Provider;
	using Linq2Rest.Tests.Provider;

	public class TestSerializerFactory : ISerializerFactory
	{
		private readonly IMemberNameResolver _memberNameResolver;

		public TestSerializerFactory(IMemberNameResolver memberNameResolver)
		{
			_memberNameResolver = memberNameResolver;
		}

		public ISerializer<T> Create<T>()
		{
			if (typeof(T).IsAnonymousType())
			{
				return new RuntimeAnonymousTypeSerializer<T, T>(_memberNameResolver);
			}

			if (typeof(T) == typeof(ComplexDto))
			{
				return new TestComplexSerializer() as ISerializer<T>;
			}

			return new TestSerializer<T>();
		}

		/// <summary>
		/// Creates an instance of an <see cref="ISerializer{T}"/>.
		/// </summary>
		/// <typeparam name="T">The item type for the serializer.</typeparam>
		/// <typeparam name="TSource">The item type to provide alias metadata for the serializer.</typeparam>
		/// <returns>An instance of an <see cref="ISerializer{T}"/>.</returns>
		public ISerializer<T> Create<T, TSource>()
		{
			if (typeof(T).IsAnonymousType())
			{
				return new RuntimeAnonymousTypeSerializer<T, TSource>(_memberNameResolver);
			}

			if (typeof(T) == typeof(ComplexDto))
			{
				return new TestComplexSerializer() as ISerializer<T>;
			}

			return new TestSerializer<T>();	
		}
	}
}