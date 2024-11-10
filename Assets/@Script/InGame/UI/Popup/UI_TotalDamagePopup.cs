using System;
using System.Collections.Generic;
using SlimeMaster.InGame.Manager;
using SlimeMaster.Popup;
using SlimeMaster.UISubItemElement;
using UnityEngine;

namespace SlimeMaster.InGame.Popup
{
    [Serializable]
    public struct TotalDamageInfoData
    {
        public Sprite skillSprite;
        public string skillName;
        public float skillAccumlatedDamage;
        public float skillDamageRatioByTotalDamage;
    }
    
    public class UI_TotalDamagePopup : BasePopup
    {
        [SerializeField] private List<UI_SkillDamageItem> _skillDamageItemList;

        public override void OpenPopup()
        {
            base.OpenPopup();

            var list = GameManager.I.Object.Player.GetTotalDamageInfoData();
            for (int i = 0; i < _skillDamageItemList.Count; i++)
            {
                if (i >= list.Count)
                {
                    _skillDamageItemList[i].gameObject.SetActive(false);
                    continue;
                }

                TotalDamageInfoData data = list[i];
                _skillDamageItemList[i].UpdateUI(data.skillDamageRatioByTotalDamage,
                    data.skillAccumlatedDamage.ToString(), data.skillName, data.skillSprite);
                _skillDamageItemList[i].transform.SetAsFirstSibling();
            }
        }
    }
}