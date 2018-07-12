
namespace BaseCoreApi.Models
{
    public class JWTOptions
    {
        public string ServerSecret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
