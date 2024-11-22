using System;
using SlimeMaster.Enum;
using SlimeMaster.Interface;
using SlimeMaster.View;
using UnityEngine;
using UnityEngine.UI;

namespace Script.InGame.UI
{
    public class UI_LobbyScene : BaseUI, IView
    {
        [SerializeField] private UI_UserInfoItem _userInfoItem;
        [SerializeField] private ToggleGroup _toggleGroup;
        [SerializeField] private ToggleType _activateToggleType;
        
        public Action<GoodsType> onGoodsClickAction;
        public Action<ToggleType, ToggleType> onClickToggleAction;
        
        public override void Initialize()
        {
            base.Initialize();
            
            _userInfoItem.Initialize(onGoodsClickAction);
            Toggle[] childs = _toggleGroup.GetComponentsInChildren<Toggle>();
            foreach (Toggle toggle in childs)
            {
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener((isOn) => OnToggleChanged(toggle, isOn));
            }
        }

        private void OnToggleChanged(Toggle toggle, bool isOn)
        {
            if (!isOn)
            {
                return;
            }
            
            int length = Enum.GetNames(typeof(ToggleType)).Length;
            for (int i = 0; i < length; i++)
            {
                ToggleType toggleType = (ToggleType)i;
                string toggleTypeName = toggleType.ToString();
                if (toggle.name == toggleTypeName && toggleType != _activateToggleType)
                {
                    _activateToggleType = toggleType;
                    onClickToggleAction.Invoke(toggleType, _activateToggleType);
                    Debug.Log("IsOn" + toggle.name);
                    break;
                }
            }
        }

        public void UpdateUserGoodsInfo(GoodsType goodsType, string value)
        {
            _userInfoItem.UpdateUserGoodsInfo(goodsType, value);
        }
        
    }
}