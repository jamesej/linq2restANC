// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeTypeProviderTests.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RuntimeTypeProviderTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests
{
	using System;
	using System.Reflection;
	using Linq2Rest.Parser;
	using NUnit.Framework;

	[TestFixture]
	public class RuntimeTypeProviderTests
	{
		private RuntimeTypeProvider _typeProvider;

		[SetUp]
		public void Setup()
		{
			_typeProvider = new RuntimeTypeProvider(new MemberNameResolver());
		}

		[Test]
		public void WhenCreatingDynamicTypeThenTransfersCustomAttributesWithDefaultConstructor()
		{
			var properties = new[] { typeof(FakeItem).GetProperty("DateValue") };

			var dynamicType = _typeProvider.Get(typeof(FakeItem), properties);

			Assert.AreEqual(1, dynamicType.GetProperties().Length);
			Assert.NotNull(dynamicType.GetProperty("DateValue"));
		}

		[Test]
		public void WhenCreatingDynamicTypeWithNoPropertiesThenThrows()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => _typeProvider.Get(typeof(FakeItem), new PropertyInfo[0]));
		}

		[Test]
		public void WhenCreatingDynamicTypeWithNullFiledsThenThrows()
		{
			PropertyInfo[] propertyInfos = null;
			Assert.Throws<ArgumentNullException>(() => _typeProvider.Get(typeof(FakeItem), propertyInfos));
		}

		[Test]
		public void WhenCreatingDynamicTypeWithOnePropertyInfoThenCreatesTypeWithOneProperty()
		{
			var properties = new[] { typeof(FakeItem).GetProperty("ChoiceValue") };

			var dynamicType = _typeProvider.Get(typeof(FakeItem), properties);

			var dataMemberAttribute = dynamicType
				.GetProperty("Choice")
				.GetCustomAttributes(false);

			Assert.IsNotEmpty(dataMemberAttribute);
		}

		[Test]
		public void WhenCreatingDynamicTypeWithOnePropertyInfoThenCreatesTypeWithOnePropertyWhereTypeMatchesProperty()
		{
			var properties = new[] { typeof(FakeItem).GetProperty("DateValue") };

			var dynamicType = _typeProvider.Get(typeof(FakeItem), properties);
			var property = dynamicType.GetProperty("DateValue");

			Assert.AreEqual(typeof(DateTime), property.PropertyType);
		}

		[Test]
		public void WhenCreatingDynamicTypeWithOnePropertyInfoThenGettingValueReturnsSetValue()
		{
			var expected = DateTime.UtcNow;
			var properties = new[] { typeof(FakeItem).GetProperty("DateValue") };

			var dynamicType = _typeProvider.Get(typeof(FakeItem), properties);

            //dynamic instance = Activator.CreateInstance(dynamicType);
            //instance.DateValue = expected;

            object instance = Activator.CreateInstance(dynamicType);
            var dvProp = dynamicType.GetProperty("DateValue");
            dvProp.SetValue(instance, expected);

			Assert.AreEqual(expected, (DateTime)dvProp.GetValue(instance));
		}

		[Test]
		public void WhenCreatingRuntimeTypeWithAttributeThenSetCustomAttribute()
		{
			var properties = new[] { typeof(FakeItem).GetProperty("DateValue") };

			var dynamicType = _typeProvider.Get(typeof(FakeItem), properties);

			var dataMemberAttribute = dynamicType
				.GetCustomAttributes(false);
			var data = dynamicType.GetCustomAttributesData();
			Assert.IsNotEmpty(dataMemberAttribute);
		}
	}
}