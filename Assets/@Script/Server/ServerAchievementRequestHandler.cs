using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Firebase;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Interface;
using SlimeMaster.Key;
using SlimeMaster.Managers;
using SlimeMaster.Shared.Data;
using UnityEngine;

namespace SlimeMaster.Server
{
    public class ServerAchievementRequestHandler : ServerRequestHandler, IAchievementClientSender
    {
        public ServerAchievementRequestHandler(FirebaseController firebaseController, DataManager dataManager) : base(firebaseController, dataManager)
        {
        }

        public async UniTask<AchievementResponse> GetAchievementRewardRequest(int achievementId)
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

                return new AchievementResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }

            if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData dbUserData))
            {
                return new AchievementResponse()
                {
                    responseCode = ServerErrorCode.FailedGetUserData
                };
            }

            if (dbUserData.AchievementContainerData.DBRewardAchievementDataList != null)
            {
                DBAchievementData dbRewardedAchievementData =
                    dbUserData.AchievementContainerData.DBRewardAchievementDataList.Find(v =>
                        v.AchievementId == achievementId);
                if (dbRewardedAchievementData != null)
                {
                    return new AchievementResponse()
                    {
                        responseCode = ServerErrorCode.AlreadyClaimed
                    };
                }
            }

            DBAchievementData dbAchievementData =
                dbUserData.AchievementContainerData.DBAchievementDataList.Find(v => v.AchievementId == achievementId);
            AchievementData achievementData = _dataManager.AchievementDataDict[achievementId];
            if (achievementData.MissionTargetValue > dbAchievementData.AccumulatedValue)
            {
                return new AchievementResponse()
                {
                    responseCode = ServerErrorCode.NotEnoughAccumulatedValue
                };
            }

            dbUserData.AchievementContainerData.DBRewardAchievementDataList ??= new List<DBAchievementData>();
            dbUserData.AchievementContainerData.DBRewardAchievementDataList.Add(new DBAchievementData()
            {
                AchievementId = dbAchievementData.AchievementId,
                AccumulatedValue = dbAchievementData.AccumulatedValue,
                MissionTarget = dbAchievementData.MissionTarget
            });
            
            List<AchievementData> achievementDataList = _dataManager.AchievementDataDict.Values
                .Where(x => x.MissionTarget == achievementData.MissionTarget).ToList();
            AchievementData nextAchievementData =
                achievementDataList.Find(v => v.AchievementID == dbAchievementData.AchievementId + 1);
            if (nextAchievementData != null)
            {
                dbAchievementData.AchievementId = nextAchievementData.AchievementID;
            }

            if (!dbUserData.ItemDataDict.TryGetValue(achievementData.ClearRewardItmeId.ToString(),
                    out DBItemData dbItemData))
            {
                dbItemData = new DBItemData();
                dbItemData.ItemId = achievementData.ClearRewardItmeId;
            }

            dbItemData.ItemValue += achievementData.RewardValue;
            userDict.Add(nameof(DBUserData), dbUserData);
            
            try
            {
                await docRef.SetAsync(userDict, SetOptions.MergeAll);
            }
            catch (Exception e)
            {
                Debug.LogError("failed firebase set " + e.Message);
                return new AchievementResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }

            return new AchievementResponse()
            {
                RewardItemData = dbItemData,
                DBAchievementContainerData = dbUserData.AchievementContainerData
            };
        }
    }
}