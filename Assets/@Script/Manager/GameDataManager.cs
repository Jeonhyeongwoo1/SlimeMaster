using System.Collections.Generic;
using Newtonsoft.Json;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using UnityEngine;

namespace SlimeMaster.Managers
{
    public class DataManager
    {
        public Dictionary<int, SkillData> SkillDict { get; private set; } = new();
        public Dictionary<int, CreatureData> CreatureDict { get; private set; } = new();
        public Dictionary<int, StageData> StageDict { get; private set; } = new();
        public Dictionary<int, LevelData> LevelDataDict { get; private set; } = new();
        public Dictionary<int, DropItemData> DropItemDict { get; private set; } = new();
        public Dictionary<int, SupportSkillData> SupportSkillDataDict { get; private set; } = new();
        public Dictionary<int, MaterialData> MaterialDataDict { get; private set; } = new();
        public Dictionary<int, DefaultUserData> DefaultUserDataDict { get; private set; } = new();
        public Dictionary<string, EquipmentData> EquipmentDataDict { get; private set; } = new();
        public Dictionary<int, EquipmentLevelData> EquipmentLevelDataDict { get; private set; } = new();
        public Dictionary<int, ShopData> ShopDataDict { get; private set; } = new();
        public Dictionary<GachaType, GachaTableData> GachaTableDataDict { get; private set; } = new();
        public Dictionary<int, CheckOutData> CheckOutDataDict { get; private set; } = new();
        public Dictionary<int, MissionData> MissionDataDict { get; private set; } = new();
        public Dictionary<int, AchievementData> AchievementDataDict { get; private set; } = new();
        public Dictionary<int, OfflineRewardData> OfflineRewardDataDict { get; private set; } = new();
        
        public void Initialize()
        {
            SkillDict = LoadJson<SkillDataLoader, int, SkillData>("SkillData").MakeDict();
            CreatureDict = LoadJson<CreatureDataLoader, int, CreatureData>("CreatureData").MakeDict();
            StageDict = LoadJson<StageDataLoader, int, StageData>("StageData").MakeDict();
            LevelDataDict = LoadJson<LevelDataLoader, int, LevelData>("LevelData").MakeDict();
            DropItemDict = LoadJson<DropItemDataLoader, int, DropItemData>("DropItemData").MakeDict();
            SupportSkillDataDict =
                LoadJson<SupportSkillDataLoader, int, SupportSkillData>("SupportSkillData").MakeDict();
            MaterialDataDict = LoadJson<MaterialDataLoader, int, MaterialData>("MaterialData").MakeDict();
            DefaultUserDataDict = LoadJson<DefaultUserDataLoader, int, DefaultUserData>("DefaultUserData").MakeDict();
            EquipmentDataDict = LoadJson<EquipmentDataLoader, string, EquipmentData>("EquipmentData").MakeDict();
            EquipmentLevelDataDict =
                LoadJson<EquipmentLevelDataLoader, int, EquipmentLevelData>("EquipmentLevelData").MakeDict();
            ShopDataDict = LoadJson<ShopDataDataLoader, int, ShopData>("ShopData").MakeDict();
            GachaTableDataDict = LoadJson<GachaDataLoader, GachaType, GachaTableData>("GachaTableData").MakeDict();
            CheckOutDataDict = LoadJson<CheckOutDataLoader, int, CheckOutData>("CheckOutData").MakeDict();
            MissionDataDict = LoadJson<MissionDataLoader, int, MissionData>("MissionData").MakeDict();
            AchievementDataDict = LoadJson<AchievementDataLoader, int, AchievementData>("AchievementData").MakeDict();
            OfflineRewardDataDict = LoadJson<OfflineRewardDataLoader, int, OfflineRewardData>("OfflineRewardData")
                .MakeDict();
        }

        TLoader LoadJson<TLoader, TKey, TValue>(string path) where TLoader : ILoader<TKey, TValue>
        {
            TextAsset textAsset = Manager.I.Resource.Load<TextAsset>($"{path}");
            return JsonConvert.DeserializeObject<TLoader>(textAsset.text);
        }
    }
}