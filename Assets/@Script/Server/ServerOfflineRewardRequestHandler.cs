using Cysharp.Threading.Tasks;
using SlimeMaster.Firebase;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Interface;
using SlimeMaster.Managers;
using SlimeMaster.Shared;

namespace SlimeMaster.Server
{
    public class ServerOfflineRewardRequestHandler: ServerRequestHandler, IOfflineRewardClientSender
    {
        public ServerOfflineRewardRequestHandler(FirebaseController firebaseController, DataManager dataManager) : base(firebaseController, dataManager)
        {
        }

        public async UniTask<OfflineRewardResponseBase> GetOfflineRewardRequest()
        {
            RequestBase request = new RequestBase();
            request.userId = _firebaseController.UserId;
            var response = await Manager.I.Web.SendRequest<OfflineRewardResponseBase>($"/api/offline/{nameof(GetOfflineRewardRequest)}", request);
            return response;
        }

        public async UniTask<FastRewardResponseBase> GetFastRewardRequest()
        {
            RequestBase request = new RequestBase();
            request.userId = _firebaseController.UserId;
            var response = await Manager.I.Web.SendRequest<FastRewardResponseBase>($"/api/offline/{nameof(GetFastRewardRequest)}", request);
            return response;
        }
    }
}