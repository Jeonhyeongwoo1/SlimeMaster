using System;
using System.Collections.Generic;
using SlimeMaster.Firebase.Data;

namespace SlimeMaster.Shared.Data
{
    public class UserResponse : Response
    {
        public DBUserData DBUserData;
        public DateTime LastLoginTime;
    }

    public class StageClearResponse : Response
    {
        public DBItemData ItemData;
        public DBStageData StageData;
    }

    public class RewardResponse : Response
    {
        public DBItemData DBItemData;
        public DBStageData DBStageData;
    }
}