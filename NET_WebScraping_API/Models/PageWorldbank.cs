namespace NET_WebScraping_API.Models
{
    public class PageWorldbank : IPage<DataWorldbank>
    {
        public PageWorldbank(int count, List<DataWorldbank> response)
        {
            this.count = count;
            this.response = response;
        }

        public int count { get; set; }
        public List<DataWorldbank> response { get; set; }
    }
}
