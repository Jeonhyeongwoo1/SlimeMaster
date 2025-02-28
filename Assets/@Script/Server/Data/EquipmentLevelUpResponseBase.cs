using System.Collections.Generic;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Shared;

namespace SlimeMaster.Shared.Data
{
    public class EquipmentLevelUpResponseBase : ResponseBase
    {
        public DBUserData DBUserData;
    }
    
    public class EquipmentLevelUpRequestBase : RequestBase
    {
        public string equipmentDataId;
        public string equipmentUID;
        public int level;
        public bool isEquipped;
    }

    public class UnequipResponseBase : ResponseBase
    {
        public List<DBEquipmentData> EquipmentDataList;
        public List<DBEquipmentData> UnEquipmentDataList;
    }
    
    public class UnequipRequestBase : RequestBase
    {
        public string equipmentUID;
    }

    public class EquipResponseBase : ResponseBase
    {
        public List<DBEquipmentData> EquipmentDataList;
        public List<DBEquipmentData> UnEquipmentDataList;
    }

    public class EquipRequestBase : RequestBase
    {
        public string unequippedItemUID;
        public string equippedItemUID;
    }

    public class MergeEquipmentResponseBase : ResponseBase
    {
        public List<DBEquipmentData> UnEquipmentDataList;
        public List<string> NewItemUIDList;
    }
    
    public class MergeEquipmentRequestBase : RequestBase
    {
        public List<AllMergeEquipmentRequestData> equipmentList;
    }
}