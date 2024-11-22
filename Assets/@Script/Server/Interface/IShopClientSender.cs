using Cysharp.Threading.Tasks;
using SlimeMaster.Attribute;
using SlimeMaster.Shared.Data;

namespace SlimeMaster.Interface
{
    [ClientSender]
    public interface IShopClientSender : IClientSender
    {
        public UniTask<ShopPurchaseResponse> PurchaseItemRequest(int shopId);
    }
}