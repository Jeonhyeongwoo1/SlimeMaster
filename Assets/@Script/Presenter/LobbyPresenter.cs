using System;
using Script.InGame.UI;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Factory;
using SlimeMaster.Managers;
using SlimeMaster.Model;
using SlimeMaster.View;
using UniRx;
using UnityEngine;

namespace SlimeMaster.Presenter
{
    public class LobbyPresenter : BasePresenter
    {
        private UI_LobbyScene _lobbySceneView;
        private UserModel _model;
        
        public void Initialize(UserModel model, UI_LobbyScene view)
        {
            _lobbySceneView = view;
            _model = model;

            _lobbySceneView.onGoodsClickAction = OnClickGoods;
            _lobbySceneView.onClickToggleAction = OnClickToggles;
            _lobbySceneView.Initialize();
            
            Manager.I.Audio.Play(Sound.Bgm, "Bgm_Lobby");

            var goldItemData = _model.GetItemData(Const.ID_GOLD);
            goldItemData.ItemValue.Subscribe((v) => _lobbySceneView.UpdateUserGoodsInfo(GoodsType.Gold, v.ToString()))
                .AddTo(_lobbySceneView);

            var diamondItemData = _model.GetItemData(Const.ID_DIA);
            diamondItemData.ItemValue.Subscribe((v) => _lobbySceneView.UpdateUserGoodsInfo(GoodsType.Dia, v.ToString()))
                .AddTo(_lobbySceneView);

            var staminaData = _model.GetItemData(Const.ID_STAMINA);
            staminaData.ItemValue.Subscribe((v) =>_lobbySceneView.UpdateUserGoodsInfo(GoodsType.Stamina, v.ToString()))
                .AddTo(_lobbySceneView);
            
            var checkoutModel = ModelFactory.CreateOrGetModel<CheckoutModel>();
            var missionModel = ModelFactory.CreateOrGetModel<MissionModel>();
            var achievementModel = ModelFactory.CreateOrGetModel<AchievementModel>();

            checkoutModel.IsPossibleGetReward.Subscribe(x => BattleRedDotProcess()).AddTo(_lobbySceneView);
            missionModel.IsPossibleGetReward.Subscribe(x => BattleRedDotProcess()).AddTo(_lobbySceneView);
            achievementModel.IsPossibleGetReward.Subscribe(x => BattleRedDotProcess()).AddTo(_lobbySceneView);
            _model.LastOfflineGetRewardTime.Subscribe(x => BattleRedDotProcess()).AddTo(_lobbySceneView);
        }

        private void BattleRedDotProcess()
        {
            var checkoutModel = ModelFactory.CreateOrGetModel<CheckoutModel>();
            var missionModel = ModelFactory.CreateOrGetModel<MissionModel>();
            var achievementModel = ModelFactory.CreateOrGetModel<AchievementModel>();
            
            TimeSpan timeSpan = Utils.GetOfflineRewardTime(_model.LastOfflineGetRewardTime.Value);
            bool isPossibleReward = timeSpan.TotalMinutes > Const.MIN_OFFLINE_REWARD_MINUTE;
            bool isShowRedDot = checkoutModel.IsPossibleGetReward.Value || missionModel.IsPossibleGetReward.Value ||
                                achievementModel.IsPossibleGetReward.Value || isPossibleReward;
            
            _lobbySceneView.ShowRedDot(ToggleType.BattleToggle, isShowRedDot);
        }

        private void OnClickToggles(ToggleType selectedToggleType, ToggleType activatedToggleType)
        {
            UIManager uiManager = Manager.I.UI;
            uiManager.ClosePopup();
            // switch (activatedToggleType)
            // {
            //     case ToggleType.BattleToggle:
            //         break;
            //     case ToggleType.EquipmentToggle:
            //         break;
            //     case ToggleType.ShopToggle:
            //         break;
            // }
            
            Manager.I.Event.Raise(GameEventType.MoveToTap, selectedToggleType);
        }

        private void OnClickGoods(GoodsType goodsType)
        {
        }
    }
}