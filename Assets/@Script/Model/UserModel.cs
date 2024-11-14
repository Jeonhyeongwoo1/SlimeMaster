using System;
using System.Collections.Generic;
using System.Linq;
using SlimeMaster.Data;
using SlimeMaster.InGame.Interface;
using SlimeMaster.InGame.Manager;
using UniRx;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SlimeMaster.Factory
{
    public class UserModel : IModel
    {
        public ReactiveProperty<Dictionary<int, ItemData>> ItemDataDict = new();
        public ReactiveProperty<List<StageInfo>> StageInfoList = new();
        public DateTime LastLoginTime;

        public void AddItem(int itemId, long itemValue)
        {
            ItemDataDict.Value ??= new Dictionary<int, ItemData>();
            if (ItemDataDict.Value.ContainsKey(itemId))
            {
                Debug.Log("Already added item " + itemId);
                return;
            }
            
            ItemData itemData = new ItemData(itemId, itemValue);
            ItemDataDict.Value.Add(itemId, itemData);
        }

        public void UpdateItem(int itemId, long itemValue)
        {
            ItemDataDict.Value ??= new Dictionary<int, ItemData>();
            if (!ItemDataDict.Value.TryGetValue(itemId, out ItemData itemData))
            {
                itemData = new ItemData(itemId, itemValue);
                ItemDataDict.Value.Add(itemId, itemData);
                return;
            }
            
            itemData.ItemValue.Value += itemValue;
        }

        public ItemData GetItemData(int itemId)
        {
            ItemDataDict.Value ??= new Dictionary<int, ItemData>();
            if (!ItemDataDict.Value.TryGetValue(itemId, out ItemData itemData))
            {
                itemData = new ItemData(itemId, 0);
                ItemDataDict.Value.Add(itemId, itemData);
            }

            return itemData;
        }

        public int GetLastClearStageIndex()
        {
            foreach (StageInfo stageInfo in StageInfoList.Value)
            {
                var isClear = stageInfo.WaveInfoList.Value.LastOrDefault()?.IsClear;
                if (isClear is { Value: true })
                {
                    return stageInfo.StageIndex.Value;
                }
            }

            return 1;
        }
        
        public StageInfo GetLastStage()
        {
            if (StageInfoList.Value == null)
            {
                return null;
            }

            foreach (StageInfo stageInfo in StageInfoList.Value)
            {
                var isClear = stageInfo.WaveInfoList.Value.LastOrDefault()?.IsClear;
                if (isClear is { Value: true })
                {
                    return stageInfo;
                }
            }

            //없다면 첫번쨰 스테이지를 전달
            return StageInfoList.Value[0];
        }

        public void AddStage(StageInfo stageInfo)
        {
            StageInfoList.Value ??= new List<StageInfo>();
            if (StageInfoList.Value.Contains(stageInfo))
            {
                Debug.Log("Already added stage" + stageInfo.StageIndex);
                return;
            }
            
            StageInfoList.Value.Add(stageInfo);
        }

        public void UpdateStage(int stageIndex, int finalWaveIndex)
        {
            StageInfoList.Value ??= new List<StageInfo>();
            StageInfo stageInfo = StageInfoList.Value.Find(v => v.StageIndex.Value == stageIndex);
            if (stageInfo == null)
            {
                Debug.LogError($"Failed get stage info {stageIndex}");
                return;
            }
            
            StageData stageData = GameManager.I.Data.StageDict[stageIndex];
            if (stageData.ThirdWaveCountValue <= finalWaveIndex)
            {
                stageInfo.UpdateWaveClear(stageData.FirstWaveCountValue);
                stageInfo.UpdateWaveClear(stageData.SecondWaveCountValue);
                stageInfo.UpdateWaveClear(stageData.ThirdWaveCountValue);
                return;
            }
            
            if (stageData.SecondWaveCountValue <= finalWaveIndex)
            {
                stageInfo.UpdateWaveClear(stageData.FirstWaveCountValue);
                stageInfo.UpdateWaveClear(stageData.SecondWaveCountValue);
                return;
            }
            
            if (stageData.FirstWaveCountValue <= finalWaveIndex)
            {
                stageInfo.UpdateWaveClear(stageData.FirstWaveCountValue);
            }
        }
    }

    [Serializable]
    public class StageInfo
    {
        public readonly ReactiveProperty<int> StageIndex = new();
        public readonly ReactiveProperty<List<WaveInfo>> WaveInfoList = new();

        public StageInfo(int stageIndex, List<WaveInfo> waveInfoList)
        {
            StageIndex.Value = stageIndex;
            WaveInfoList.Value = new List<WaveInfo>();
            WaveInfoList.Value.AddRange(waveInfoList);
        }
        
        public int GetLastClearWaveIndex()
        {
            var waveInfo = WaveInfoList.Value?.FindLast(v => v.IsGet.Value);
            return waveInfo == null ? 0 : waveInfo.WaveIndex.Value;
        }

        public WaveInfo GetWaveInfo(int waveIndex)
        {
            return WaveInfoList.Value?.Find(v => v.WaveIndex.Value == waveIndex);
        }

        public void UpdateWaveClear(int waveIndex)
        {
            WaveInfoList.Value ??= new();
            WaveInfo waveInfo = WaveInfoList.Value.Find(v => v.WaveIndex.Value == waveIndex);
            waveInfo.WaveIndex.Value = waveIndex;
            waveInfo.IsClear.Value = true;
            waveInfo.IsGet.Value = true;
        }
    }

    [Serializable]
    public class WaveInfo
    {
        public ReactiveProperty<int> WaveIndex = new();
        public ReactiveProperty<bool> IsGet = new();
        public ReactiveProperty<bool> IsClear = new();
        
        public WaveInfo(int waveIndex, bool isClear, bool isGet)
        {
            WaveIndex.Value = waveIndex;
            IsClear.Value = isClear;
            IsGet.Value = isGet;
        }
    }

    [Serializable]
    public class ItemData
    {
        public ReactiveProperty<int> ItemId = new();
        public ReactiveProperty<long> ItemValue = new();
        
        public ItemData(int itemId, long itemValue)
        {
            ItemId.Value = itemId;
            ItemValue.Value = itemValue;
        }
    }
}