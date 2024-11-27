using System;
using System.Collections.Generic;
using SlimeMaster.Enum;
using SlimeMaster.Managers;
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
            Manager.I.Event.AddEvent(GameEventType.ShowOutGameContentPopup, OnOpenPopup);
        }

        private void OnOpenPopup(object value)
        {
            OutGameContentButtonType type = (OutGameContentButtonType)value;
            if (type != OutGameContentButtonType.Setting)
            {
                return;
            }
            
            _popup = Manager.I.UI.OpenPopup<UI_SettingPopup>();
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
                        SettingType.BGM => Manager.I.IsOnBGM,
                        SettingType.SFX => Manager.I.IsOnSfx,
                        SettingType.Joystick => Manager.I.IsFixJoystick
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
                    Manager.I.IsOnBGM = isActivate;
                    break;
                case SettingType.SFX:
                    Manager.I.IsOnSfx = isActivate;
                    break;
                case SettingType.Joystick:
                    Manager.I.IsFixJoystick = isActivate;
                    break;
            }
            
            RefreshUI();
        }
    }
}