using System.Collections.Generic;
using Firebase.Firestore;

namespace SlimeMaster.Firebase.Data
{
    [FirestoreData]
    public class DBAchievementContainerData
    {
        [FirestoreProperty] public List<DBAchievementData> DBAchievementDataList { get; set; }
        [FirestoreProperty] public List<DBAchievementData> DBRewardAchievementDataList { get; set; }
    }

    [FirestoreData]
    public class DBAchievementData
    {
        [FirestoreProperty] public int AchievementId { get; set; }
        [FirestoreProperty] public int MissionTarget { get; set; }
        [FirestoreProperty] public int AccumulatedValue { get; set; }
    }
}