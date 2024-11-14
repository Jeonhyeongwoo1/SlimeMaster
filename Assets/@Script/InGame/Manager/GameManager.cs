using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Script.InGame.UI.Popup;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Factory;
using SlimeMaster.InGame.Controller;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Popup;
using SlimeMaster.InGame.Skill;
using SlimeMaster.InGame.View;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace SlimeMaster.InGame.Manager
{
    public class GameManager
    {
        public static GameManager I
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameManager();
                    CreateManager();
                }
                
                return _instance;
            }
        }
        
        private static GameManager _instance;
        
        public PoolManager Pool => _pool;
        public EventManager Event => _event;
        public ResourcesManager Resource => _resource;
        public DataManager Data => _data;
        public StageManager Stage => _stage;
        public ObjectManager Object => _object;
        public UIManager UI => _ui;

        private static EventManager _event;
        private static PoolManager _pool;
        private static ResourcesManager _resource;
        private static DataManager _data;
        private static StageManager _stage;
        private static ObjectManager _object;
        private static UIManager _ui;

        public GameContinueData GameContinueData => _gameContinueData ??= new GameContinueData();
        private GameContinueData _gameContinueData;
        
        private static void CreateManager()
        {
            _pool = new PoolManager();
            _event = new EventManager();
            _resource = new ResourcesManager();
            _data = new DataManager();
            _stage = new StageManager();
            _object = new ObjectManager();
            _ui = new UIManager();
        }

        public static void ManagerInitialize()
        {
            _data.Initialize();
            _stage.Initialize();
            _object.Initialize();
        }

        public async void StartGame()
        {
            string sceneName = SceneType.GameScene.ToString();
            SceneManager.sceneLoaded += (scene, loadSceneMode) =>
            {
                if (scene.name != sceneName)
                {
                    return;
                }
                
                var gameSceneUI = UI.ShowUI<UI_GameScene>();
                gameSceneUI.Initialize();
            
                UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
                int stageIndex = userModel.GetLastClearStageIndex();
                StageData stageData = _data.StageDict[stageIndex];
                _stage.StartStage(stageData).Forget();
            };
            
            var operation = SceneManager.LoadSceneAsync(sceneName);
            if (!operation.isDone)
            {
                await UniTask.Yield();
            }
            
            //Fader 
        }

        public async void MoveToLobbyScene()
        {
            string sceneName = SceneType.LobbyScene.ToString();
            var operation = SceneManager.LoadSceneAsync(sceneName);
            if (!operation.isDone)
            {
                await UniTask.Yield();
            }
        }
    }
}