using SlimeMaster.Firebase.Data;
using SlimeMaster.Shared;

namespace Script.Server.Data
{
    public class GetMissionRewardResponse : Response
    {
        public DBMissionContainerData DBMissionContainerData;
        public DBItemData DBRewardItemData;
    }
}