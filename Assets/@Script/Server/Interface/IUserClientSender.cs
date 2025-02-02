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
        UniTask<UserResponse> LoadUserDataRequest(UserRequest request);
        UniTask<UserResponse> UseStaminaRequest(int staminaCount);
        UniTask<StageClearResponse> StageClearRequest(int stageIndex);
        UniTask<RewardResponse> GetWaveClearRewardRequest(int stageIndex, WaveClearType waveClearType);
        UniTask<Response> CopyNewUser();
    }
}