using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using SlimeMaster.Firebase;
using SlimeMaster.Firebase.Data;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Manager;
using SlimeMaster.Interface;
using SlimeMaster.Key;
using SlimeMaster.Shared.Data;
using UnityEngine;

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

    }
}