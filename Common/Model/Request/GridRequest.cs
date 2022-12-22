namespace Common.Model.Request
{
    public class GridRequest
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string Active { get; set; }

        public string Direction { get; set; }

    }
}
