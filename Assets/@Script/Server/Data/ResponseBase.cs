using SlimeMaster.Enum;

namespace SlimeMaster.Shared
{
    public class ResponseBase
    {
        public ServerErrorCode responseCode;
        public string errorMessage;
    }
    
    public class RequestBase
    {
        public string userId { get; set; }
    }
}