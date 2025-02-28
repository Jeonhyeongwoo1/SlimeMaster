using Cysharp.Threading.Tasks;
using SlimeMaster.Firebase;
using SlimeMaster.Interface;
using SlimeMaster.Managers;
using SlimeMaster.Shared.Data;

namespace SlimeMaster.Server
{
    public class ServerCheckoutRequestHandler: ServerRequestHandler, ICheckoutClientSender
    {
        public ServerCheckoutRequestHandler(FirebaseController firebaseController, DataManager dataManager) : base(firebaseController, dataManager)
        {
        }
        
        public async UniTask<GetCheckoutRewardResponseBase> GetCheckoutRewardRequest(GetCheckoutRewardRequestBase request)
        {
            request.userId = _firebaseController.UserId;
            var response = await Manager.I.Web.SendRequest<GetCheckoutRewardResponseBase>($"/api/checkout/{nameof(GetCheckoutRewardRequest)}", request);
            return response;
        }
    }
}