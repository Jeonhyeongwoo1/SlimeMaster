using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Firebase;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Interface;
using SlimeMaster.Key;
using SlimeMaster.Managers;
using SlimeMaster.Shared;
using SlimeMaster.Shared.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SlimeMaster.Server
{
    public class ServerUserRequestHandler : ServerRequestHandler, IUserClientSender
    {
        public ServerUserRequestHandler(FirebaseController firebaseController, DataManager dataManager) : base(
            firebaseController, dataManager)
        {
        }

        public async UniTask<UserResponseBase> LoadUserDataRequest(UserRequest request)
        {
            request.userId = _firebaseController.UserId;
            var response = await Manager.I.Web.SendRequest<UserResponseBase>($"/api/user/{nameof(LoadUserDataRequest)}", request);
            return response;
        }

        public async UniTask<UserResponseBase> UseStaminaRequest(UseStaminaRequest request)
        {
            request.userId = _firebaseController.UserId;
            var response = await Manager.I.Web.SendRequest<UserResponseBase>($"/api/user/{nameof(UseStaminaRequest)}", request);
            return response;
        }

        public async UniTask<StageClearResponseBase> StageClearRequest(StageClearRequest request)
        {
            request.userId = _firebaseController.UserId;
            var response = await Manager.I.Web.SendRequest<StageClearResponseBase>($"/api/user/{nameof(StageClearRequest)}", request);
            return response;
        }

        public async UniTask<RewardResponseBase> GetWaveClearRewardRequest(GetWaveClearRewardRequest request)
        {
            request.userId = _firebaseController.UserId;
            var response = await Manager.I.Web.SendRequest<RewardResponseBase>($"/api/user/{nameof(GetWaveClearRewardRequest)}", request);
            return response;
        }

        public async UniTask<ResponseBase> CopyNewUser(RequestBase request)
        {
            request.userId = _firebaseController.UserId;
            var response = await Manager.I.Web.SendRequest<RewardResponseBase>($"/api/user/{nameof(CopyNewUser)}", request);
            return response;
        }
    }
}