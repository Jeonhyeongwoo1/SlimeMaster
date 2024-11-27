using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Managers;
using SlimeMaster.Popup;
using SlimeMaster.UISubItemElement;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SlimeMaster.InGame.Popup
{
    public class UI_GameResultPopup : BasePopup
    {
        [SerializeField] private TextMeshProUGUI _stageDepthText;
        [SerializeField] private TextMeshProUGUI _gamePlayTimeText;
        [SerializeField] private TextMeshProUGUI _rewardGoldText;
        [SerializeField] private TextMeshProUGUI _totalMonsterKillText;
        [SerializeField] private Transform _resultRewardScrollContent;
        [FormerlySerializedAs("_closeButton")] [SerializeField] private Button _confirmButton;

        private void Start()
        {
            SafeButtonAddListener(ref _confirmButton, () => Managers.Manager.I.MoveToLobbyScene());
        }

        public void UpdateUI(string stageDepth, string gameplayTime, int rewardGold, string totalMonsterKillCount)
        {
            _stageDepthText.text = stageDepth;
            _gamePlayTimeText.text = gameplayTime;
            _rewardGoldText.text = rewardGold.ToString();
            _totalMonsterKillText.text = totalMonsterKillCount;

            var childs = Utils.GetChildComponent<UI_MaterialItem>(_resultRewardScrollContent);
            foreach (UI_MaterialItem materialItem in childs)
            {
                materialItem.Release();
            }
            
            var item = Managers.Manager.I.UI.AddSubElementItem<UI_MaterialItem>(_resultRewardScrollContent);
            var materialData = Managers.Manager.I.Data.MaterialDataDict[(int)MaterialType.RandomScroll];
            Sprite sprite = Managers.Manager.I.Resource.Load<Sprite>(materialData.SpriteName);
            Color color = Const.EquipmentUIColors.GetMaterialGradeColor(materialData.MaterialGrade);
            item.UpdateUI(sprite, color, rewardGold.ToString(), true, _resultRewardScrollContent);
        }
    }
}