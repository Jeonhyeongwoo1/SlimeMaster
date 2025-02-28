using SlimeMaster.Firebase.Data;

namespace SlimeMaster.Shared.Data
{
    public class AchievementResponseBase : ResponseBase
    {
        public DBItemData RewardItemData;
        public DBAchievementContainerData DBAchievementContainerData;
    }

    public class AchievementRequestBase : RequestBase
    {
        public int achievementId;
    }
}