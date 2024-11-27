using SlimeMaster.Enum;
using SlimeMaster.Managers;
using SlimeMaster.Popup;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;

namespace SlimeMaster.InGame.Popup
{
    public class UI_BackToHomePopup : BasePopup
    {
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _resumeButton;

        private void Start()
        {
            _quitButton.OnPointerClickAsObservable().Subscribe(x => OnQuit()).AddTo(this);
            _resumeButton.OnPointerClickAsObservable().Subscribe(x => OnResumeGame()).AddTo(this);
        }

        private void OnResumeGame()
        {
            Managers.Manager.I.UI.ClosePopup();
        }

        private void OnQuit()
        {
            SceneManager.LoadScene(SceneType.LobbyScene.ToString());
        }
    }
}