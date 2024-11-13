using System;
using SlimeMaster.InGame.Manager;
using SlimeMaster.InGame.View;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SlimeMaster.Controller
{
    public class GameSceneController : MonoBehaviour
    {
        private async void Start()
        {
            GameManager gameManager = GameManager.I;
            gameManager.UI.ShowUI<UI_GameScene>();
            gameManager.Stage.StartStage(gameManager.Data.StageDict[1]);
        }
    }
}