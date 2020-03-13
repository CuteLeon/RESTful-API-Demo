using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace RESTful_API_Demo.Assists
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(
            this IEnumerable<TSource> source,
            string fields)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var propertyInfoList = new List<PropertyInfo>();
            if (string.IsNullOrEmpty(fields))
            {
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                var fieldNames = fields.Split(",");
                foreach (var field in fieldNames)
                {
                    var propertyName = field.Trim();
                    var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo == null)
                    {
                        throw new InvalidOperationException($"未找到名为 {propertyName} 的属性！");
                    }
                    propertyInfoList.Add(propertyInfo);
                }
            }

            var expandoObjects = source
                .Select(item =>
                {
                    var expandoObject = new ExpandoObject();
                    foreach (var property in propertyInfoList)
                    {
                        var propertyValue = property.GetValue(item);
                        expandoObject.TryAdd(property.Name, propertyValue);
                    }
                    return expandoObject;
                })
                .ToList();

            return expandoObjects;
        }
    }
}
