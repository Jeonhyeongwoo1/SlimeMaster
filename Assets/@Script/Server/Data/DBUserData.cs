using System;
using System.Collections.Generic;
using Firebase.Firestore;

namespace SlimeMaster.Firebase.Data
{
    [FirestoreData]
    public class DBUserData
    {
        [FirestoreProperty] public string UserId { get; set; }
        [FirestoreProperty] public DateTime LastLoginTime { get; set; }
        [FirestoreProperty] public Dictionary<string, DBItemData> ItemDataDict { get; set; }
        [FirestoreProperty] public Dictionary<string, DBStageData> StageDataDict { get; set; }

        public DBUserData()
        {
            ItemDataDict = new();
            StageDataDict = new Dictionary<string, DBStageData>();
            LastLoginTime = DateTime.UtcNow;
        }
    }

    [FirestoreData]
    public class DBStageData
    {
        [FirestoreProperty] public int StageIndex { get; set; }
        [FirestoreProperty] public List<DBWaveData> WaveDataList { get; set; }

        public void Initialize(int stageIndex, params DBWaveData[] dbWaveDataArray)
        {
            StageIndex = stageIndex;
            WaveDataList = new();
            foreach (DBWaveData dbWaveData in dbWaveDataArray)
            {
                WaveDataList.Add(dbWaveData);
            }
        }
    }

    [FirestoreData]
    public class DBWaveData
    {
        [FirestoreProperty] public int WaveIndex { get; set; }
        [FirestoreProperty] public bool IsGet { get; set; }
        [FirestoreProperty] public bool IsClear { get; set; }

        public void Initialize(int waveIndex)
        {
            WaveIndex = waveIndex;
            IsGet = false;
            IsClear = false;
        }
    }

    [FirestoreData]
    public class DBItemData
    {
        [FirestoreProperty] public int ItemId { get; set; }
        [FirestoreProperty] public int ItemValue { get; set; }
    }
}