using System;
using System.Collections.Generic;
using Firebase.Firestore;

namespace SlimeMaster.Firebase.Data
{
    [FirestoreData]
    public class DBShopData
    {
        [FirestoreProperty] public List<DBShopHistoryData> ShopHistoryDataList { get; set; }
    }
    
    [FirestoreData]
    public class DBShopHistoryData
    {
        [FirestoreProperty] public DateTime PurchaseTime { get; set; }
        [FirestoreProperty] public int PurchaseItemId { get; set; }
        [FirestoreProperty] public int CostItemType { get; set; }
        [FirestoreProperty] public float CostValue { get; set; }
    }
        
}