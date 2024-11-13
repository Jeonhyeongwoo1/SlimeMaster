using Script.InGame.UI;
using SlimeMaster.Factory;
using SlimeMaster.View;
using UniRx;

namespace SlimeMaster.InGame.Controller
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
            _model.Diamond
                .Subscribe((v) => _lobbySceneView.UpdateUserGoodsInfo(GoodsType.Dia, v.ToString()))
                .AddTo(_compositeDisposable);
            _model.Gold
                .Subscribe((v) => _lobbySceneView.UpdateUserGoodsInfo(GoodsType.Gold, v.ToString()))
                .AddTo(_compositeDisposable);
            _model.Stamina
                .Subscribe((v) => _lobbySceneView.UpdateUserGoodsInfo(GoodsType.Stamina, v.ToString()))
                .AddTo(_compositeDisposable);
        }

        private void OnClickGoods(GoodsType goodsType)
        {
            
        }
    }
}