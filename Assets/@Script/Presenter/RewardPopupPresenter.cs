using System;
using System.Collections.Generic;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Manager;
using SlimeMaster.Model;
using SlimeMaster.UISubItemElement;
using UnityEngine;
using UnityEngine.Serialization;

namespace SlimeMaster.Presenter
{
    [Serializable]
    public struct RewardItemData
    {
        public int materialItemId;
        public string equipmentId;
        public int rewardValue;
    }
    
    public class RewardPopupPresenter : BasePresenter
    {
        private UI_RewardPopup _rewardPopup;
        private UserModel _userModel;
        
        public void Initialize(UserModel userModel)
        {
            _userModel = userModel;
            GameManager.I.Event.AddEvent(GameEventType.GetReward, OnGetReward);
        }

        private void OnGetReward(object value)
        {
            List<RewardItemData> list = (List<RewardItemData>)value;
            DataManager dataManager = GameManager.I.Data;
            ResourcesManager resourcesManager = GameManager.I.Resource;

            _rewardPopup = GameManager.I.UI.OpenPopup<UI_RewardPopup>();
            _rewardPopup.ReleaseRewardItemScrollContentChildComponent();
            
            foreach (RewardItemData itemData in list)
            {
                int id = itemData.materialItemId;
                GameObject prefab = resourcesManager.Instantiate(nameof(UI_MaterialItem));
                if (!prefab.TryGetComponent(out UI_MaterialItem materialItem))
                {
                    Debug.LogError($"Failed try get component ${prefab.name}");
                    continue;
                }

                if (dataManager.MaterialDataDict.TryGetValue(id, out MaterialData materialData))
                {
                    Sprite sprite = resourcesManager.Load<Sprite>(materialData.SpriteName);
                    Color color = Const.EquipmentUIColors.GetMaterialGradeColor(materialData.MaterialGrade);
                    materialItem.UpdateUI(sprite, color, itemData.rewardValue.ToString(),
                        true, _rewardPopup.RewardItemScrollContentObject);
                }
                else if (!string.IsNullOrEmpty(itemData.equipmentId) &&
                         dataManager.EquipmentDataDict.TryGetValue(itemData.equipmentId,
                             out EquipmentData equipmentData))
                {
                    Sprite sprite = resourcesManager.Load<Sprite>(equipmentData.SpriteName);
                    Color color = Const.EquipmentUIColors.GetEquipmentGradeColor(equipmentData.EquipmentGrade);
                    materialItem.UpdateUI(sprite, color, itemData.rewardValue.ToString(),
                        true, _rewardPopup.RewardItemScrollContentObject);
                }
            }
        }
    }
}