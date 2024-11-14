using Cysharp.Threading.Tasks;
using SlimeMaster.Shared;
using SlimeMaster.Shared.Data;

namespace SlimeMaster.Interface
{
    public interface IUserClientSender : IClientSender
    {
        UniTask<UserResponse> LoadUserDataRequest(UserRequest request);
    }
}