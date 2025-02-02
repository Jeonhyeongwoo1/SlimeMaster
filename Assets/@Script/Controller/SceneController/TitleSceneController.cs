using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Factory;
using SlimeMaster.Firebase;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Interface;
using SlimeMaster.Managers;
using SlimeMaster.Model;
using SlimeMaster.Presenter;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SlimeMaster.Controller
{
    public class TitleSceneController : MonoBehaviour
    {
        [SerializeField] private Button _gameStartbutton;
        [SerializeField] private Slider _slider;

        private bool _isCompletedLogin = false;
        private FirebaseController _firebaseController;
        
        private async void Start()
        {
            _gameStartbutton.gameObject.SetActive(false);
            _gameStartbutton.SafeAddButtonListener(MoveToLobbyScene);
            await Initialize();
        }

        private async UniTask OnGuestLogin()
        {
            bool isSuccessLogin = await _firebaseController.TrySignInAnonymously();
            _isCompletedLogin = isSuccessLogin;
        }

        private async UniTask Initialize()
        {
            _firebaseController = new FirebaseController();
            bool isSuccess = await _firebaseController.FirebaseInit();
            if (!isSuccess)
            {
                Debug.LogError("Failed firebase init");
                return;
            }

            await Manager.I.Resource.LoadResourceAsync<Object>("PreLoad", (v) => _slider.value = v);
            Manager.I.Initialize();
            ServerHandlerFactory.InitializeServerHandlerRequest(_firebaseController, Manager.I.Data);

            await OnGuestLogin();
            await UniTask.WaitUntil(() => _isCompletedLogin);
            var response = await ServerHandlerFactory.Get<IUserClientSender>()
                .LoadUserDataRequest(new UserRequest());
            
            if (response.responseCode != ServerErrorCode.Success)
            {
                Debug.LogError("failed get user data");
                return;
            }

            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            foreach (var (key, itemData) in response.DBUserData.ItemDataDict)
            {
                userModel.CreateItem(itemData.ItemId, itemData.ItemValue);
            }

            List<WaveInfo> waveInfoList = new List<WaveInfo>(3);
            foreach (var (key, stageData) in response.DBUserData.StageDataDict)
            {
                waveInfoList.Clear();
                int index = 0;
                foreach (DBWaveData dbWaveData in stageData.WaveDataList)
                {
                    var waveInfo = new WaveInfo(dbWaveData.WaveIndex, dbWaveData.IsClear, dbWaveData.IsGet,
                        (WaveClearType)index);
                    waveInfoList.Add(waveInfo);
                    index++;
                }
                
                StageInfo stageInfo = new StageInfo(stageData.StageIndex, stageData.IsOpened, waveInfoList);
                userModel.AddStage(stageInfo);
            }

            DataManager dataManager = Manager.I.Data;
            userModel.ClearAndSetEquipmentDataList(response.DBUserData.EquippedItemDataList);

            if (response.DBUserData.UnEquippedItemDataList != null)
            {
                userModel.ClearAndSetUnEquipmentDataList(response.DBUserData.UnEquippedItemDataList);
            }

            userModel.CreatureData = dataManager.CreatureDict[Const.PLAYER_DATA_ID];

            var checkoutModel = ModelFactory.CreateOrGetModel<CheckoutModel>();
            checkoutModel.Initialize(response.DBCheckoutData,
                response.DBCheckoutData.TotalAttendanceDays);

            var missionModel = ModelFactory.CreateOrGetModel<MissionModel>();
            missionModel.SetMissionData(response.DBMissionContainerData);

            var achievementModel = ModelFactory.CreateOrGetModel<AchievementModel>();
            achievementModel.Initialize(response.DBAchievementContainerData);

            userModel.LastOfflineGetRewardTime.Value = response.LastOfflineGetRewardTime;
            TimeDataModel timeDataModel = ModelFactory.CreateOrGetModel<TimeDataModel>();
            timeDataModel.Initialize(response.LastLoginTime);
            
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
            
            _gameStartbutton.gameObject.SetActive(true);
        }

        public void MoveToLobbyScene()
        {
            SceneManager.LoadScene(SceneType.LobbyScene.ToString());
        }

        private IEnumerator Wait(float wait, Action action)
        {
            yield return new WaitForSeconds(wait);
            action?.Invoke();
        }
    }
}