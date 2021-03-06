using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Database
{
	[AttributeUsage(AttributeTargets.Class)]
	public class TableAttribute : Attribute
	{
		public string Name { get; set; }

		public TableAttribute(string name)
		{
			Name = name;
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class SchemaAttribute : Attribute
	{
		public string Name { get; set; }

		public SchemaAttribute(string name)
		{
			Name = name;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class ColumnAttribute : Attribute
	{
		public string Name { get; set; }

		public ColumnAttribute(string name)
		{
			Name = name;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class PrimaryKeyAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class AutoIncrementAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class IndexedAttribute : Attribute
	{
		public string Name { get; set; }
		public int Order { get; set; }
		public virtual bool Unique { get; set; }

		public IndexedAttribute()
		{
		}

		public IndexedAttribute(string name, int order)
		{
			Name = name;
			Order = order;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class IgnoreAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class UniqueAttribute : IndexedAttribute
	{
		public override bool Unique
		{
			get { return true; }
			set { /* throw?  */ }
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class MaxLengthAttribute : Attribute
	{
		public int Value { get; private set; }

		public MaxLengthAttribute(int length)
		{
			Value = length;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class CollationAttribute : Attribute
	{
		public string Value { get; private set; }

		public CollationAttribute(string collation)
		{
			Value = collation;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class NotNullAttribute : Attribute
	{
	}
}
