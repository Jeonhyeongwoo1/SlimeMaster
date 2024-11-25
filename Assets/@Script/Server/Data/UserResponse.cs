using System;
using System.Collections.Generic;
using SlimeMaster.Firebase.Data;

namespace SlimeMaster.Shared.Data
{
    public class UserResponse : Response
    {
        public DBUserData DBUserData;
        public DBCheckoutData DBCheckoutData;
        public DBMissionContainerData DBMissionContainerData;
        public DBAchievementContainerData DBAchievementContainerData;
        public DateTime LastLoginTime;
        public DateTime LastOfflineGetRewardTime;
    }

    public class RewardResponse : Response
    {
        public DBItemData DBItemData;
        public DBStageData DBStageData;
    }
}