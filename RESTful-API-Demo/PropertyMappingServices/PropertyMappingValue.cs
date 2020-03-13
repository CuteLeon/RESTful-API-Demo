using System;
using System.Collections.Generic;

namespace RESTful_API_Demo.PropertyMappingServices
{
    public class PropertyMappingValue
    {
        public PropertyMappingValue(
            IEnumerable<string> destinationProperties,
            bool revert = false)
        {
            this.DestinationProperties = destinationProperties ??
                throw new ArgumentNullException(nameof(destinationProperties));
            this.Revert = revert;
        }

        public IEnumerable<string> DestinationProperties { get; set; }
        public bool Revert { get; set; }
    }
}
