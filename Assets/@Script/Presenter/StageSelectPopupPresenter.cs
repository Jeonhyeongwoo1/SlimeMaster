using System.Collections.Generic;
using System.Linq;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Managers;
using SlimeMaster.Model;
using SlimeMaster.OutGame.Popup;
using SlimeMaster.UISubItemElement;
using UnityEngine;

namespace SlimeMaster.Presenter
{
    public class StageSelectPopupPresenter : BasePresenter
    {
        private UserModel _userModel;
        private UI_StageSelectPopup _popup;
        private DataManager _dataManager = Manager.I.Data;
        private UIManager _uiManager = Manager.I.UI;
        private ResourcesManager _resourcesManager = Manager.I.Resource;
        private int _currentStageIndex = 0;
        
        public void Initialize(UserModel userModel)
        {
            _userModel = userModel;
            Manager.I.Event.AddEvent(GameEventType.ShowStageSelectPopup, OnOpenStageSelectPopup);
        }

        private void OnOpenStageSelectPopup(object value)
        {
            _popup = _uiManager.OpenPopup<UI_StageSelectPopup>();
            _popup.onChangedSelectStageAction = OnChangedSelectStageAction;
            _popup.onSelectStageAction = OnSelectStageAction;
            _popup.onCloseStageSelectAction = OnCloseStageSelectPopup;
            _popup.AddEvents();

            _currentStageIndex = Manager.I.CurrentStageIndex;
            ResourcesManager resourcesManager = Manager.I.Resource;
            List<bool> stageCompletedList = new List<bool>(3);
            _popup.ReleaseSubItem<UI_StageInfoItem>(_popup.StageScrollContentObject);
            foreach (var (key, stageData) in _dataManager.StageDict)
            {
                stageCompletedList.Clear();
                var stageInfoItem = _uiManager.AddSubElementItem<UI_StageInfoItem>(_popup.StageScrollContentObject);
                int stageIndex = stageData.StageIndex;
                StageInfo stageInfo =  _userModel.GetStageInfo(stageIndex);
                bool isOpenedStage = !stageInfo.IsOpenedStage.Value;
                var sprite = resourcesManager.Load<Sprite>(stageData.StageImage);
                foreach (var t in stageInfo.WaveInfoList.Value)
                {
                    stageCompletedList.Add(t.IsClear.Value);
                }
                
                stageInfoItem.UpdateUI(stageData.StageIndex, sprite, isOpenedStage, stageInfo.lastClearWaveIndex, stageCompletedList);
                _popup.AddChildObjectInHorizontalScrollSnap(stageInfoItem.gameObject);
                
                AddMonsterInfoItem(stageData.AppearingMonsters, stageData.StageLevel);
            }

            _popup.UpdateUI(Manager.I.CurrentStageIndex);
        }

        private void AddMonsterInfoItem(List<int> appearingMonsters, int stageLevel)
        {
            _popup.ReleaseSubItem<UI_MonsterInfoItem>(_popup.AppearingMonsterContentObject);
            foreach (int stageDataAppearingMonster in appearingMonsters)
            {
                CreatureData creatureData = _dataManager.CreatureDict[stageDataAppearingMonster];
                var monsterInfoItem = _uiManager.AddSubElementItem<UI_MonsterInfoItem>(_popup.AppearingMonsterContentObject);
                Sprite monsterSprite = _resourcesManager.Load<Sprite>(creatureData.IconLabel);
                monsterInfoItem.UpdateUI(monsterSprite, stageLevel);
            }
        }

        private void OnChangedSelectStageAction(int index)
        {
            int stageIndex = index + 1;
            _currentStageIndex = stageIndex;
            StageData stageData = _dataManager.StageDict[stageIndex];
            AddMonsterInfoItem(stageData.AppearingMonsters, stageData.StageLevel);   
        }

        private void OnSelectStageAction()
        {
            bool isOpened = _userModel.GetStageInfo(_currentStageIndex).IsOpenedStage.Value;
            if (!isOpened)
            {
                return;
            }

            Manager.I.CurrentStageIndex = _currentStageIndex;
            _uiManager.ClosePopup();
            Manager.I.Event.Raise(GameEventType.ChangeStage);
        }
        
        private void OnCloseStageSelectPopup()
        {
            _currentStageIndex = 0;
            _uiManager.ClosePopup();   
        }
    }
}