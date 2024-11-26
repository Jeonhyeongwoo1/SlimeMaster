using Cysharp.Threading.Tasks;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Factory;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Manager;
using SlimeMaster.InGame.View;
using SlimeMaster.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SlimeMaster.Manager
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
                }
                
                return _instance;
            }
        }
        
        private static GameManager _instance;
        
        public PoolManager Pool => I._pool ??= new PoolManager();
        public EventManager Event => I._event ??= new EventManager();
        public ResourcesManager Resource => I._resource ??= new ResourcesManager();
        public DataManager Data => I._data ??= new DataManager();
        public StageManager Stage => I._stage ??= new StageManager();
        public ObjectManager Object => I._object ??= new ObjectManager();
        public UIManager UI => I._ui ??= new UIManager();
        public AudioManager Audio => I._audio ??= new AudioManager();

        private EventManager _event;
        private PoolManager _pool;
        private ResourcesManager _resource;
        private DataManager _data;
        private StageManager _stage;
        private ObjectManager _object;
        private UIManager _ui;
        private AudioManager _audio;

        public GameContinueData GameContinueData => _gameContinueData ??= new GameContinueData();
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

        public void InitializeManager()
        {
            Data.Initialize();
            Stage.Initialize();
            Object.Initialize();
            Audio.Initialize();
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
                StageData stageData = Data.StageDict[stageIndex];
                Stage.StartStage(stageData).Forget();
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