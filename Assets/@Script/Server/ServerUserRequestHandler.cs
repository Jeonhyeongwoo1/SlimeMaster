using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Firebase;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Interface;
using SlimeMaster.Key;
using SlimeMaster.Manager;
using SlimeMaster.Shared.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SlimeMaster.Server
{
    public class ServerUserRequestHandler : ServerRequestHandler, IUserClientSender
    {
        public ServerUserRequestHandler(FirebaseController firebaseController, DataManager dataManager) : base(
            firebaseController, dataManager)
        {
        }

        public async UniTask<UserResponse> LoadUserDataRequest(UserRequest request)
        {
            string userID = _firebaseController.UserId;
            FirebaseFirestore db = _firebaseController.DB;
            Dictionary<string, object> userDict = new Dictionary<string, object>();
            DocumentReference docRef = db.Collection(DBKey.UserDB).Document(userID);
            DocumentSnapshot snapshot = null;
            DateTime lastLoginTime = DateTime.MinValue;
            
            try
            {
                snapshot = await docRef.GetSnapshotAsync();
            }
            catch (Exception e)
            {
                Debug.LogError("failed get data :" + e);

                return new UserResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }

            if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData userData))
            {
                userData = new DBUserData();
                userData.UserId = userID;
                userData.LastLoginTime = DateTime.UtcNow;

                userData.ItemDataDict = new Dictionary<string, DBItemData>();
                foreach (var (key, value) in _dataManager.DefaultUserDataDict)
                {
                    var itemData = new DBItemData
                    {
                        ItemId = value.itemId,
                        ItemValue = value.itemValue
                    };

                    userData.ItemDataDict.Add(key.ToString(), itemData);
                }

                userData.StageDataDict = new Dictionary<string, DBStageData>();
                foreach (var (key, value) in _dataManager.StageDict)
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
            }

            lastLoginTime = userData.LastLoginTime;
            userDict.Add(nameof(DBUserData), userData);

            try
            {
                await docRef.SetAsync(userDict, SetOptions.MergeAll);
            }
            catch (Exception e)
            {
                Debug.LogError("failed firebase set " + e.Message);
                return new UserResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }

            return new UserResponse()
            {
                DBUserData = userData,
                LastLoginTime = lastLoginTime,
                responseCode = ServerErrorCode.Success,
            };
        }

        public async UniTask<UserResponse> UseStaminaRequest(int staminaCount)
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

                return new UserResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }

            if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData userData))
            {
                return new UserResponse()
                {
                    responseCode = ServerErrorCode.FailedGetUserData
                };
            }

            if (!userData.ItemDataDict.TryGetValue(Const.ID_STAMINA.ToString(), out DBItemData itemData))
            {
                return new UserResponse()
                {
                    responseCode = ServerErrorCode.FailedGetUserData
                };
            }

            if (itemData.ItemValue < staminaCount)
            {
                return new UserResponse()
                {
                    responseCode = ServerErrorCode.NotEnoughStamina
                };
            }
            
            itemData.ItemValue -= staminaCount;
            userDict.Add(nameof(DBUserData), userData);
            
            try
            {
                await docRef.SetAsync(userDict, SetOptions.MergeAll);
            }
            catch (Exception e)
            {
                Debug.LogError("failed firebase set " + e.Message);
                return new UserResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }

            return new UserResponse()
            {
                DBUserData = userData,
                responseCode = ServerErrorCode.Success,
            };
        }

        public async UniTask<StageClearResponse> StageClearRequest(int stageIndex)
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

                return new StageClearResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError,
                    errorMessage = e.Message
                };
            }

            if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData userData))
            {
                return new StageClearResponse()
                {
                    responseCode = ServerErrorCode.FailedGetUserData,
                };
            }

            if (userData.StageDataDict.TryGetValue(stageIndex.ToString(), out DBStageData dbStageData))
            {
                foreach (DBWaveData dbWaveData in dbStageData.WaveDataList)
                {
                    dbWaveData.IsClear = true;
                }
            }

            if (userData.ItemDataDict.TryGetValue(Const.ID_GOLD.ToString(), out DBItemData itemData))
            {
                var stageData = _dataManager.StageDict[stageIndex];
                var rewardGold = stageData.ClearReward_Gold;
                itemData.ItemValue += rewardGold;
            }
            
            userDict.Add(nameof(DBUserData), userData);
            
            try
            {
                await docRef.SetAsync(userDict, SetOptions.MergeAll);
            }
            catch (Exception e)
            {
                Debug.LogError("failed firebase set " + e.Message);
                return new StageClearResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError,
                    errorMessage = e.Message
                };
            }

            return new StageClearResponse()
            {
                ItemData = itemData,
                StageData = dbStageData,
                responseCode = ServerErrorCode.Success,
            };
        }

        public async UniTask<RewardResponse> GetWaveClearRewardRequest(int stageIndex, WaveClearType waveClearType)
        {
            int itemId = -1;
            int itemValue = -1;
            StageData stageData = _dataManager.StageDict[stageIndex];
            int waveIndex = 0;
            switch (waveClearType)
            {
                case WaveClearType.FirstWaveClear:
                    itemId = stageData.FirstWaveClearRewardItemId;
                    itemValue = stageData.FirstWaveClearRewardItemValue;
                    waveIndex = stageData.FirstWaveCountValue;
                    break;
                case WaveClearType.SecondWaveClear:
                    itemId = stageData.SecondWaveClearRewardItemId;
                    itemValue = stageData.SecondWaveClearRewardItemValue;
                    waveIndex = stageData.SecondWaveCountValue;
                    break;
                case WaveClearType.ThirdWaveClear:
                    itemId = stageData.ThirdWaveClearRewardItemId;
                    itemValue = stageData.ThirdWaveClearRewardItemValue;
                    waveIndex = stageData.ThirdWaveCountValue;
                    break;
            }

            if (itemId == (int)MaterialType.RandomScroll)
            {
                itemId = Random.Range((int)MaterialType.WeaponScroll, (int)MaterialType.BootsScroll);
            }
            
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
                return new RewardResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError,
                    errorMessage = e.Message
                };
            }

            if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData userData))
            {
                return new RewardResponse()
                {
                    responseCode = ServerErrorCode.FailedGetUserData,
                };
            }

            if (!userData.ItemDataDict.TryGetValue(itemId.ToString(), out DBItemData dbItemData))
            {
                dbItemData = new DBItemData();
                dbItemData.ItemId = itemId;
            }

            dbItemData.ItemValue += itemValue;
            if (!userData.StageDataDict.TryGetValue(stageIndex.ToString(), out DBStageData dbStageData))
            {
                return new RewardResponse()
                {
                    responseCode = ServerErrorCode.FailedGetStage
                };
            }

            var waveData = dbStageData.WaveDataList.Find(v => v.WaveIndex == waveIndex);
            waveData.IsGet = true;
            userDict.Add(nameof(DBUserData), userData);
            
            try
            {
                await docRef.SetAsync(userDict, SetOptions.MergeAll);
            }
            catch (Exception e)
            {
                Debug.LogError("failed firebase set " + e.Message);
                return new RewardResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError,
                    errorMessage = e.Message
                };
            }
            
            return new RewardResponse()
            {
                responseCode = ServerErrorCode.Success,
                DBItemData = dbItemData,
                DBStageData = dbStageData
            };
        }
    }
}