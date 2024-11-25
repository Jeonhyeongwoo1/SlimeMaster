using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using Script.Server.Data;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Firebase;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Interface;
using SlimeMaster.Key;
using SlimeMaster.Manager;
using UnityEngine;
using Random = System.Random;

namespace SlimeMaster.Server
{
    public class ServerOfflineRewardRequestHandler: ServerRequestHandler, IOfflineRewardClientSender
    {
        public ServerOfflineRewardRequestHandler(FirebaseController firebaseController, DataManager dataManager) : base(firebaseController, dataManager)
        {
        }

        public async UniTask<OfflineRewardResponse> GetOfflineRewardRequest()
        {
            string userID = _firebaseController.UserId;
            FirebaseFirestore db = _firebaseController.DB;
            Dictionary<string, object> userDict = new Dictionary<string, object>();
            DocumentReference docRef = db.Collection(DBKey.UserDB).Document(userID);
            DocumentSnapshot snapshot = null;

            try
            {
                snapshot = await docRef.GetSnapshotAsync();
            }
            catch (Exception e)
            {
                Debug.LogError("failed get data :" + e);

                return new OfflineRewardResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }

            if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData dbUserData))
            {
                return new OfflineRewardResponse()
                {
                    responseCode = ServerErrorCode.FailedGetUserData
                };
            }

            TimeSpan rewardTime = Utils.GetOfflineRewardTime(dbUserData.LastGetOfflineRewardTime);
            Debug.Log($"{dbUserData.LastGetOfflineRewardTime} / {rewardTime.Minutes}");
            if (rewardTime.Minutes < Const.MIN_OFFLINE_REWARD_MINUTE)
            {
                return new OfflineRewardResponse()
                {
                    responseCode = ServerErrorCode.NotEnoughRewardTime
                };
            }

            int lastClearStageIndex = GetLastStageIndex(dbUserData);
            int rewardGold = _dataManager.OfflineRewardDataDict[lastClearStageIndex].Reward_Gold;            
            int gold = CalculateRewardGold(rewardGold, rewardTime);
            if(!dbUserData.ItemDataDict.TryGetValue(Const.ID_GOLD.ToString(), out DBItemData dbItemData))
            {
                dbItemData = new DBItemData();
                dbItemData.ItemId = Const.ID_GOLD;
            }

            dbUserData.LastGetOfflineRewardTime = DateTime.UtcNow;
            dbItemData.ItemValue += gold;
            userDict.Add(nameof(DBUserData), dbUserData);
            
            try
            {
                await docRef.SetAsync(userDict, SetOptions.MergeAll);
            }
            catch (Exception e)
            {
                Debug.LogError("failed firebase set " + e.Message);
                return new OfflineRewardResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }
            
            Debug.Log($"{dbItemData.ItemId} / {dbItemData.ItemValue}");
            return new OfflineRewardResponse()
            {
                DBRewardItemData = dbItemData,
                LastGetOfflineRewardTime = dbUserData.LastGetOfflineRewardTime
            };
        }

        public async UniTask<FastRewardResponse> GetFastRewardRequest()
        {
            string userID = _firebaseController.UserId;
            FirebaseFirestore db = _firebaseController.DB;
            Dictionary<string, object> userDict = new Dictionary<string, object>();
            DocumentReference docRef = db.Collection(DBKey.UserDB).Document(userID);
            DocumentSnapshot snapshot = null;

            try
            {
                snapshot = await docRef.GetSnapshotAsync();
            }
            catch (Exception e)
            {
                Debug.LogError("failed get data :" + e);

                return new FastRewardResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }

            if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData dbUserData))
            {
                return new FastRewardResponse()
                {
                    responseCode = ServerErrorCode.FailedGetUserData
                };
            }

            var rewardItemDataList = new List<DBItemData>();
            
            int lastClearStageIndex = GetLastStageIndex(dbUserData);
            OfflineRewardData offlineRewardData = _dataManager.OfflineRewardDataDict[lastClearStageIndex];
            int rewardGold = offlineRewardData.Reward_Gold * Const.FAST_REWARD_GOLD_MULITPILIER;
            if (!dbUserData.ItemDataDict.TryGetValue(Const.ID_GOLD.ToString(), out DBItemData dbGoldItemData))
            {
                dbGoldItemData = new DBItemData();
                dbGoldItemData.ItemId = Const.ID_GOLD;
            }

            dbGoldItemData.ItemValue += rewardGold;
            rewardItemDataList.Add(dbGoldItemData);
            
            Random random = new Random();
            int scrollIndex = random.Next((int)MaterialType.WeaponScroll, (int)MaterialType.BootsScroll);
            if (!dbUserData.ItemDataDict.TryGetValue(scrollIndex.ToString(), out DBItemData dbScrollItemData))
            {
                dbScrollItemData = new DBItemData();
                dbScrollItemData.ItemId = scrollIndex;
            }

            dbScrollItemData.ItemValue += scrollIndex;
            rewardItemDataList.Add(dbScrollItemData);
            
            EquipmentGrade equipmentGrade = Utils.GetRandomEquipmentGrade(Const.COMMON_GACHA_GRADE_PROB);
            var equipmentDataList = _dataManager.EquipmentDataDict.Values.Where(x => x.EquipmentGrade == equipmentGrade)
                .ToList();
            EquipmentData equipmentData = equipmentDataList[random.Next(0, equipmentDataList.Count)];
            DBEquipmentData rewardEquipmentData = new DBEquipmentData
            {
                DataId = equipmentData.DataId,
                EquipmentType = (int)equipmentData.EquipmentType,
                Level = 1,
                UID = Guid.NewGuid().ToString()
            };

            dbUserData.UnEquippedItemDataList ??= new();
            dbUserData.UnEquippedItemDataList.Add(rewardEquipmentData);
            userDict.Add(nameof(DBUserData), dbUserData);
            
            try
            {
                await docRef.SetAsync(userDict, SetOptions.MergeAll);
            }
            catch (Exception e)
            {
                Debug.LogError("failed firebase set " + e.Message);
                return new FastRewardResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }

            return new FastRewardResponse()
            {
                responseCode = ServerErrorCode.Success,
                DBEquipmentData = rewardEquipmentData,
                DBItemDataList = rewardItemDataList,
            };
        }

        private int CalculateRewardGold(int gold, TimeSpan lastOfflineGetRewardTime)
        {
            double minute = lastOfflineGetRewardTime.Minutes;
            return (int)(gold / 60f * minute);
        }
        
        private int GetLastStageIndex(DBUserData dbUserData)
        {
            foreach (var (key, dbStageData) in dbUserData.StageDataDict)
            {
                bool? isClear = dbStageData.WaveDataList.LastOrDefault()?.IsClear;
                if (isClear.HasValue)
                {
                    return dbStageData.StageIndex;
                }
            }

            return 1;
        }
    }
}