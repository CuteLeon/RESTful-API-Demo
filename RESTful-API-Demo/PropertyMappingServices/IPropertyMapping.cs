using System.Collections.Generic;

namespace RESTful_API_Demo.PropertyMappingServices
{
    public interface IPropertyMapping
    {
        Dictionary<string, PropertyMappingValue> PropertyMappingValueDictionary { get; }
    }
}