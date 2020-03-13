using System.Collections.Generic;

namespace RESTful_API_Demo.PropertyMappingServices
{
    public interface IPropertyMappingService
    {
        IList<IPropertyMapping> PropertyMappings { get; }

        IPropertyMappingService AddPropertyMapping(IPropertyMapping propertyMapping);
        IPropertyMapping GetPropertyMapping<TSource, TDestination>();
    }
}