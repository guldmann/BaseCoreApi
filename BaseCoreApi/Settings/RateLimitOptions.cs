
namespace BaseCoreApi.Settings
{
    public class RateLimitOptions
    {
        public  int Limit { get; set; }
        public int TimeSec { get; set; }
        public string[] WhiteList { get; set; }
    }
}
