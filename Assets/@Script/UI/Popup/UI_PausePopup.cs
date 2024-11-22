using SlimeMaster.Manager;
using SlimeMaster.Popup;
using UniRx.Triggers;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

namespace SlimeMaster.InGame.Popup
{
    public class UI_PausePopup : BasePopup
    {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _goHomeButton;
        [SerializeField] private Button _soundButton;
        [SerializeField] private Button _settingButton;
        [SerializeField] private Button _statisticsButton;

        private void Start()
        {
            _resumeButton.OnPointerClickAsObservable().Subscribe((v) => OnResumeGame()).AddTo(this);
            _goHomeButton.OnPointerClickAsObservable().Subscribe(x => OnGoHomeButton()).AddTo(this);
            _soundButton.OnPointerClickAsObservable().Subscribe(x => OnActivateSound()).AddTo(this);
            _statisticsButton.OnPointerClickAsObservable().Subscribe(x => OnShowStatisticsPopup()).AddTo(this);
            _settingButton.OnPointerClickAsObservable().Subscribe(x => OnShowSettingPopup()).AddTo(this);
        }

        private void OnShowStatisticsPopup()
        {
            GameManager.I.UI.OpenPopup<UI_TotalDamagePopup>();
        }

        private void OnShowSettingPopup()
        {
            
        }

        private void OnActivateSound()
        {
            
        }

        private void OnResumeGame()
        {
            Time.timeScale = 1;
            GameManager.I.UI.ClosePopup();
        }

        private void OnGoHomeButton()
        {
            GameManager.I.UI.OpenPopup<UI_BackToHomePopup>();
        }
    }
}