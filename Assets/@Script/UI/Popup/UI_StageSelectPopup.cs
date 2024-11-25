using System;
using System.Collections;
using System.Collections.Generic;
using SlimeMaster.Common;
using SlimeMaster.Popup;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace SlimeMaster.OutGame.Popup
{
    public class UI_StageSelectPopup : BasePopup
    {
        public Transform AppearingMonsterContentObject => _appearingMonsterContentObject;
        public Transform StageScrollContentObject => _stageScrollContentObject;
        
        [SerializeField] private Transform _appearingMonsterContentObject;
        [SerializeField] private HorizontalScrollSnap _stageSelectScrollView;
        [SerializeField] private Transform _stageScrollContentObject;
        [SerializeField] private Button _selectStageButton;
        [SerializeField] private Button _backButton;

        public Action onSelectStageAction;
        public Action onCloseStageSelectAction;
        public Action<int> onChangedSelectStageAction;

        public override void AddEvents()
        {
            base.AddEvents();
            
            _selectStageButton.SafeAddButtonListener(onSelectStageAction.Invoke);
            _backButton.SafeAddButtonListener(onCloseStageSelectAction.Invoke);
        }
        public void AddChildObjectInHorizontalScrollSnap(GameObject child)
        {
            _stageSelectScrollView.AddChild(child);
        }

        public void UpdateUI(int stageIndex)
        {
            Debug.Log("stageIndex : " + stageIndex);
            // _stageSelectScrollView.StartingScreen = stageIndex;
        }

        public void OnChangedSelectStage(int index)
        {
            onChangedSelectStageAction.Invoke(index);
        }
    }
}