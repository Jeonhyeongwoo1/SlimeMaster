using System.Collections.Generic;
using Script.InGame.UI;
using SlimeMaster.Factory;
using SlimeMaster.InGame.Data;
using SlimeMaster.Presenter;
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

        private void OnClickGoods(GoodsType goodsType)
        {
            
        }
    }
}