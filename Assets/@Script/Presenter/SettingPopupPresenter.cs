using System;
using System.Collections.Generic;
using SlimeMaster.Enum;
using SlimeMaster.Manager;
using SlimeMaster.OutGame.Popup;
using UnityEngine;

namespace SlimeMaster.Presenter
{
    [Serializable]
    public struct SettingData
    {
        public SettingType settingType;
        public bool isOn;
        public Action<SettingType, bool> onActivateAction;
    }

    public class SettingPopupPresenter : BasePresenter
    {
        private UI_SettingPopup _popup;

        public void Initialize()
        {
            GameManager.I.Event.AddEvent(GameEventType.ShowOutGameContentPopup, OnOpenPopup);
        }

        private void OnOpenPopup(object value)
        {
            OutGameContentButtonType type = (OutGameContentButtonType)value;
            if (type != OutGameContentButtonType.Setting)
            {
                return;
            }
            
            _popup = GameManager.I.UI.OpenPopup<UI_SettingPopup>();
            RefreshUI();
        }

        private void RefreshUI()
        {
            var settingDataList = new List<SettingData>();
            int length = System.Enum.GetNames(typeof(SettingType)).Length;
            for (int i = 0; i < length; i++)
            {
                SettingType settingType = (SettingType)i;
                SettingData settingData = new SettingData
                {
                    isOn = settingType switch
                    {
                        SettingType.BGM => GameManager.I.IsOnBGM,
                        SettingType.SFX => GameManager.I.IsOnSfx,
                        SettingType.Joystick => GameManager.I.IsFixJoystick
                    },
                    settingType = settingType,
                    onActivateAction = OnActivateSettingByType
                };
                settingDataList.Add(settingData);
            }
            
            _popup.UpdateUI(settingDataList, Application.version);
        }

        private void OnActivateSettingByType(SettingType settingType, bool isActivate)
        {
            switch (settingType)
            {
                case SettingType.BGM:
                    GameManager.I.IsOnBGM = isActivate;
                    break;
                case SettingType.SFX:
                    GameManager.I.IsOnSfx = isActivate;
                    break;
                case SettingType.Joystick:
                    GameManager.I.IsFixJoystick = isActivate;
                    break;
            }
            
            RefreshUI();
        }
    }
}