using System.IO;
using Clicker.Manager;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Factory;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Manager;
using SlimeMaster.InGame.View;
using SlimeMaster.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SlimeMaster.Managers
{
    public class Manager
    {
        public static Manager I => _instance ??= new Manager();
        
        public PoolManager Pool => I._pool ??= new PoolManager();
        public EventManager Event => I._event ??= new EventManager();
        public ResourcesManager Resource => I._resource ??= new ResourcesManager();
        public DataManager Data => I._data ??= new DataManager();
        public GameManager Game => I._game ??= new GameManager();
        public ObjectManager Object => I._object ??= new ObjectManager();
        public UIManager UI => I._ui ??= new UIManager();
        public AudioManager Audio => I._audio ??= new AudioManager();
        public WebManager Web => I._web ??= new WebManager();
        
        public GameContinueData GameContinueData
        {
            get =>I._gameContinueData ??= new GameContinueData();
            set => I._gameContinueData = value;
        }

        private static Manager _instance;
        private EventManager _event;
        private PoolManager _pool;
        private ResourcesManager _resource;
        private DataManager _data;
        private GameManager _game;
        private ObjectManager _object;
        private UIManager _ui;
        private AudioManager _audio;
        private WebManager _web;
        private GameContinueData _gameContinueData;

        public bool IsOnBGM
        {
            get => PlayerPrefs.GetInt(nameof(IsOnBGM), 0) == 0;
            set
            {
                PlayerPrefs.SetInt(nameof(IsOnBGM), value ? 0 : 1);
                if (value)
                {
                    Scene scene = SceneManager.GetActiveScene();
                    Audio.Play(Sound.Bgm, scene.name == SceneType.LobbyScene.ToString() ? "Bgm_Lobby" : "Bgm_Game");
                }
                else
                {
                    Audio.Stop(Sound.Bgm);
                }
            }
        }

        public bool IsOnSfx
        {
            get => PlayerPrefs.GetInt(nameof(IsOnSfx), 0) == 0;
            set
            {
                PlayerPrefs.SetInt(nameof(IsOnSfx), value ? 0 : 1);
                if (!value)
                {
                    Audio.Stop(Sound.Effect);
                }
            }
        }

        public bool IsFixJoystick
        {
            get => PlayerPrefs.GetInt(nameof(IsFixJoystick), 0) == 0;
            set => PlayerPrefs.SetInt(nameof(IsFixJoystick), value ? 0 : 1);
        }

        public int CurrentStageIndex
        {
            get
            {
                int savedStageIndex = PlayerPrefs.GetInt(nameof(CurrentStageIndex), 0);
                int stageIndex = savedStageIndex == 0
                    ? ModelFactory.CreateOrGetModel<UserModel>().GetLastClearStageIndex()
                    : savedStageIndex;

                return stageIndex;
            }
            set => PlayerPrefs.SetInt(nameof(CurrentStageIndex), value);
        }

        public void Initialize()
        {
            Data.Initialize();
            Game.Initialize();
            Object.Initialize();
            Audio.Initialize();
            Web.Initialize();

            // TryLoadGameContinueData();
        }

        public async void StartGame()
        {
            string sceneName = SceneType.GameScene.ToString();
            SceneManager.sceneLoaded -= OnLoadedGameScene;
            SceneManager.sceneLoaded += OnLoadedGameScene;
            
            var operation = SceneManager.LoadSceneAsync(sceneName);
            if (!operation.isDone)
            {
                await UniTask.Yield();
            }
            
            //Fader 
        }

        private void OnLoadedGameScene(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name != SceneType.GameScene.ToString())
            {
                return;
            }
            
            var gameSceneUI = UI.ShowUI<UI_GameScene>();
            gameSceneUI.Initialize();

            bool isExistContinueGameData = GameContinueData.IsContinue;
            int stageIndex = isExistContinueGameData ? GameContinueData.stageIndex : CurrentStageIndex;
            int waveIndex = isExistContinueGameData ? GameContinueData.waveIndex : 0;
            StageData stageData = Data.StageDict[stageIndex];
            Game.StartStage(stageData, waveIndex).Forget();
            Audio.Play(Sound.Bgm, "Bgm_Game");

            var playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            playerModel.Reset();
            var stageModel = ModelFactory.CreateOrGetModel<StageModel>();
            stageModel.Reset();

            // if (!isExistContinueGameData)
            // {
            //     playerModel.Reset();
            // }
            // else
            // {
            //     playerModel.SoulAmount.Value = GameContinueData.soulAmount;
            //     playerModel.CurrentLevel.Value = GameContinueData.playerLevel;
            //     playerModel.CurrentExp.Value = GameContinueData.playerExp;
            //     var levelData = Data.LevelDataDict[GameContinueData.playerLevel];
            //     playerModel.CurrentExpRatio.Value = (float)playerModel.CurrentExp.Value / levelData.TotalExp;
            //
            //     var stageModel = ModelFactory.CreateOrGetModel<StageModel>();
            //     stageModel.killCount.Value = GameContinueData.killCount;
            //     stageModel.currentWaveStep.Value = GameContinueData.waveIndex;
            // }
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
        
        public void SaveGameContinueData()
        {
            if (GameContinueData == null)
            {
                return;
            }
            
            var jsonStr = JsonConvert.SerializeObject(GameContinueData);
            File.WriteAllText(Const.PATH, jsonStr);
        }

        public bool TryLoadGameContinueData()
        {
            if (!File.Exists(Const.PATH))
            {
                return false;
            }

            string fileStr = File.ReadAllText(Const.PATH);
            var gameContinueData = JsonConvert.DeserializeObject<GameContinueData>(fileStr);
            if (gameContinueData == null)
            {
                return false;
            }

            GameContinueData = gameContinueData;
            return true;
        }
    }
}