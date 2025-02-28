using Cysharp.Threading.Tasks;
using Script.Server.Data;
using SlimeMaster.Firebase;
using SlimeMaster.Interface;
using SlimeMaster.Managers;

namespace SlimeMaster.Server
{
    public class ServerMissionRequestHandler: ServerRequestHandler, IMissionClientSender
    {
        public ServerMissionRequestHandler(FirebaseController firebaseController, DataManager dataManager) : base(firebaseController, dataManager)
        {
        }
        
        public async UniTask<GetMissionRewardResponseBase> GetMissionRewardRequest(GetMissionRewardRequest request)
        {
            request.userId = _firebaseController.UserId;
            var response = await Manager.I.Web.SendRequest<GetMissionRewardResponseBase>($"/api/mission/{nameof(GetMissionRewardRequest)}", request);
            return response;
        }
    }
}