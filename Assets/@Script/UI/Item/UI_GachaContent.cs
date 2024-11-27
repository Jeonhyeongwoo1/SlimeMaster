using System;
using System.Collections.Generic;
using SlimeMaster.Common;
using SlimeMaster.Enum;
using SlimeMaster.Managers;
using SlimeMaster.Presenter;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.OutGame.UI
{
    
    public class UI_GachaContent : MonoBehaviour
    {
        [Serializable]
        public struct GachaElement
        {
            public ShopItemType shopItemType;
            public UI_GachaElement gachaElement;
        }

        [SerializeField] private List<GachaElement> _gachaElementList;
        [SerializeField] private Button _probabilityTableButton;
        [SerializeField] private GachaType _gachaType;

        private void Awake()
        {
            _probabilityTableButton.SafeAddButtonListener(OnClickProbabilityTableButton);
        }
        
        private void OnClickProbabilityTableButton()
        {
            Manager.I.Event.Raise(GameEventType.ShowGachaListPopup, _gachaType);
        }

        public void UpdateUI(List<ShopGachaItemData> advancedGachaItemDataList)
        {
            foreach (ShopGachaItemData itemData in advancedGachaItemDataList)
            {
                GachaElement element = _gachaElementList.Find(v => v.shopItemType == itemData.shopItemType);
                element.gachaElement.UpdateUI(itemData.costItemSprite, itemData.costItemValue, itemData.onPurchaseItemAction);
            }
        }
    }
}