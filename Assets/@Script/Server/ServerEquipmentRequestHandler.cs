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

namespace SlimeMaster.Server
{
    public class ServerEquipmentRequestHandler : ServerRequestHandler, IEquipmentClientSender
    {
        public ServerEquipmentRequestHandler(FirebaseController firebaseController, DataManager dataManager) : base(firebaseController, dataManager)
        {
        }

        public async UniTask<EquipmentLevelUpResponse> EquipmentLevelUpRequest(string equipmentDataId,
            string equipmentUID, int level, bool isEquipped)
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

                return new EquipmentLevelUpResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }

            if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData userData))
            {
                return new EquipmentLevelUpResponse()
                {
                    responseCode = ServerErrorCode.FailedGetUserData
                };
            }

            EquipmentData equipmentData = _dataManager.EquipmentDataDict[equipmentDataId];
            EquipmentLevelData equipmentLevelData = _dataManager.EquipmentLevelDataDict[level];
            if (!userData.ItemDataDict.TryGetValue(Const.ID_GOLD.ToString(), out DBItemData dbGoldItemData))
            {
                return new EquipmentLevelUpResponse()
                {
                    responseCode = ServerErrorCode.FailedGetUserData
                };
            }

            if (!userData.ItemDataDict.TryGetValue(equipmentData.LevelupMaterialID.ToString(),
                    out DBItemData dbMaterialItemData))
            {
                return new EquipmentLevelUpResponse()
                {
                    responseCode = ServerErrorCode.NotEnoughMaterialAmount
                };
            }

            DBEquipmentData dbEquipmentData = isEquipped
                ? userData.EquippedItemDataList.Find(v => v.UID == equipmentUID)
                : userData.UnEquippedItemDataList.Find(v => v.UID == equipmentUID);

            if (dbEquipmentData == null)
            {
                return new EquipmentLevelUpResponse()
                {
                    responseCode = ServerErrorCode.FailedGetEquipment
                };
            }

            if (dbGoldItemData.ItemValue < equipmentLevelData.UpgradeCost)
            {
                return new EquipmentLevelUpResponse()
                {
                    responseCode = ServerErrorCode.NotEnoughGold
                };
            }

            if (dbMaterialItemData.ItemValue < equipmentLevelData.UpgradeRequiredItems)
            {
                return new EquipmentLevelUpResponse()
                {
                    responseCode = ServerErrorCode.NotEnoughMaterialAmount
                };
            }

            dbEquipmentData.Level++;
            dbGoldItemData.ItemValue -= equipmentLevelData.UpgradeCost;
            dbMaterialItemData.ItemValue -= equipmentLevelData.UpgradeRequiredItems;
            userDict.Add(nameof(DBUserData), userData);

            try
            {
                await docRef.SetAsync(userDict, SetOptions.MergeAll);
            }
            catch (Exception e)
            {
                Debug.LogError("failed firebase set " + e.Message);
                return new EquipmentLevelUpResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }

            return new EquipmentLevelUpResponse()
            {
                DBUserData = userData,
                responseCode = ServerErrorCode.Success,
            };
        }

        public async UniTask<UnequipResponse> UnequipRequest(string equipmentId)
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

                return new UnequipResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }
            
            if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData dbUserData))
            {
                return new UnequipResponse()
                {
                    responseCode = ServerErrorCode.FailedGetUserData
                };
            }

            DBEquipmentData dbEquipmentData = dbUserData.EquippedItemDataList.Find(v => v.UID == equipmentId);
            if (dbEquipmentData == null)
            {
                return new UnequipResponse()
                {
                    responseCode = ServerErrorCode.FailedGetEquipment
                };
            }

            dbUserData.EquippedItemDataList.Remove(dbEquipmentData);
            dbUserData.UnEquippedItemDataList ??= new List<DBEquipmentData>();
            dbUserData.UnEquippedItemDataList.Add(dbEquipmentData);
            userDict.Add(nameof(DBUserData), dbUserData);
            
            try
            {
                await docRef.SetAsync(userDict, SetOptions.MergeAll);
            }
            catch (Exception e)
            {
                Debug.LogError("failed firebase set " + e.Message);
                return new UnequipResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }

            return new UnequipResponse()
            {
                responseCode = ServerErrorCode.Success,
                EquipmentDataList = dbUserData.EquippedItemDataList,
                UnEquipmentDataList = dbUserData.UnEquippedItemDataList
            };
        }

        public async UniTask<EquipResponse> EquipRequest(string unequippedItemUID, string equippedItemUID)
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

                return new EquipResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }
            
            if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData dbUserData))
            {
                return new EquipResponse()
                {
                    responseCode = ServerErrorCode.FailedGetUserData
                };
            }

            DBEquipmentData unEquipmentData = dbUserData.UnEquippedItemDataList.Find(v => v.UID == unequippedItemUID);
            if (unEquipmentData == null)
            {
                return new EquipResponse()
                {
                    responseCode = ServerErrorCode.FailedGetEquipment
                };
            }
            
            if (!string.IsNullOrEmpty(equippedItemUID))
            {
                DBEquipmentData equippedItemData =
                    dbUserData.EquippedItemDataList.Find(v => v.UID == equippedItemUID);
                if (equippedItemData != null)
                {
                    dbUserData.EquippedItemDataList.Remove(equippedItemData);
                    dbUserData.UnEquippedItemDataList.Add(equippedItemData);
                }
            }

            dbUserData.UnEquippedItemDataList.Remove(unEquipmentData);
            dbUserData.EquippedItemDataList.Add(unEquipmentData);
            userDict.Add(nameof(DBUserData), dbUserData);
            try
            {
                await docRef.SetAsync(userDict, SetOptions.MergeAll);
            }
            catch (Exception e)
            {
                Debug.LogError("failed firebase set " + e.Message);
                return new EquipResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }

            return new EquipResponse()
            {
                responseCode = ServerErrorCode.Success,
                EquipmentDataList = dbUserData.EquippedItemDataList,
                UnEquipmentDataList = dbUserData.UnEquippedItemDataList
            };
        }

        public async UniTask<MergeEquipmentResponse> MergeEquipmentRequest(List<AllMergeEquipmentRequestData> requestDataList)
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

                return new MergeEquipmentResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }
            
            if (!snapshot.TryGetValue(nameof(DBUserData), out DBUserData dbUserData))
            {
                return new MergeEquipmentResponse()
                {
                    responseCode = ServerErrorCode.FailedGetUserData
                };
            }

            List<string> uidList = new List<string>();
            foreach (AllMergeEquipmentRequestData requestData in requestDataList)
            {
                string selectedEquipItemUid = requestData.selectedEquipItemUid;
                string firstCostItemUID = requestData.firstCostItemUID;
                string secondCostItemUID = requestData.secondCostItemUID;
                
                DBEquipmentData selectedDBEquipmentData =
                    dbUserData.UnEquippedItemDataList.Find(v => v.UID == selectedEquipItemUid);
                if (selectedDBEquipmentData == null)
                {
                    Debug.LogError("selectedDBEquipmentData is null" + selectedEquipItemUid);
                    return new MergeEquipmentResponse()
                    {
                        responseCode = ServerErrorCode.FailedGetEquipment
                    };
                }

                DBEquipmentData firstDBEquipmentData = null;
                if (!string.IsNullOrEmpty(firstCostItemUID))
                {
                    firstDBEquipmentData =
                        dbUserData.UnEquippedItemDataList.Find(v => v.UID == firstCostItemUID);
                    if (firstDBEquipmentData == null)
                    {
                        Debug.LogError("firstDBEquipmentData is null");
                        return new MergeEquipmentResponse()
                        {
                            responseCode = ServerErrorCode.FailedGetEquipment
                        };
                    }
                }

                DBEquipmentData secondDBEquipmentData = null;
                if (!string.IsNullOrEmpty(secondCostItemUID))
                {
                    secondDBEquipmentData =
                        dbUserData.UnEquippedItemDataList.Find(v => v.UID == secondCostItemUID);
                    if (secondDBEquipmentData == null)
                    {
                        Debug.LogError("secondDBEquipmentData is null");
                        return new MergeEquipmentResponse()
                        {
                            responseCode = ServerErrorCode.FailedGetEquipment
                        };
                    }
                }
                
                string id = selectedDBEquipmentData.DataId;
                EquipmentData equipmentData = _dataManager.EquipmentDataDict[id];
                string mergeItemCode = equipmentData.MergedItemCode;

                if (!_dataManager.EquipmentDataDict.TryGetValue(mergeItemCode, out EquipmentData upgradedEquipmentData))
                {
                    Debug.LogError("upgradedEquipmentData is null");
                    return new MergeEquipmentResponse()
                    {
                        responseCode = ServerErrorCode.FailedGetEquipment
                    };
                }

                dbUserData.UnEquippedItemDataList.Remove(selectedDBEquipmentData);
                if (firstDBEquipmentData != null)
                {
                    dbUserData.UnEquippedItemDataList.Remove(firstDBEquipmentData);
                }

                if (secondDBEquipmentData != null)
                {
                    dbUserData.UnEquippedItemDataList.Remove(selectedDBEquipmentData);
                }

                var newData = new DBEquipmentData
                {
                    DataId = upgradedEquipmentData.DataId,
                    Level = 1,
                    EquipmentType = (int) upgradedEquipmentData.EquipmentType,
                    UID = selectedEquipItemUid
                };
            
                Debug.Log($"newData : {newData.DataId} " );
                dbUserData.UnEquippedItemDataList.Add(newData);
                uidList.Add(selectedEquipItemUid);
            }
            
            userDict.Add(nameof(DBUserData), dbUserData);
             
            try
            {
                await docRef.SetAsync(userDict, SetOptions.MergeAll);
            }
            catch (Exception e)
            {
                Debug.LogError("failed firebase set " + e.Message);
                return new MergeEquipmentResponse()
                {
                    responseCode = ServerErrorCode.FailedFirebaseError
                };
            }

            return new MergeEquipmentResponse()
            {
                responseCode = ServerErrorCode.Success,
                UnEquipmentDataList = dbUserData.UnEquippedItemDataList,
                NewItemUIDList = uidList
            };
        }
    }
}