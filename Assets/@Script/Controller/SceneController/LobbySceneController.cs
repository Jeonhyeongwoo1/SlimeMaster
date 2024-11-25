using Script.InGame.UI;
using SlimeMaster.Factory;
using SlimeMaster.Manager;
using SlimeMaster.Model;
using SlimeMaster.Presenter;
using UnityEngine;

namespace SlimeMaster.Controller
{
    public class LobbySceneController : MonoBehaviour
    {
        private void Start()
        {
            GameManager gameManager = GameManager.I;
            var lobbyUI = gameManager.UI.ShowUI<UI_LobbyScene>();

            var lobbyPresenter = PresenterFactory.CreateOrGet<LobbyPresenter>();
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            var checkoutModel = ModelFactory.CreateOrGetModel<CheckoutModel>();
            var missionModel = ModelFactory.CreateOrGetModel<MissionModel>();
            var achievementModel = ModelFactory.CreateOrGetModel<AchievementModel>();
            var timeDataModel = ModelFactory.CreateOrGetModel<TimeDataModel>();
            lobbyPresenter.Initialize(userModel, lobbyUI);

            var battlePresenter = PresenterFactory.CreateOrGet<BattlePopupPresenter>();
            battlePresenter.Initialize(userModel);
            battlePresenter.OpenBattlePopup();

            var rewardPopupPresenter = PresenterFactory.CreateOrGet<RewardPopupPresenter>();
            rewardPopupPresenter.Initialize(userModel);

            var equipmentPopupPresenter = PresenterFactory.CreateOrGet<EquipmentPopupPresenter>();
            equipmentPopupPresenter.Initialize(userModel);

            var equipmentInfoPopupPresenter = PresenterFactory.CreateOrGet<EquipmentInfoPopupPresenter>();
            equipmentInfoPopupPresenter.Initialize(userModel);

            var shopPopupPresenter = PresenterFactory.CreateOrGet<ShopPopupPresenter>();
            shopPopupPresenter.Initialize(userModel);

            var shopGachaListPopupPresenter = PresenterFactory.CreateOrGet<GachaListPopupPresenter>();
            shopGachaListPopupPresenter.Initialize();

            var gachaResultPopupPresenter = PresenterFactory.CreateOrGet<GachaResultPresenter>();
            gachaResultPopupPresenter.Initialize(userModel);

            var mergePopupPresenter = PresenterFactory.CreateOrGet<MergePopupPresenter>();
            mergePopupPresenter.Initialize(userModel);

            var mergeResultPopupPresenter = PresenterFactory.CreateOrGet<MergeResultPresenter>();
            mergeResultPopupPresenter.Initialize(userModel);

            var mergeAllResultPopupPresenter = PresenterFactory.CreateOrGet<MergeAllResultPopupPresenter>();
            mergeAllResultPopupPresenter.Initialize(userModel);

            var checkoutPopupPresenter = PresenterFactory.CreateOrGet<CheckOutPopupPresenter>();
            checkoutPopupPresenter.Initialize(userModel, checkoutModel);

            var missionPopupPresenter = PresenterFactory.CreateOrGet<MissionPopupPresenter>();
            missionPopupPresenter.Initialize(userModel, missionModel);

            var achievementPopupPresenter = PresenterFactory.CreateOrGet<AchievementPopupPresenter>();
            achievementPopupPresenter.Initialize(userModel, achievementModel);

            var offlinePopupPresenter = PresenterFactory.CreateOrGet<OfflineRewardPopupPresenter>();
            offlinePopupPresenter.Initialize(userModel);

            var fastRewardPopupPresenter = PresenterFactory.CreateOrGet<FastRewardPopupPresenter>();
            fastRewardPopupPresenter.Initialize(timeDataModel);

            var settingPopupPresenter = PresenterFactory.CreateOrGet<SettingPopupPresenter>();
            settingPopupPresenter.Initialize();

            var stageSelectPopupPresenter = PresenterFactory.CreateOrGet<StageSelectPopupPresenter>();
            stageSelectPopupPresenter.Initialize(userModel);
        }
    }
}