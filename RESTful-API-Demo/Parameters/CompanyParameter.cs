using System;

namespace RESTful_API_Demo.Parameters
{
    public class CompanyParameter
    {
        const int MaxPageSize = 20;
        private int pageSize = 5;

        public string CompanyName { get; set; }
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => pageSize;
            set => this.pageSize = Math.Min(value, MaxPageSize);
        }
        public string OrderBy { get; set; } = "name";
    }
}
