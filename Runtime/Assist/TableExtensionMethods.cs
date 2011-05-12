﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.Assist
{
    public static class TableHelperExtensionMethods
    {
        public static T CreateInstance<T>(this Table table)
        {
            return TEHelpers.ThisTypeHasADefaultConstructor<T>()
                       ? TEHelpers.CreateTheInstanceWithTheDefaultConstructor<T>(table)
                       : TEHelpers.CreateTheInstanceWithTheValuesFromTheTable<T>(table);
        }

        public static void FillInstance<T>(this Table table, T instance)
        {
            TEHelpers.LoadInstanceWithKeyValuePairs(table, instance);
        }

        public static IEnumerable<T> CreateSet<T>(this Table table)
        {
            var list = new List<T>();

            var pivotTable = new PivotTable(table);
            for (var index = 0; index < table.Rows.Count(); index++)
            {
                var instance = pivotTable.GetInstanceTable(index).CreateInstance<T>();
                list.Add(instance);
            }

            return list;
        }

        private static bool IsPropertyMatchingToColumnName(PropertyInfo property, string columnName)
        {
            return property.Name.Equals(columnName.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase);
        }

        private static void LoadInstanceWithKeyValuePairs<T>(Table table, T instance)
        {
            var handlers = GetTypeHandlersForFieldValuePairs<T>();

            var propertiesThatNeedToBeSet = from property in typeof (T).GetProperties()
                                            from key in handlers.Keys
                                            from row in table.Rows
                                            where key.IsAssignableFrom(property.PropertyType)
                                                  && IsPropertyMatchingToColumnName(property, row[0])
                                            select new {Row = row, property.Name, Handler = handlers[key]};

            propertiesThatNeedToBeSet.ToList()
                .ForEach(x => instance.SetPropertyValue(x.Name, x.Handler(x.Row)));
        }

        private static Dictionary<Type, Func<TableRow, object>> GetTypeHandlersForFieldValuePairs<T>()
        {
            return new Dictionary<Type, Func<TableRow, object>>
                       {
                           {typeof (string), (TableRow row) => new StringValueRetriever().GetValue(row[1])},
                           {typeof (int), (TableRow row) => new IntValueRetriever().GetValue(row[1])},
                           {typeof (int?), (TableRow row) => new NullableIntValueRetriever(v => new IntValueRetriever().GetValue(v)).GetValue(row[1])},
                           {typeof (decimal), (TableRow row) => new DecimalValueRetriever().GetValue(row[1])},
                           {typeof (decimal?), (TableRow row) => new NullableDecimalValueRetriever(v => new DecimalValueRetriever().GetValue(v)).GetValue(row[1])},
                           {typeof (bool), (TableRow row) => new BoolValueRetriever().GetValue(row[1])},
                           {
                               typeof (bool?),
                               (TableRow row) => new NullableBoolValueRetriever(v => new BoolValueRetriever().GetValue(v)).GetValue(row[1])
                               },
                           {typeof (DateTime), (TableRow row) => new DateTimeValueRetriever().GetValue(row[1])},
                           {typeof (DateTime?), (TableRow row) => new NullableDateTimeValueRetriever(v => new DateTimeValueRetriever().GetValue(v)).GetValue(row[1])},
                           {typeof (Double), (TableRow row) => new DoubleValueRetriever().GetValue(row[1])},
                           {typeof (Double?), (TableRow row) => new NullableDoubleValueRetriever(v => new DoubleValueRetriever().GetValue(v)).GetValue(row[1])},
                           {typeof (Guid), (TableRow row) => new GuidValueRetriever().GetValue(row[1])},
                           {typeof (Guid?), (TableRow row) => new NullableGuidValueRetriever(v => new GuidValueRetriever().GetValue(v)).GetValue(row[1])},
                           {typeof (char), (TableRow row) => new CharValueRetriever().GetValue(row[1])},
                           {typeof (char?), (TableRow row) => new NullableCharValueRetriever(v => new CharValueRetriever().GetValue(v)).GetValue(row[1])},
                           {
                               typeof (Enum),
                               (TableRow row) =>
                               new EnumValueRetriever().GetValue(row[1], typeof (T).GetProperties().First(x => x.Name == row[0]).PropertyType)
                               }
                       };
        }
    }
}