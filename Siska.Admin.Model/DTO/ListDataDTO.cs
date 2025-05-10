namespace Siska.Admin.Model.DTO
{
    public record ListDataDTO
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string SearchField { get; set; }
        public List<SearchTerm> Search { get; set; }

        public class SearchTerm
        {
            public string Field { get; set; }
            public string Opr { get; set; }
            public object Value { get; set; }
            public string Added { get; set; }
        }
    }
}
