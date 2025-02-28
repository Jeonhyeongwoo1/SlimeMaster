using SlimeMaster.Firebase.Data;

namespace SlimeMaster.Shared.Data
{
    public class GetCheckoutRewardResponseBase : ResponseBase
    {
        public DBCheckoutData DBCheckoutData;
        public DBItemData DBItemData;
        public DBEquipmentData DBEquipmentData;
    }

    public class GetCheckoutRewardRequestBase : RequestBase
    {
        public int day;
    }
}