namespace NET_WebScraping_API.Models
{
    public class PageOffshore : IPage<DataOffshore>
    {
        public PageOffshore(int count, List<DataOffshore> response)
        {
            this.count = count;
            this.response = response;
        }

        public int count { get; set; }
        public List<DataOffshore> response { get; set; }

    }
}
