using System.Collections.Generic;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Shared;

namespace SlimeMaster.Shared.Data
{
    public class ShopPurchaseResponse : Response
    {
        public List<DBItemData> CostItemList;
        public List<DBItemData> RewardItemList;
        public List<DBEquipmentData> RewardEquipmentDataList;
    }
}