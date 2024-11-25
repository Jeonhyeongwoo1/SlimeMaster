using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using Script.Server.Data;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Firebase;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Interface;
using SlimeMaster.Key;
using SlimeMaster.Manager;
using SlimeMaster.Shared.Data;
using UnityEngine;

namespace SlimeMaster.Server
{
    public class ServerMissionRequestHandler: ServerRequestHandler, IMissionClientSender
    {
        public ServerMissionRequestHandler(FirebaseController firebaseController, DataManager dataManager) : base(firebaseController, dataManager)
        {
        }
        
        public async UniTask<GetMissionRewardResponse> GetMissionRewardRequest(int missionId, MissionType missionType)
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

                return new GetMissionRewardResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }

            if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData dbUserData))
            {
                return new GetMissionRewardResponse()
                {
                    responseCode = ServerErrorCode.FailedGetUserData
                };
            }

            DBMissionData dbMissionData = null;
            if (missionType == MissionType.Daily)
            {
                dbMissionData =
                    dbUserData.MissionContainerData.DBDailyMissionDataList.Find(v => v.MissionId == missionId);
            }

            if (dbMissionData == null)
            {
                return new GetMissionRewardResponse()
                {
                    responseCode = ServerErrorCode.FailedGetMissionData
                };
            }

            if (dbMissionData.IsGet)
            {
                return new GetMissionRewardResponse()
                {
                    responseCode = ServerErrorCode.AlreadyClaimed
                };
            }
            
            MissionData missionData = _dataManager.MissionDataDict[missionId];
            if (dbMissionData.AccumulatedValue < missionData.MissionTargetValue)
            {
                return new GetMissionRewardResponse()
                {
                    responseCode = ServerErrorCode.NotEnoughAccumulatedValue
                };
            }

            dbMissionData.IsGet = true;
            int rewardItemId = missionData.ClearRewardItmeId;
            if (!dbUserData.ItemDataDict.TryGetValue(rewardItemId.ToString(), out DBItemData dbItemData))
            {
                dbItemData = new DBItemData();
                dbItemData.ItemId = rewardItemId;
            }

            dbItemData.ItemValue += missionData.RewardValue;
            userDict.Add(nameof(DBUserData), dbUserData);
            
            try
            {
                await docRef.SetAsync(userDict, SetOptions.MergeAll);
            }
            catch (Exception e)
            {
                Debug.LogError("failed firebase set " + e.Message);
                return new GetMissionRewardResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }
            
            return new GetMissionRewardResponse()
            {
                responseCode = ServerErrorCode.Success,
                DBMissionContainerData = dbUserData.MissionContainerData,
                DBRewardItemData = dbItemData
            };
        }
    }
}