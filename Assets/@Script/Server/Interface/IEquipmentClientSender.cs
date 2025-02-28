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
        UniTask<EquipmentLevelUpResponseBase> EquipmentLevelUpRequest(EquipmentLevelUpRequestBase request);
        UniTask<UnequipResponseBase> UnequipRequest(UnequipRequestBase request);
        UniTask<EquipResponseBase> EquipRequest(EquipRequestBase request);
        UniTask<MergeEquipmentResponseBase> MergeEquipmentRequest(MergeEquipmentRequestBase request);
    }
}