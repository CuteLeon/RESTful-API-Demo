namespace RESTful_API_Demo.DTOS
{
    public class LinkDTO
    {
        public LinkDTO(string href, string rel, string method)
        {
            this.Href = href;
            this.Rel = rel;
            this.Method = method;
        }

        public string Href { get; set; }
        public string Rel { get; set; }
        public string Method { get; set; }
    }
}
