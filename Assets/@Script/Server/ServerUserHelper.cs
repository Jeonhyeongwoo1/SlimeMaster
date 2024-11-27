using System;
using System.Collections.Generic;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Managers;

namespace SlimeMaster.Server
{
    public static class ServerUserHelper
    {
        public static DBUserData MakeNewUser(DataManager dataManager, string userID)
        {
            var userData = new DBUserData();
            userData.UserId = userID;
            userData.LastLoginTime = DateTime.UtcNow;

            userData.ItemDataDict = new Dictionary<string, DBItemData>();
            foreach (var (key, value) in dataManager.DefaultUserDataDict)
            {
                var itemData = new DBItemData
                {
                    ItemId = value.itemId,
                    ItemValue = value.itemValue
                };

                userData.ItemDataDict.Add(key.ToString(), itemData);
            }

            userData.StageDataDict = new Dictionary<string, DBStageData>();
            foreach (var (key, value) in dataManager.StageDict)
            {
                var dbFirstWaveData = new DBWaveData();
                dbFirstWaveData.Initialize(value.FirstWaveCountValue);
                var dbSecondWaveData = new DBWaveData();
                dbSecondWaveData.Initialize(value.SecondWaveCountValue);
                var dbThirdWaveData = new DBWaveData();
                dbThirdWaveData.Initialize(value.ThirdWaveCountValue);
                var dbStageData = new DBStageData();
                dbStageData.Initialize(key, dbFirstWaveData, dbSecondWaveData, dbThirdWaveData);
                userData.StageDataDict.Add(key.ToString(), dbStageData);
            }

            userData.EquippedItemDataList = new List<DBEquipmentData>
            {
                new DBEquipmentData()
                {
                    DataId = Const.DefaultWeaponId,
                    Level = 1,
                    UID = Guid.NewGuid().ToString(),
                    EquipmentType = (int)EquipmentType.Weapon
                },
                new DBEquipmentData()
                {
                    DataId = Const.DefaultArmorId,
                    Level = 1,
                    UID = Guid.NewGuid().ToString(),
                    EquipmentType = (int)EquipmentType.Armor
                },
                new DBEquipmentData()
                {
                    DataId = Const.DefaultBeltId,
                    Level = 1,
                    UID = Guid.NewGuid().ToString(),
                    EquipmentType = (int)EquipmentType.Belt
                },
                new DBEquipmentData()
                {
                    DataId = Const.DefaultBootsId,
                    Level = 1,
                    UID = Guid.NewGuid().ToString(),
                    EquipmentType = (int)EquipmentType.Boots
                },
                new DBEquipmentData()
                {
                    DataId = Const.DefaultGlovesId,
                    Level = 1,
                    UID = Guid.NewGuid().ToString(),
                    EquipmentType = (int)EquipmentType.Gloves
                },
                new DBEquipmentData()
                {
                    DataId = Const.DefaultRingId,
                    Level = 1,
                    UID = Guid.NewGuid().ToString(),
                    EquipmentType = (int)EquipmentType.Ring
                }
            };

            return userData;
        }
    }
}