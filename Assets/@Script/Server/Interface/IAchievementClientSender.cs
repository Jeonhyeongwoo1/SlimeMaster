using Cysharp.Threading.Tasks;
using SlimeMaster.Attribute;
using SlimeMaster.Shared.Data;

namespace SlimeMaster.Interface
{
    [ClientSender]
    public interface IAchievementClientSender : IClientSender
    {
        UniTask<AchievementResponseBase> GetAchievementRewardRequest(AchievementRequestBase request);
    }
}