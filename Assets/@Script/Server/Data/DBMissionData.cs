using System.Collections.Generic;
using Firebase.Firestore;

namespace SlimeMaster.Firebase.Data
{
    [FirestoreData]
    public class DBMissionContainerData
    {   
        [FirestoreProperty] public List<DBMissionData> DBDailyMissionDataList { get; set; }
        [FirestoreProperty] public int TotalDailyMissionClearCount { get; set; }
    }

    [FirestoreData]
    public class DBMissionData
    {
        [FirestoreProperty] public int MissionId { get; set; }
        [FirestoreProperty] public int MissionType { get; set; }
        [FirestoreProperty] public int MissionTarget { get; set; }
        [FirestoreProperty] public int AccumulatedValue { get; set; }
        [FirestoreProperty] public bool IsGet { get; set; }
    }
}