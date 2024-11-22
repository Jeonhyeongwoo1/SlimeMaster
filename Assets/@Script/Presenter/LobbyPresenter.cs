using Script.InGame.UI;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Manager;
using SlimeMaster.Model;
using SlimeMaster.View;
using UniRx;

namespace SlimeMaster.Presenter
{
    public class LobbyPresenter : BasePresenter
    {
        private UI_LobbyScene _lobbySceneView;
        private UserModel _model;
        private CompositeDisposable _compositeDisposable;
        
        public void Initialize(UserModel model, UI_LobbyScene view)
        {
            _lobbySceneView = view;
            _model = model;

            _lobbySceneView.onGoodsClickAction = OnClickGoods;
            _lobbySceneView.onClickToggleAction = OnClickToggles;
            _lobbySceneView.Initialize();

            if (_compositeDisposable != null)
            {
                _compositeDisposable.Clear();
            }

            _compositeDisposable = new();
            var goldItemData = _model.GetItemData(Const.ID_GOLD);
            goldItemData.ItemValue.Subscribe((v) => _lobbySceneView.UpdateUserGoodsInfo(GoodsType.Gold, v.ToString()))
                .AddTo(_compositeDisposable);

            var diamondItemData = _model.GetItemData(Const.ID_DIA);
            diamondItemData.ItemValue.Subscribe((v) => _lobbySceneView.UpdateUserGoodsInfo(GoodsType.Dia, v.ToString()))
                .AddTo(_compositeDisposable);

            var staminaData = _model.GetItemData(Const.ID_STAMINA);
            staminaData.ItemValue.Subscribe((v) => _lobbySceneView.UpdateUserGoodsInfo(GoodsType.Stamina, v.ToString()))
                .AddTo(_compositeDisposable);
        }

        private void OnClickToggles(ToggleType selectedToggleType, ToggleType activatedToggleType)
        {
            UIManager uiManager = GameManager.I.UI;
            uiManager.ClosePopup();
            // switch (activatedToggleType)
            // {
            //     case ToggleType.BattleToggle:
            //         break;
            //     case ToggleType.EquipmentToggle:
            //         break;
            //     case ToggleType.ShopToggle:
            //         break;
            // }
            
            GameManager.I.Event.Raise(GameEventType.MoveToTap, selectedToggleType);
        }

        private void OnClickGoods(GoodsType goodsType)
        {
        }
    }
}