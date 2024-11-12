using System.Collections;
using System.Collections.Generic;
using SlimeMaster.Common;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Item;
using SlimeMaster.InGame.Manager;
using SlimeMaster.Popup;
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

        public void UpdateUI(string stageDepth, string gameplayTime, int rewardGold, string totalMonsterKillCount)
        {
            _stageDepthText.text = stageDepth;
            _gamePlayTimeText.text = gameplayTime;
            _rewardGoldText.text = rewardGold.ToString();
            _totalMonsterKillText.text = totalMonsterKillCount;

            Transform[] result = Utils.GetChilds(_resultRewardScrollContent);
            foreach (Transform tr in result)
            {
                DestroyImmediate(tr.gameObject);
            }

            var item = GameManager.I.UI.AddSubElementItem<UI_MaterialItem>(_resultRewardScrollContent);
            Sprite sprite =
                GameManager.I.Resource.Load<Sprite>(
                    GameManager.I.Data.MaterialDataDict[(int)MaterialType.RandomScroll].SpriteName);
            
            item.UpdateUI(sprite, rewardGold.ToString());
        }
    }
}