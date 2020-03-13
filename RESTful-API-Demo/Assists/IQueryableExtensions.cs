using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using RESTful_API_Demo.PropertyMappingServices;

namespace RESTful_API_Demo.Assists
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(
            this IQueryable<T> source,
            string orderBy,
            Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (string.IsNullOrEmpty(orderBy))
            {
                return source;
            }

            if (mappingDictionary == null)
                throw new ArgumentNullException(nameof(mappingDictionary));

            var orderByMembers = orderBy.Split(",");
            foreach (var orderByMemberValue in orderByMembers.Reverse())
            {
                var orderByMemberValues = orderByMemberValue.Trim().ToLower().Split(" ", 2);
                var orderByMemberName = string.Empty;
                var orderDescending = false;
                switch (orderByMemberValues.Length)
                {
                    case 1:
                        {
                            orderByMemberName = orderByMemberValues[0];
                            break;
                        }

                    case 2:
                        {
                            orderByMemberName = orderByMemberValues[0];
                            orderDescending = orderByMemberValues[1] == "desc";
                            break;
                        }
                }

                if (!mappingDictionary.TryGetValue(orderByMemberName, out var mappingValue))
                {
                    throw new ArgumentException($"未找到名为 {orderByMemberName} 的属性映射！");
                }

                if (mappingValue.Revert)
                {
                    orderDescending = !orderDescending;
                }

                foreach (var destinationProperty in mappingValue.DestinationProperties)
                {
                    source = source.OrderBy($"{destinationProperty} {(orderDescending ? "descending" : "ascending")}");
                }
            }

            return source;
        }
    }
}
