using Cysharp.Threading.Tasks;
using SlimeMaster.Attribute;
using SlimeMaster.Enum;
using SlimeMaster.InGame.Enum;
using SlimeMaster.Shared;
using SlimeMaster.Shared.Data;

namespace SlimeMaster.Interface
{
    [ClientSender]
    public interface IUserClientSender : IClientSender
    {
        UniTask<UserResponseBase> LoadUserDataRequest(UserRequest request);
        UniTask<UserResponseBase> UseStaminaRequest(UseStaminaRequest request);
        UniTask<StageClearResponseBase> StageClearRequest(StageClearRequest request);
        UniTask<RewardResponseBase> GetWaveClearRewardRequest(GetWaveClearRewardRequest request);
        UniTask<ResponseBase> CopyNewUser(RequestBase requestBase);
    }
}