namespace NET_WebScraping_API.Models
{
    public class PageSanction : IPage<DataSanction>
    {
        public PageSanction(int count, List<DataSanction> response)
        {
            this.count = count;
            this.response = response;
        }

        public int count { get; set; }
        public List<DataSanction> response { get; set; }
    }
}
