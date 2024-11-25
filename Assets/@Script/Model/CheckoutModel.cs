using System.Collections.Generic;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Interface;
using UniRx;
using System;
using System.Linq;

namespace SlimeMaster.Model
{
    public class CheckoutModel : IModel
    {
        public ReactiveProperty<bool> IsPossibleGetReward = new();
        public List<CheckoutDayData> checkoutDayDataList;
        public int totalAttendanceDays;
        
        public void Initialize(DBCheckoutData dbCheckoutData, int totalAttendanceDays)
        {
            this.totalAttendanceDays = totalAttendanceDays;
            SetCheckOutDataList(dbCheckoutData);
        }
        
        public void SetCheckOutDataList(DBCheckoutData dbCheckoutData)
        {
            totalAttendanceDays = dbCheckoutData.TotalAttendanceDays;
            checkoutDayDataList ??= new(dbCheckoutData.DBCheckoutDayDataList.Count);
            if (checkoutDayDataList.Count > 0)
            {
                checkoutDayDataList.Clear();
            }
            
            foreach (DBCheckoutDayData dbCheckoutDayData in dbCheckoutData.DBCheckoutDayDataList)
            {
                var dayData = new CheckoutDayData
                {
                    Day = dbCheckoutDayData.Day,
                    IsGet = dbCheckoutDayData.IsGet
                };

                checkoutDayDataList.Add(dayData);
            }
            
            foreach (var dbCheckoutDayData in dbCheckoutData.DBCheckoutDayDataList)
            {
                IsPossibleGetReward.Value = false;
                if (dbCheckoutDayData.Day <= totalAttendanceDays && !dbCheckoutDayData.IsGet)
                {
                    IsPossibleGetReward.Value = true;
                    break;
                }
            }
            
        }

        public CheckoutDayData GetCheckOutData(int day)
        {
            return checkoutDayDataList?.Find(v => v.Day == day);
        }
    }
    
    [Serializable]
    public class CheckoutDayData
    {
        public int Day;
        public bool IsGet = new();
    }
}