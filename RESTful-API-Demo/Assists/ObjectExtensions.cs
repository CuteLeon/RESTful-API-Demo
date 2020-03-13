using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace RESTful_API_Demo.Assists
{
    public static class ObjectExtensions
    {
        public static ExpandoObject ShapeData<TSource>(this TSource source, string fields)
        {
            if (source == null)
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
                foreach (var field in fields.Split(","))
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

            var expandoObject = new ExpandoObject();
            foreach (var property in propertyInfoList)
            {
                var propertyValue = property.GetValue(source);
                expandoObject.TryAdd(property.Name, propertyValue);
            }
            return expandoObject;
        }
    }
}
