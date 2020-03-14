using System;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace RESTful_API_Demo.ActionConstraints
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class RequestHeaderMatchsMediaTypeAttribute : Attribute, IActionConstraint
    {
        private readonly string requestHeaderToMatch;
        private readonly MediaTypeCollection mediaTypes = new MediaTypeCollection();

        public int Order { get; set; }

        public RequestHeaderMatchsMediaTypeAttribute(
            string requestHeaderToMatch,
            string mediaType,
            params string[] otherMediaTypes)
        {
            this.requestHeaderToMatch = requestHeaderToMatch;
            if (MediaTypeHeaderValue.TryParse(mediaType, out var parsedMediaType))
            {
                this.mediaTypes.Add(parsedMediaType);
            }
            else
            {
                throw new ArgumentException(nameof(mediaType));
            }

            foreach (var otherMediaType in otherMediaTypes)
            {
                if (MediaTypeHeaderValue.TryParse(otherMediaType, out var parsedOtherMediaType))
                {
                    this.mediaTypes.Add(parsedOtherMediaType);
                }
                else
                {
                    throw new ArgumentException(nameof(otherMediaType));
                }
            }
        }

        public bool Accept(ActionConstraintContext context)
        {
            var requestHeaders = context.RouteContext.HttpContext.Request.Headers;
            if (!requestHeaders.ContainsKey(requestHeaderToMatch))
            {
                return false;
            }

            var parsedRequestMediaType = new MediaType(requestHeaders[this.requestHeaderToMatch]);
            foreach (var mediaType in this.mediaTypes)
            {
                var parsedMediaType = new MediaType(mediaType);
                if (parsedRequestMediaType.Equals(parsedMediaType))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
