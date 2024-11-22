using System;
using System.Collections.Generic;
using Firebase.Firestore;

namespace SlimeMaster.Firebase.Data
{
    [FirestoreData]
    public class DBCheckoutData
    {
        [FirestoreProperty] public List<DBCheckoutDayData> DBCheckoutDayDataList { get; set; }
        [FirestoreProperty] public int TotalAttendanceDays { get; set; }
    }

    [FirestoreData]
    public class DBCheckoutDayData
    {
        [FirestoreProperty] public int Day { get; set; }
        [FirestoreProperty] public bool IsGet { get; set; }
    }
}