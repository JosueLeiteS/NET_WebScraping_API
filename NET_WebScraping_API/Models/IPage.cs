namespace NET_WebScraping_API.Models
{
    public interface IPage<T>
    {
        public int count { get; set; }

        public List<T> response {  get; set; } 
    }
}
