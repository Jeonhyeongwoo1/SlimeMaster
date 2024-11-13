using System;
using System.Collections;
using System.Collections.Generic;
using Script.InGame.UI;
using SlimeMaster.Factory;
using SlimeMaster.InGame.Controller;
using SlimeMaster.InGame.Interface;
using SlimeMaster.InGame.Manager;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SlimeMaster.Controller
{
    public class LobbySceneController : MonoBehaviour
    {
        private async void Start()
        {
            GameManager gameManager = GameManager.I;
            await gameManager.Resource.LoadResourceAsync<Object>("PreLoad", null);
            GameManager.ManagerInitialize();

            var lobbyUI = gameManager.UI.ShowUI<UI_LobbyScene>();

            var lobbyPresenter = PresenterFactory.CreateOrGet<LobbyPresenter>();
            var characterModel = ModelFactory.CreateOrGetModel<UserModel>();
            lobbyPresenter.Initialize(characterModel, lobbyUI);
        }
    }
}