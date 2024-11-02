using Cysharp.Threading.Tasks;
using SlimeMaster.InGame.Controller;
using SlimeMaster.InGame.Entity;
using SlimeMaster.InGame.Enum;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SlimeMaster.InGame.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager I
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();
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

        private EventManager _event;
        private PoolManager _pool;
        private ResourcesManager _resource;
        private DataManager _data;
        private StageManager _stage;
        private ObjectManager _object;
        
        
        private PlayerController _player;

        private GameState _gameState;
        private int _stageIndex = 1;

        private void Start()
        {
            LoadData();
        }

        private void CreateManager()
        {
            _pool = new PoolManager();
            _event = new EventManager();
            _resource = new ResourcesManager();
            _data = new DataManager();
            _stage = new StageManager();
            _object = new ObjectManager();
        }

        private async void LoadData()
        {
            CreateManager();
           
            _player = FindObjectOfType<PlayerController>();
            await Resource.LoadResourceAsync<Object>("PreLoad", null);
            _data.Initialize();
            _object.Initialize(_event, _resource, _data, _player);
            _stage.Initialize(_stageIndex, _player);
            
            GameStart();
        }

        private void GameStart()
        {
            _stage.StartStage();
        }

        private void OnPlayerDead()
        {
            Debug.LogWarning("On player dead");
            UpdateGameState(GameState.End);
        }

        private void UpdateGameState(GameState gameState) => _gameState = gameState;
    }
}