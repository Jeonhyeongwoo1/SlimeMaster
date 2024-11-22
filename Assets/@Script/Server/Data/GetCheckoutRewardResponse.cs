using SlimeMaster.Firebase.Data;

namespace SlimeMaster.Shared.Data
{
    public class GetCheckoutRewardResponse : Response
    {
        public DBCheckoutData DBCheckoutData;
        public DBItemData DBItemData;
        public DBEquipmentData DBEquipmentData;
    }
}