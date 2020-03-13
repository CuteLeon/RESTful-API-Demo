using System;
using System.Collections.Generic;

namespace RESTful_API_Demo.PropertyMappingServices
{
    public class PropertyMapping<TSource, TDestination> : IPropertyMapping
    {
        public PropertyMapping(
            Dictionary<string, PropertyMappingValue> propertyMappingValueDictionary)
        {
            this.PropertyMappingValueDictionary = propertyMappingValueDictionary ??
                throw new ArgumentNullException(nameof(propertyMappingValueDictionary));
        }

        public Dictionary<string, PropertyMappingValue> PropertyMappingValueDictionary { get; }
            = new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase);
    }
}
