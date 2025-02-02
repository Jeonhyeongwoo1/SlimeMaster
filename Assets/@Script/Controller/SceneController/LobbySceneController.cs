using System;
using Script.InGame.UI;
using SlimeMaster.Factory;
using SlimeMaster.Interface;
using SlimeMaster.Managers;
using SlimeMaster.Model;
using SlimeMaster.Presenter;
using UnityEngine;

namespace SlimeMaster.Controller
{
    public class LobbySceneController : MonoBehaviour
    {
        private void Start()
        {
            Manager manager = Manager.I;
            var lobbyUI = manager.UI.ShowUI<UI_LobbyScene>();
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            var lobbyPresenter = PresenterFactory.CreateOrGet<LobbyPresenter>();
            lobbyPresenter.Initialize(userModel, lobbyUI);
            
            var battlePresenter = PresenterFactory.CreateOrGet<BattlePopupPresenter>();
            battlePresenter.Initialize(userModel);
            battlePresenter.OpenBattlePopup();

        }

        private async void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                var response = await ServerHandlerFactory.Get<IUserClientSender>().CopyNewUser();
                Debug.Log("response  " + response.responseCode);
            }
        }
    }
}