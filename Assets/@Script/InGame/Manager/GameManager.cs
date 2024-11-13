using System;
using System.Collections.Generic;
using System.Linq;
using Script.InGame.UI.Popup;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.InGame.Controller;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Popup;
using SlimeMaster.InGame.Skill;
using SlimeMaster.InGame.View;
using UnityEngine;
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
    }
}