using Cysharp.Threading.Tasks;
using SlimeMaster.Firebase;
using SlimeMaster.Interface;
using SlimeMaster.Managers;
using SlimeMaster.Shared.Data;

namespace SlimeMaster.Server
{
    public class ServerAchievementRequestHandler : ServerRequestHandler, IAchievementClientSender
    {
        public ServerAchievementRequestHandler(FirebaseController firebaseController, DataManager dataManager) : base(firebaseController, dataManager)
        {
        }

        public async UniTask<AchievementResponseBase> GetAchievementRewardRequest(AchievementRequestBase request)
        {
            request.userId = _firebaseController.UserId;
            var response = await Manager.I.Web.SendRequest<AchievementResponseBase>($"/api/achievement/{nameof(GetAchievementRewardRequest)}", request);
            return response;
        }
    }
}