using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Equipmenets;
using SlimeMaster.Factory;
using SlimeMaster.Firebase;
using SlimeMaster.Firebase.Data;
using SlimeMaster.Interface;
using SlimeMaster.Manager;
using SlimeMaster.Model;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SlimeMaster.Controller
{
    public class TitleSceneController : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Slider _slider;
        
        private async void Start()
        {
            _button.onClick.AddListener(MoveToLobbyScene);
            await Initialize();
        }

        private async UniTask Initialize()
        {
            _button.interactable = false;
            var firebaseController = new FirebaseController();
            bool isSuccess = await firebaseController.FirebaseInit();
            if (!isSuccess)
            {
                Debug.LogError("Failed firebase init");
                return;
            }

            if (!firebaseController.HasUserId)
            {
                await firebaseController.SignInAnonymously();
            }

            await GameManager.I.Resource.LoadResourceAsync<Object>("PreLoad", (v) => _slider.value = v);
            GameManager.I.ManagerInitialize();
            ServerHandlerFactory.InitializeServerHandlerRequest(firebaseController, GameManager.I.Data);
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

            DataManager dataManager = GameManager.I.Data;
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

            userModel.LastOfflineGetRewardTime = response.LastOfflineGetRewardTime;
            TimeDataModel timeDataModel = ModelFactory.CreateOrGetModel<TimeDataModel>();
            timeDataModel.Initialize(response.LastLoginTime);
            
            _button.interactable = true;
            SceneManager.LoadScene(SceneType.LobbyScene.ToString());
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