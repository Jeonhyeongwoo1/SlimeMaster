using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SlimeMaster.Attribute;
using SlimeMaster.Enum;
using SlimeMaster.Shared.Data;

namespace SlimeMaster.Interface
{
    [ClientSender]
    public interface IEquipmentClientSender : IClientSender
    {
        UniTask<EquipmentLevelUpResponse> EquipmentLevelUpRequest(string equipmentDataId, string equipmentUID, int level, bool isEquipped);
        UniTask<UnequipResponse> UnequipRequest(string equipmentUID);
        UniTask<EquipResponse> EquipRequest(string unequippedItemUID, string equippedItemUID);
        UniTask<MergeEquipmentResponse> MergeEquipmentRequest(List<AllMergeEquipmentRequestData> requestDataList);
    }
}