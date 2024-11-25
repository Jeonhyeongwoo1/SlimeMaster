using Cysharp.Threading.Tasks;
using SlimeMaster.Attribute;
using SlimeMaster.Firebase.Data;

namespace SlimeMaster.Interface
{
    [ClientSender]
    public interface IOfflineRewardClientSender : IClientSender
    {
        UniTask<OfflineRewardResponse> GetOfflineRewardRequest();
        UniTask<FastRewardResponse> GetFastRewardRequest();
    }
}