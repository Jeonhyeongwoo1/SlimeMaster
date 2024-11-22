using Cysharp.Threading.Tasks;
using SlimeMaster.Attribute;
using SlimeMaster.Shared.Data;

namespace SlimeMaster.Interface
{
    [ClientSender]
    public interface ICheckoutClientSender : IClientSender
    {
        public UniTask<GetCheckoutRewardResponse> GetCheckoutRewardRequest(int day);
    }
}