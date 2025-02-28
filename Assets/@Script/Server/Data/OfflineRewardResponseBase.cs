using System;
using System.Collections.Generic;
using SlimeMaster.Shared;

namespace SlimeMaster.Firebase.Data
{
    public class OfflineRewardResponseBase : ResponseBase
    {
        public DBItemData DBRewardItemData;
        public DateTime LastGetOfflineRewardTime;
    }
    
    public class FastRewardResponseBase : ResponseBase
    {
        public List<DBItemData> DBItemDataList;
        public DBEquipmentData DBEquipmentData;
    }
}