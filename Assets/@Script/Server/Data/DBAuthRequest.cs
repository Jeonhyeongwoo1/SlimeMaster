using SlimeMaster.Shared;

namespace Script.Server.Data
{
    public class DBAuthRequest
    {
        public string uid;
    }
    
    public class DBAuthResponseBase : ResponseBase
    {
        public string uid;
        public string token;
    }
}