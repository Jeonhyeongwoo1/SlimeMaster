using System;
using System.Collections;
using System.Collections.Generic;
using SlimeMaster.Common;
using SlimeMaster.Enum;
using SlimeMaster.Popup;
using SlimeMaster.Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeMaster.OutGame.Popup
{
    public class UI_SettingPopup : BasePopup
    {
        [Serializable]
        public struct ButtonGroup
        {
            public SettingType settingType;
            public Button onButton;
            public Button offButton;
        }

        [SerializeField] private List<ButtonGroup> _buttonGroupList;
        [SerializeField] private TextMeshProUGUI _versionText;

        public void UpdateUI(List<SettingData> settingDataList, string version)
        {
            foreach (SettingData settingData in settingDataList)
            {
                ButtonGroup group = _buttonGroupList.Find(v => v.settingType == settingData.settingType);
                group.onButton.gameObject.SetActive(settingData.isOn);
                group.offButton.gameObject.SetActive(!settingData.isOn);
                group.onButton.SafeAddButtonListener(()=> settingData.onActivateAction.Invoke(group.settingType, false));
                group.offButton.SafeAddButtonListener(()=> settingData.onActivateAction.Invoke(group.settingType, true));
            }

            _versionText.text = version;
        }
    }
}