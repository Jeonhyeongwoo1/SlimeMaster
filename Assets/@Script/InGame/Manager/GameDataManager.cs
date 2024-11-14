using System.Collections.Generic;
using Newtonsoft.Json;
using SlimeMaster.Data;
using UnityEngine;

namespace SlimeMaster.InGame.Manager
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
        }

        TLoader LoadJson<TLoader, TKey, TValue>(string path) where TLoader : ILoader<TKey, TValue>
        {
            TextAsset textAsset = GameManager.I.Resource.Load<TextAsset>($"{path}");
            return JsonConvert.DeserializeObject<TLoader>(textAsset.text);
        }
    }
}