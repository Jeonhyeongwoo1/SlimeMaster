using System;
using SlimeMaster.Common;
using SlimeMaster.Manager;
using SlimeMaster.Popup;
using SlimeMaster.UISubItemElement;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.OutGame.Popup
{
    public class UI_GachaResultsPopup : BasePopup
    {
        public Transform ResultsContentScrollObject => _resultsContentScrollObject;
        
        [SerializeField] private Button _skipButton;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Transform _resultsContentScrollObject;
        [SerializeField] private GameObject _openContentObject;
        
        public Action onEndGachaAnimationAction;
        
        public override void AddEvents()
        {
            base.AddEvents();
         
            _skipButton.SafeAddButtonListener(OnClickSkipButton);
            _confirmButton.SafeAddButtonListener(()=>
            {
                ReleaseItem();
                _openContentObject.SetActive(true);
                GameManager.I.UI.ClosePopup();
            });
        }
        
        private void OnClickSkipButton()
        {
            _openContentObject.SetActive(false);
            OnEndGachaAnimation();
        }

        public void ReleaseItem()
        {
            var childs = Utils.GetChildComponent<UI_MaterialItem>(_resultsContentScrollObject);
            if (childs == null)
            {
                return;
            }
            
            foreach (UI_MaterialItem item in childs)
            {
                item.Release();
            }
        }

        public void OnEndGachaAnimation()
        {
            onEndGachaAnimationAction.Invoke();
        }
    }
}