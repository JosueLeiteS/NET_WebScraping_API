namespace NET_WebScraping_API.Models
{
    public class ResponseModel
    {
        public IPage<DataOffshore> pagOffshore { get; set; }
        public IPage<DataWorldbank> pagWorldbank { get; set; }
        public IPage<DataSanction> pagSanction { get; set; }

    }
}
