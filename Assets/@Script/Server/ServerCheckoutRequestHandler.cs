using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Equipmenets;
using SlimeMaster.Firebase;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Interface;
using SlimeMaster.Key;
using SlimeMaster.Managers;
using SlimeMaster.Shared.Data;
using UnityEngine;
using Random = System.Random;

namespace SlimeMaster.Server
{
    public class ServerCheckoutRequestHandler: ServerRequestHandler, ICheckoutClientSender
    {
        public ServerCheckoutRequestHandler(FirebaseController firebaseController, DataManager dataManager) : base(firebaseController, dataManager)
        {
        }
        
        public async UniTask<GetCheckoutRewardResponse> GetCheckoutRewardRequest(int day)
        {
            string userID = _firebaseController.UserId;
            FirebaseFirestore db = _firebaseController.DB;
            Dictionary<string, object> userDict = new Dictionary<string, object>();
            Dictionary<string, object> checkoutDict = new Dictionary<string, object>();
            DocumentReference userDocRef = db.Collection(DBKey.UserDB).Document(userID);
            DocumentReference checkoutDocRef = db.Collection(DBKey.CheckoutDB).Document(userID);

            GetCheckoutRewardResponse getCheckoutRewardResponse = null;
            try
            {
               getCheckoutRewardResponse = await db.RunTransactionAsync(async transaction =>
                {
                    Task<DocumentSnapshot> userTask = transaction.GetSnapshotAsync(userDocRef);
                    Task<DocumentSnapshot> checkoutTask = transaction.GetSnapshotAsync(checkoutDocRef);
                    
                    await Task.WhenAll(userTask, checkoutTask);
                    
                    DocumentSnapshot userSnapshot = userTask.Result;
                    DocumentSnapshot checkoutSnapshot = checkoutTask.Result;

                    if (!checkoutSnapshot.TryGetValue(nameof(DBCheckoutData), out DBCheckoutData dbCheckoutData) ||
                        !userSnapshot.TryGetValue(nameof(DBUserData), out DBUserData dbUserData))
                    {
                        return new GetCheckoutRewardResponse()
                        {
                            responseCode = ServerErrorCode.FailedGetUserData
                        };
                    }
                    
                    if (dbCheckoutData.TotalAttendanceDays < day)
                    {
                        return new GetCheckoutRewardResponse()
                        {
                            responseCode = ServerErrorCode.NotEnoughTime
                        };
                    }

                    DBCheckoutDayData dbCheckoutDayData = dbCheckoutData.DBCheckoutDayDataList.Find(v => v.Day == day);
                    if (dbCheckoutDayData.IsGet)
                    {
                        return new GetCheckoutRewardResponse()
                        {
                            responseCode = ServerErrorCode.AlreadyClaimed
                        };
                    }
                    
                    dbCheckoutDayData.IsGet = true;

                    //AllClear
                    if (ServerCheckoutHelper.IsAllClear(dbCheckoutData))
                    {
                        dbCheckoutData.TotalAttendanceDays = 0;
                        dbCheckoutData.DBCheckoutDayDataList.ForEach(v=> v.IsGet = false);
                    }
                    
                    DBEquipmentData dbEquipmentData = null;
                    DBItemData dbItemData = null;
                    CheckOutData checkoutData = _dataManager.CheckOutDataDict[day];
                    if (checkoutData.RewardItemId >= (int)MaterialType.AllRandomEquipmentBox &&
                        checkoutData.RewardItemId <= (int)MaterialType.LegendaryEquipmentBox)
                    {
                        EquipmentData selectedEquipmentData = null;
                        MaterialType type = (MaterialType) checkoutData.RewardItemId;
                        EquipmentGrade grade = EquipmentGrade.Common;
                        Random ran = new Random();
                        switch (type)
                        {
                            case MaterialType.AllRandomEquipmentBox:
                                int random = ran.Next((int)EquipmentGrade.Common, (int)EquipmentGrade.Epic);
                                grade = (EquipmentGrade)random;
                                break;
                            case MaterialType.CommonEquipmentBox:
                                grade = EquipmentGrade.Common;
                                break;
                            case MaterialType.UncommonEquipmentBox:
                                grade = EquipmentGrade.Uncommon;
                                break;
                            case MaterialType.RareEquipmentBox:
                                grade = EquipmentGrade.Rare;
                                break;
                            case MaterialType.EpicEquipmentBox:
                                grade = EquipmentGrade.Epic;
                                break;
                            case MaterialType.LegendaryEquipmentBox:
                                grade = EquipmentGrade.Legendary;
                                break;
                        }
                        
                        List<EquipmentData> equipmentDataList = _dataManager.EquipmentDataDict.Values.Where(x =>
                            x.EquipmentGrade == grade).ToList();

                        int select = ran.Next(0, equipmentDataList.Count);
                        selectedEquipmentData = equipmentDataList[select];

                        dbEquipmentData = new DBEquipmentData()
                        {
                            DataId = selectedEquipmentData.DataId,
                            EquipmentType = (int)selectedEquipmentData.EquipmentType,
                            Level = 1,
                            UID = Guid.NewGuid().ToString()
                        };

                        dbUserData.UnEquippedItemDataList ??= new List<DBEquipmentData>();
                        dbUserData.UnEquippedItemDataList.Add(dbEquipmentData);
                    }
                    else
                    {
                        if (!dbUserData.ItemDataDict.TryGetValue(checkoutData.RewardItemId.ToString(),
                                out dbItemData))
                        {
                            dbItemData = new DBItemData();
                            dbItemData.ItemId = checkoutData.RewardItemId;
                        }

                        dbItemData.ItemValue += checkoutData.MissionTarRewardItemValuegetValue;
                    }
                    
                    
                    checkoutDict.Add(nameof(DBCheckoutData), dbCheckoutData);
                    userDict.Add(nameof(DBUserData), dbUserData);
                    transaction.Set(userDocRef, userDict, SetOptions.MergeAll);
                    transaction.Set(checkoutDocRef, checkoutDict, SetOptions.MergeAll);

                    return new GetCheckoutRewardResponse()
                    {
                        DBCheckoutData = dbCheckoutData,
                        DBItemData = dbItemData,
                        DBEquipmentData = dbEquipmentData
                    };
                });
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed {nameof(GetCheckoutRewardRequest)} messge {e.Message}");
                return new GetCheckoutRewardResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError,
                    errorMessage = e.Message
                };
            }

            return getCheckoutRewardResponse;
        }
    }
}