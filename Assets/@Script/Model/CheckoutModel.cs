using System.Collections.Generic;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Interface;
using UniRx;
using System;

namespace SlimeMaster.Model
{
    public class CheckoutModel : IModel
    {
        public List<CheckoutDayData> checkoutDayDataList;
        public int totalAttendanceDays;

        public void Initialize(DBCheckoutData dbCheckoutData, int totalAttendanceDays)
        {
            this.totalAttendanceDays = totalAttendanceDays;
            SetCheckOutDataList(dbCheckoutData);
        }
        
        public void SetCheckOutDataList(DBCheckoutData dbCheckoutData)
        {
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
                    IsGet =
                    {
                        Value = dbCheckoutDayData.IsGet
                    }
                };
                
                checkoutDayDataList.Add(dayData);
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
        public ReactiveProperty<bool> IsGet = new();
    }
}