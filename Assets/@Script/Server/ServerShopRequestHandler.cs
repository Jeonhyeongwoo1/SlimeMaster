using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Firebase;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Interface;
using SlimeMaster.Key;
using SlimeMaster.Managers;
using SlimeMaster.Shared.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SlimeMaster.Server
{
    public class ServerShopRequestHandler : ServerRequestHandler, IShopClientSender
    {
        public ServerShopRequestHandler(FirebaseController firebaseController, DataManager dataManager) : base(firebaseController, dataManager)
        {
        }

        public async UniTask<ShopPurchaseResponseBase> PurchaseItemRequest(ShopPurchaseRequestBase request)
        {
            request.userId = _firebaseController.UserId;
            var response = await Manager.I.Web.SendRequest<ShopPurchaseResponseBase>($"/api/shop/{nameof(PurchaseItemRequest)}", request);
            return response;
        }
    }
}