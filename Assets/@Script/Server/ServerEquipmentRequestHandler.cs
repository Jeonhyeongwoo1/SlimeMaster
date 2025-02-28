using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Firebase;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Interface;
using SlimeMaster.Key;
using SlimeMaster.Managers;
using SlimeMaster.Shared.Data;
using UnityEngine;

namespace SlimeMaster.Server
{
    public class ServerEquipmentRequestHandler : ServerRequestHandler, IEquipmentClientSender
    {
        public ServerEquipmentRequestHandler(FirebaseController firebaseController, DataManager dataManager) : base(firebaseController, dataManager)
        {
        }

        public async UniTask<EquipmentLevelUpResponseBase> EquipmentLevelUpRequest(EquipmentLevelUpRequestBase request)
        {
            request.userId = _firebaseController.UserId;
            var response = await Manager.I.Web.SendRequest<EquipmentLevelUpResponseBase>($"/api/equip/{nameof(EquipmentLevelUpRequest)}", request);
            return response;
        }

        public async UniTask<UnequipResponseBase> UnequipRequest(UnequipRequestBase request)
        {
            request.userId = _firebaseController.UserId;
            var response = await Manager.I.Web.SendRequest<UnequipResponseBase>($"/api/equip/{nameof(UnequipRequest)}", request);
            return response;
        }

        public async UniTask<EquipResponseBase> EquipRequest(EquipRequestBase request)
        {
            request.userId = _firebaseController.UserId;
            var response = await Manager.I.Web.SendRequest<EquipResponseBase>($"/api/equip/{nameof(EquipRequest)}", request);
            return response;
        }

        public async UniTask<MergeEquipmentResponseBase> MergeEquipmentRequest(MergeEquipmentRequestBase request)
        {
            request.userId = _firebaseController.UserId;
            var response = await Manager.I.Web.SendRequest<MergeEquipmentResponseBase>($"/api/equip/{nameof(MergeEquipmentRequest)}", request);
            return response;
        }
    }
}