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
        public Dictionary<int, WaveData> WaveDict { get; private set; } = new();

        public void Initialize()
        {
            SkillDict = LoadJson<SkillDataLoader, int, SkillData>("SkillData").MakeDict();
            CreatureDict = LoadJson<CreatureDataLoader, int, CreatureData>("CreatureData").MakeDict();
            StageDict = LoadJson<StageDataLoader, int, StageData>("StageData").MakeDict();
        }

        TLoader LoadJson<TLoader, TKey, TValue>(string path) where TLoader : ILoader<TKey, TValue>
        {
            TextAsset textAsset = GameManager.I.Resource.Load<TextAsset>($"{path}");
            return JsonConvert.DeserializeObject<TLoader>(textAsset.text);
        }
    }
}