using Cysharp.Threading.Tasks;
using SlimeMaster.Attribute;
using SlimeMaster.Firebase.Data;

namespace SlimeMaster.Interface
{
    [ClientSender]
    public interface IOfflineRewardClientSender : IClientSender
    {
        UniTask<OfflineRewardResponseBase> GetOfflineRewardRequest();
        UniTask<FastRewardResponseBase> GetFastRewardRequest();
    }
}