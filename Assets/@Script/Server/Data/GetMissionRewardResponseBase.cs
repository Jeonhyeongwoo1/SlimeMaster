using SlimeMaster.Enum;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Shared;

namespace Script.Server.Data
{
    public class GetMissionRewardResponseBase : ResponseBase
    {
        public DBMissionContainerData DBMissionContainerData;
        public DBItemData DBRewardItemData;
    }

    public class GetMissionRewardRequest : RequestBase
    {
        public int missionId;
        public MissionType missionType;
    }
}