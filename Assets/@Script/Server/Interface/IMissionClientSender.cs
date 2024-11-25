using Cysharp.Threading.Tasks;
using Script.Server.Data;
using SlimeMaster.Attribute;
using SlimeMaster.Enum;

namespace SlimeMaster.Interface
{
    [ClientSender]
    public interface IMissionClientSender : IClientSender
    {
        UniTask<GetMissionRewardResponse> GetMissionRewardRequest(int missionId, MissionType missionType);
    }
}