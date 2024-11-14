using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SlimeMaster.Factory;
using SlimeMaster.Firebase;
using SlimeMaster.Firebase.Data;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Manager;
using SlimeMaster.Server;
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
            GameManager.ManagerInitialize();
            ServerHandlerFactory.InitializeServerHandlerRequest(firebaseController, GameManager.I.Data);
            var response = await ServerHandlerFactory.Get<ServerUserRequestHandler>()
                .LoadUserDataRequest(new UserRequest());
            if (response.responseCode != ServerErrorCode.Success)
            {
                Debug.LogError("failed get user data");
                return;
            }

            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            foreach (var (key, itemData) in response.DBUserData.ItemDataDict)
            {
                userModel.AddItem(itemData.ItemId, itemData.ItemValue);
            }

            List<WaveInfo> waveInfoList = new List<WaveInfo>(3);
            foreach (var (key, stageData) in response.DBUserData.StageDataDict)
            {
                waveInfoList.Clear();
                foreach (DBWaveData dbWaveData in stageData.WaveDataList)
                {
                    var waveInfo = new WaveInfo(dbWaveData.WaveIndex, dbWaveData.IsClear, dbWaveData.IsGet);
                    waveInfoList.Add(waveInfo);
                }
                
                StageInfo stageInfo = new StageInfo(stageData.StageIndex, waveInfoList);
                userModel.AddStage(stageInfo);
            }

            _button.interactable = true;
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