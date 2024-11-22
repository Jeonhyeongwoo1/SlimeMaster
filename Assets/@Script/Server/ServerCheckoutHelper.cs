using System;
using System.Collections.Generic;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Manager;

namespace SlimeMaster.Server
{
    public static class ServerCheckoutHelper
    {
        public static DBCheckoutData MakeNewCheckOutData(DataManager dataManager)
        {
            var dbCheckoutData = new DBCheckoutData();
            dbCheckoutData.DBCheckoutDayDataList ??= new List<DBCheckoutDayData>();
            dbCheckoutData.TotalAttendanceDays = 1;
            
            foreach (var (key, checkOutData) in dataManager.CheckOutDataDict)
            {
                var data = new DBCheckoutDayData
                {
                    Day = checkOutData.Day,
                    IsGet = false
                };
                
                dbCheckoutData.DBCheckoutDayDataList.Add(data);
            }

            return dbCheckoutData;
        }

        public static bool IsAllClear(DBCheckoutData dbCheckoutData)
        {
            var dayData = dbCheckoutData.DBCheckoutDayDataList.Find(v=> !v.IsGet);
            return dayData == null && dbCheckoutData.TotalAttendanceDays == 30;
        }
    }
}