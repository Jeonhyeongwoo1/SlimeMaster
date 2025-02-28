using System;
using System.Collections.Generic;
using SlimeMaster.Enum;
using SlimeMaster.Firebase.Data;

namespace SlimeMaster.Shared.Data
{
    public class UserResponseBase : ResponseBase
    {
        public DBUserData DBUserData;
        public DBCheckoutData DBCheckoutData;
        public DBMissionContainerData DBMissionContainerData;
        public DBAchievementContainerData DBAchievementContainerData;
        public DateTime LastLoginTime;
        public DateTime LastOfflineGetRewardTime;
    }

    public class RewardResponseBase : ResponseBase
    {
        public DBItemData DBItemData;
        public DBStageData DBStageData;
    }
    
    
    public class UseStaminaRequest : RequestBase
    {
        public int staminaCount { get; set; }
    }

    public class StageClearRequest : RequestBase
    {
        public int stageIndex { get; set; }
    }
    
    public class GetWaveClearRewardRequest : RequestBase
    {
        public int stageIndex;
        public WaveClearType waveClearType;
    }
}