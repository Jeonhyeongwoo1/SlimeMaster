using System.Collections.Generic;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Shared;

namespace SlimeMaster.Shared.Data
{
    public class EquipmentLevelUpResponse : Response
    {
        public DBUserData DBUserData;
    }

    public class UnequipResponse : Response
    {
        public List<DBEquipmentData> EquipmentDataList;
        public List<DBEquipmentData> UnEquipmentDataList;
    }

    public class EquipResponse : Response
    {
        public List<DBEquipmentData> EquipmentDataList;
        public List<DBEquipmentData> UnEquipmentDataList;
    }

    public class MergeEquipmentResponse : Response
    {
        public List<DBEquipmentData> UnEquipmentDataList;
        public List<string> NewItemUIDList;
    }
}