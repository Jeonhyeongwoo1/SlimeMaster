using SlimeMaster.Factory;
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
