using System;
using System.Collections.Generic;
using SlimeMaster.Shared;

namespace SlimeMaster.Firebase.Data
{
    public class OfflineRewardResponse : Response
    {
        public DBItemData DBRewardItemData;
        public DateTime LastGetOfflineRewardTime;
    }
    
    public class FastRewardResponse : Response
    {
        public List<DBItemData> DBItemDataList;
        public DBEquipmentData DBEquipmentData;
    }
}