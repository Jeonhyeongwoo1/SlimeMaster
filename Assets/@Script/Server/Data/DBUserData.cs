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
        [FirestoreProperty] public DateTime LastGetOfflineRewardTime { get; set; }
        [FirestoreProperty] public Dictionary<string, DBItemData> ItemDataDict { get; set; }
        [FirestoreProperty] public Dictionary<string, DBStageData> StageDataDict { get; set; }
        [FirestoreProperty] public List<DBEquipmentData> EquippedItemDataList { get; set; }
        [FirestoreProperty] public List<DBEquipmentData> UnEquippedItemDataList { get; set; }
        [FirestoreProperty] public DBMissionContainerData MissionContainerData { get; set; }
        [FirestoreProperty] public DBAchievementContainerData AchievementContainerData { get; set; }
        
        public DBUserData()
        {
            ItemDataDict = new();
            StageDataDict = new Dictionary<string, DBStageData>();
            MissionContainerData = new DBMissionContainerData();
        }
    }

    [FirestoreData]
    public class DBStageData
    {
        [FirestoreProperty] public int StageIndex { get; set; }
        [FirestoreProperty] public bool IsOpened { get; set; }
        [FirestoreProperty] public List<DBWaveData> WaveDataList { get; set; }

        public void Initialize(int stageIndex, params DBWaveData[] dbWaveDataArray)
        {
            StageIndex = stageIndex;
            WaveDataList = new();
            IsOpened = stageIndex == 1;
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

    [FirestoreData]
    public class DBEquipmentData
    {
        [FirestoreProperty] public string DataId { get; set; }
        [FirestoreProperty] public string UID { get; set; }
        [FirestoreProperty] public int Level { get; set; }
        [FirestoreProperty] public int EquipmentType { get; set; }
    }
}