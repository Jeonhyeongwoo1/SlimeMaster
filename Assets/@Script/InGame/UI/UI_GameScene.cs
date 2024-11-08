using SlimeMaster.Factory;
using SlimeMaster.InGame.Enum;
using SlimeMaster.Model;
using SlimeMaster.View;
using TMPro;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

namespace SlimeMaster.InGame.View
{
    public class UI_GameScene : BaseUI
    {
        [SerializeField] private UI_GameStageInfoPanel uiGameStageInfoPanel;
        [SerializeField] private Slider _playerExpSlider;
        [SerializeField] private TextMeshProUGUI _playerLvelText;
        [SerializeField] private UI_MonsterInfo _eliteMonsterInfo;
        [SerializeField] private UI_MonsterInfo _bossMonsterInfo;
        
        public override void Initialize()
        {
            if (IsInitialize)
            {
                return;
            }
            
            uiGameStageInfoPanel.Initialize(OnPauseGame);

            var playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            playerModel.CurrentExpRatio
                .Subscribe(OnChangedPlayerExp)
                .AddTo(this);

            playerModel.CurrentLevel
                .Subscribe(OnChangedCurrentLevel)
                .AddTo(this);
    
            IsInitialize = true;
        }
        
        public void ShowMonsterInfo(MonsterType monsterType, string monsterName, float ratio)
        {
            if (monsterType == MonsterType.Boss)
            {
                _bossMonsterInfo.gameObject.SetActive(true);
                _bossMonsterInfo.UpdateMonsterInfo(monsterName, ratio);
            }
            else if(monsterType == MonsterType.Elete)
            {
                _eliteMonsterInfo.gameObject.SetActive(true);
                _eliteMonsterInfo.UpdateMonsterInfo(monsterName, ratio);
            }
        }

        public void HideMonsterInfo(MonsterType monsterType)
        {
            if (monsterType == MonsterType.Boss)
            {
                _bossMonsterInfo.gameObject.SetActive(false);
            }else if (monsterType == MonsterType.Elete)
            {
                _eliteMonsterInfo.gameObject.SetActive(false);
            }
        }

        private void OnChangedCurrentLevel(int value)
        {
            _playerLvelText.text = value.ToString();
        }

        private void OnChangedPlayerExp(float value)
        {
            _playerExpSlider.value = value;
        }

        private void OnPauseGame()
        {
            
        }
    }
}
