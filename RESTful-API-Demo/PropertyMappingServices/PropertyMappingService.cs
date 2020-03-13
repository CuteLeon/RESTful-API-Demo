using System;
using System.Collections.Generic;
using System.Linq;

namespace RESTful_API_Demo.PropertyMappingServices
{
    public class PropertyMappingService : IPropertyMappingService
    {
        public IList<IPropertyMapping> PropertyMappings { get; }
            = new List<IPropertyMapping>();

        public IPropertyMappingService AddPropertyMapping(IPropertyMapping propertyMapping)
        {
            if (propertyMapping == null)
            {
                throw new ArgumentNullException(nameof(propertyMapping));
            }

            this.PropertyMappings.Add(propertyMapping);
            return this;
        }

        public IPropertyMapping GetPropertyMapping<TSource, TDestination>()
        {
            var matchMappings = this.PropertyMappings.OfType<PropertyMapping<TSource, TDestination>>().ToArray();
            if (matchMappings.Length == 1)
            {
                return matchMappings.First();
            }

            throw new InvalidOperationException($"发现 {matchMappings.Length} 个 {typeof(TSource).Name} 至 {typeof(TDestination)} 的属性配置！");
        }
    }
}
