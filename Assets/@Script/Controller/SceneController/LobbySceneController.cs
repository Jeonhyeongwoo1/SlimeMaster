using Script.InGame.UI;
using SlimeMaster.Factory;
using SlimeMaster.InGame.Manager;
using SlimeMaster.Presenter;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SlimeMaster.Controller
{
    public class LobbySceneController : MonoBehaviour
    {
        private void Start()
        {
            GameManager gameManager = GameManager.I;
            var lobbyUI = gameManager.UI.ShowUI<UI_LobbyScene>();

            var lobbyPresenter = PresenterFactory.CreateOrGet<LobbyPresenter>();
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            lobbyPresenter.Initialize(userModel, lobbyUI);

            var battlePresenter = PresenterFactory.CreateOrGet<BattlePopupPresenter>();
            battlePresenter.Initialize(userModel);
            battlePresenter.OpenBattlePopup();
        }
    }
}