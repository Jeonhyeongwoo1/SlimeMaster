using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            Dictionary<string, object> checkoutDict = new Dictionary<string, object>();
            DocumentReference userDocRef = db.Collection(DBKey.UserDB).Document(userID);
            DocumentReference checkoutDocRef = db.Collection(DBKey.CheckoutDB).Document(userID);
            DateTime lastLoginTime = DateTime.MinValue;

            UserResponse userResponse = null;
            try
            {
                userResponse = await db.RunTransactionAsync(async transaction =>
                {
                    Task<DocumentSnapshot> userTask = transaction.GetSnapshotAsync(userDocRef);
                    Task<DocumentSnapshot> checkoutTask = transaction.GetSnapshotAsync(checkoutDocRef);
                    await Task.WhenAll(userTask, checkoutTask);
                    
                    DocumentSnapshot userSnapshot = userTask.Result;
                    DocumentSnapshot checkoutSnapshot = checkoutTask.Result;

                    if (!userSnapshot.TryGetValue(nameof(DBUserData), out DBUserData userData))
                    {
                        userData = ServerUserHelper.MakeNewUser(_dataManager, userID);
                    }

                    lastLoginTime = userData.LastLoginTime;
                    userDict.Add(nameof(DBUserData), userData);
                    transaction.Set(userDocRef, userDict, SetOptions.MergeAll);
                    DBCheckoutData dbCheckoutData = null;
                    if (!checkoutSnapshot.Exists)
                    {
                        dbCheckoutData = ServerCheckoutHelper.MakeNewCheckOutData(_dataManager);
                        checkoutDict.Add(nameof(DBCheckoutData), dbCheckoutData);
                        transaction.Set(checkoutDocRef, checkoutDict, SetOptions.MergeAll);
                    }
                    else
                    {
                        if (!checkoutSnapshot.TryGetValue(nameof(DBCheckoutData), out dbCheckoutData))
                        {
                            dbCheckoutData = ServerCheckoutHelper.MakeNewCheckOutData(_dataManager);
                            checkoutDict.Add(nameof(DBCheckoutData), dbCheckoutData);
                            transaction.Set(checkoutDocRef, checkoutDict, SetOptions.MergeAll);
                        }

                        if ((DateTime.UtcNow - lastLoginTime).TotalHours > 24)
                        {
                            dbCheckoutData.TotalAttendanceDays++;
                            transaction.Set(checkoutDocRef, checkoutDict, SetOptions.MergeAll);
                        }
                    }
                    
                    return new UserResponse()
                    {
                        DBUserData = userData,
                        DBCheckoutData = dbCheckoutData,
                        LastLoginTime = lastLoginTime,
                        responseCode = ServerErrorCode.Success,
                    };
                });
            }
            catch (Exception e)
            {
                Debug.LogError("failed get data :" + e);

                return new UserResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }

            return userResponse;
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