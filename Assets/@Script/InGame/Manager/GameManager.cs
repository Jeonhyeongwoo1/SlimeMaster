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
        public UIManager UI => _ui;

        private EventManager _event;
        private PoolManager _pool;
        private ResourcesManager _resource;
        private DataManager _data;
        private StageManager _stage;
        private ObjectManager _object;
        private UIManager _ui;
        
        private PlayerController _player;

        public List<SupportSkillData> lockSupportSkillDataList = new(Const.SUPPORT_ITEM_USEABLECOUNT);

        public List<SupportSkillData> CurrentSupportSkillDataList
        {
            get
            {
                if (_currentSupportSkillDataList == null || _currentSupportSkillDataList.Count == 0)
                {
                    _currentSupportSkillDataList = Object.Player.SkillBook.GetRecommendSupportSkillDataList();
                }

                return _currentSupportSkillDataList;
            }
            set
            {
                _currentSupportSkillDataList ??= new List<SupportSkillData>();
                _currentSupportSkillDataList = value;
            }
        }
        
        private List<SupportSkillData> _currentSupportSkillDataList;
        
        private GameState _gameState;
        private int _stageIndex = 1;

        private void Start()
        {
            Initialize();
        }
        private void CreateManager()
        {
            _pool = new PoolManager();
            _event = new EventManager();
            _resource = new ResourcesManager();
            _data = new DataManager();
            _stage = new StageManager();
            _object = new ObjectManager();
            _ui = new UIManager();
        }
        
        private async void Initialize()
        {
            CreateManager();
           
            _player = FindObjectOfType<PlayerController>(true);
            await Resource.LoadResourceAsync<Object>("PreLoad", null);
            _data.Initialize();
            _stage.Initialize(_stageIndex, _player);
            _ui.ShowUI<UI_GameScene>();
            _object.Initialize(_player);
            AddEvents();
            GameStart();
        }

        private void AddEvents()
        {
            _event.AddEvent(GameEventType.LevelUp, OnLevelUp);
            _event.AddEvent(GameEventType.UpgradeOrAddNewSkill, OnUpgradeOrAddNewSkill);
            _event.AddEvent(GameEventType.ActivateDropItem, OnActivateDropItem);
        }
        
        private void OnActivateDropItem(object value)
        {
            DropItemData dropItemData = (DropItemData)value;
            switch (dropItemData.DropItemType)
            {
                case DropableItemType.DropBox:
                    var learnSkillPopup = _ui.OpenPopup<UI_LearnSkillPopup>();
                    List<BaseSkill> skillList = Object.Player.SkillBook.GetRecommendSkillList(1);
                    if (skillList == null)
                    {
                        Debug.LogError("failed get skill list");
                        learnSkillPopup.ClosePopup();
                        return;
                    }
                    
                    learnSkillPopup.UpdateSKillItem(skillList[0]);
                    Time.timeScale = 0;
                    break;
            }
        }

        private void OnLevelUp(object value)
        {
            SkillBook skillBook = (SkillBook)value;
            
            /*
             *  스킬리스트 정리
             *  총 6개의 스킬을 얻음.
             *  6개의 스킬을 가지고 있으면
             *      만랩이 아닌 스킬중에서 3개를 선택
             *  아니면
             *  선택할 수 있는 스킬들 중에서 만랩이 아닌 스킬 3개를 선택
             */

            List<BaseSkill> skillList = skillBook.GetRecommendSkillList();
            if (skillList.Count == 0)
            {
                Debug.Log($"recommend skill list {skillList.Count}");
                return;
            }
            
            var popup = _ui.OpenPopup<UI_SkillSelectPopup>();
            popup.UpdateUI(skillList, skillBook.ActivateSkillList);
            
            Time.timeScale = 0;
        }

        private void OnUpgradeOrAddNewSkill(object value)
        {
            int skillId = (int)value;
            SkillData skill = _data.SkillDict[skillId];
            _player.UpgradeOrAddSKill(skill);
            _player.LevelUp();
            _ui.ClosePopup();

            List<BaseSkill> skillList = _player.SkillBook.ActivateSkillList;
            var gamesceneUI = _ui.SceneUI as UI_GameScene;
            gamesceneUI.UpdateSkillSlotItem(skillList);

            Time.timeScale = 1;
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

        public bool TryResetSupportSkillList()
        {
            PlayerController player = Object.Player;
            if (player.SoulAmount < Const.CHANGE_SUPPORT_SKILL_AMOUNT)
            {
                return false;
            }

            player.SoulAmount -= Const.CHANGE_SUPPORT_SKILL_AMOUNT;
            CurrentSupportSkillDataList = player.SkillBook.GetRecommendSupportSkillDataList();
            return true;
        }
    }
}