using System;
using System.Collections.Generic;
using System.Linq;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Equipmenets;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Interface;
using SlimeMaster.Manager;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace SlimeMaster.Model
{
    public class UserModel : IModel
    {        
        public int MaxHp => (int)(CreatureData.MaxHp + (CreatureData.MaxHpBonus * _userLevel) * CreatureData.HpRate);
        public int MaxAttackDamage => (int)(CreatureData.Atk + (CreatureData.AtkBonus * _userLevel) * CreatureData.AtkRate);

        public ReactiveProperty<Dictionary<int, ItemData>> ItemDataDict = new();
        public ReactiveProperty<List<StageInfo>> StageInfoList = new();
        public ReactiveProperty<List<Equipment>> EquippedItemDataList = new();
        public ReactiveProperty<List<Equipment>> UnEquippedItemDataList = new();
        public DateTime LastLoginTime;
        public DateTime LastOfflineGetRewardTime = DateTime.UtcNow;
        public CreatureData CreatureData;
        
        private readonly int _userLevel = 1;

        public void CreateItem(int itemId, long itemValue)
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

        public void AddItemValue(int itemId, long itemValue)
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

        public ItemData SetItemValue(int itemId, long itemValue)
        {
            ItemDataDict.Value ??= new Dictionary<int, ItemData>();
            if (!ItemDataDict.Value.TryGetValue(itemId, out ItemData itemData))
            {
                itemData = new ItemData(itemId, itemValue);
                ItemDataDict.Value.Add(itemId, itemData);
                return itemData;
            }

            itemData.ItemValue.Value = itemValue;
            return itemData;
        }

        public ItemData GetItemData(int itemId)
        {
            ItemDataDict.Value ??= new Dictionary<int, ItemData>();
            if (ItemDataDict.Value.TryGetValue(itemId, out ItemData itemData))
            {
                return itemData;
            }

            Debug.LogError("Failed Get item " + itemId);
            return null;
        }

        public StageInfo GetStageInfo(int stageIndex)
        {
            StageInfo stageInfo = StageInfoList.Value.Find(v => v.StageIndex.Value == stageIndex);
            return stageInfo;
        }

        public bool IsClearStage(int stageIndex)
        {
            StageInfo stageInfo = StageInfoList.Value.Find(v => v.StageIndex.Value == stageIndex);
            WaveInfo waveInfo = stageInfo.WaveInfoList.Value.FirstOrDefault(v => !v.IsClear.Value);
            return waveInfo == null;
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

        public Equipment FindEquippedItem(EquipmentType equipmentType)
        {
            if (EquippedItemDataList.Value == null)
            {
                Debug.LogError("Failed find equipped : " + equipmentType);
                return null;
            }

            foreach (var equipment in EquippedItemDataList.Value)
            {
                if (equipment.IsEquippedByType(equipmentType))
                {
                    return equipment;
                }
            }

            Debug.LogWarning("Failed find equipped : " + equipmentType);
            return null;
        }

        public Equipment FindEquippedItemOrUnEquippedItem(string uid)
        {
            if (EquippedItemDataList.Value != null)
            {
                foreach (var equipment in EquippedItemDataList.Value)
                {
                    if (equipment.UID == uid)
                    {
                        return equipment;
                    }
                }
            }

            if (UnEquippedItemDataList.Value != null)
            {
                foreach (var equipment in UnEquippedItemDataList.Value)
                {
                    if (equipment.UID == uid)
                    {
                        return equipment;
                    }
                }
            }
            
            Debug.LogError("Failed find equipped : " + uid);
            return null;
        }
        
        public (int hp, int atk) GetEquipmentBonus()
        {
            if (EquippedItemDataList.Value == null)
            {
                Debug.LogError("Failed find equipped");
                return (0, 0);
            }

            int hp = 0;
            int atk = 0;
            foreach (var equipment in EquippedItemDataList.Value)
            {
                if (equipment.IsEquipped())
                {
                    hp += equipment.EquipmentData.MaxHpBonus;
                    atk += equipment.EquipmentData.AtkDmgBonus;
                }
            }

            return (hp, atk);
        }

        public void AddUnEquipmentDataList(List<DBEquipmentData> equipmentDataList)
        {
            UnEquippedItemDataList.Value ??= new List<Equipment>();
            foreach (DBEquipmentData equipmentData in equipmentDataList)
            {
                var data = GameManager.I.Data.EquipmentDataDict[equipmentData.DataId];
                Equipment equipment =
                    new Equipment(data, false, equipmentData.Level, equipmentData.UID);
                UnEquippedItemDataList.Value.Add(equipment);
            }
        }

        public void ClearAndSetUnEquipmentDataList(List<DBEquipmentData> equipmentDataList)
        {
            UnEquippedItemDataList.Value ??= new List<Equipment>();
            if (UnEquippedItemDataList.Value.Count > 0)
            {
                UnEquippedItemDataList.Value.Clear();
            }

            foreach (DBEquipmentData equipmentData in equipmentDataList)
            {
                var data = GameManager.I.Data.EquipmentDataDict[equipmentData.DataId];
                Equipment equipment =
                    new Equipment(data, false, equipmentData.Level, equipmentData.UID);
                UnEquippedItemDataList.Value.Add(equipment);
            }
        }
        
        public void ClearAndSetEquipmentDataList(List<DBEquipmentData> equipmentDataList)
        {
            EquippedItemDataList.Value ??= new List<Equipment>();
            if (EquippedItemDataList.Value.Count > 0)
            {
                EquippedItemDataList.Value.Clear();
            }
            
            foreach (DBEquipmentData equipmentData in equipmentDataList)
            {
                var data = GameManager.I.Data.EquipmentDataDict[equipmentData.DataId];
                Equipment equipment =
                    new Equipment(data, true, equipmentData.Level, equipmentData.UID);
                EquippedItemDataList.Value.Add(equipment);
            }
        }

        public List<Equipment> GetUnequipItemList()
        {
            UnEquippedItemDataList.Value ??= new List<Equipment>();
            return UnEquippedItemDataList.Value;
        }
    }

    [Serializable]
    public class StageInfo
    {
        public readonly ReactiveProperty<int> StageIndex = new();
        public readonly ReactiveProperty<List<WaveInfo>> WaveInfoList = new();
        public int lastClearWaveIndex = 0;
        public readonly ReactiveProperty<bool> IsOpenedStage = new();

        public StageInfo(int stageIndex, bool isOpenedStage, List<WaveInfo> waveInfoList)
        {
            StageIndex.Value = stageIndex;
            IsOpenedStage.Value = isOpenedStage;
            WaveInfoList.Value = new List<WaveInfo>();
            WaveInfoList.Value.AddRange(waveInfoList);
        }
        
        public int GetLastClearWaveIndex()
        {
            var waveInfo = WaveInfoList.Value?.FindLast(v => v.IsClear.Value);
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
        public WaveClearType WaveClearType;
        
        public WaveInfo(int waveIndex, bool isClear, bool isGet, WaveClearType waveClearType)
        {
            WaveIndex.Value = waveIndex;
            IsClear.Value = isClear;
            IsGet.Value = isGet;
            WaveClearType = waveClearType;
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

    // [Serializable]
    // public class EquipmentData
    // {
    //     public bool IsEquipped;
    //     public string DataId;
    //     
    // }
}