using SlimeMaster.Enum;
using SlimeMaster.Manager;
using SlimeMaster.Model;
using SlimeMaster.OutGame.Popup;

namespace SlimeMaster.Presenter
{
    public class CheckOutPopupPresenter : BasePresenter
    {
        private UserModel _userModel;
        
        public void Initialize(UserModel model)
        {
            _userModel = model;
            GameManager.I.Event.AddEvent(GameEventType.ShowOutGameContentPopup, OnOpenCheckoutPopup);
        }

        private void OnOpenCheckoutPopup(object value)
        {
            var buttonType = (OutGameContentButtonType)value;
            if (buttonType != OutGameContentButtonType.Checkout)
            {
                return;
            }

            var popup = GameManager.I.UI.OpenPopup<UI_CheckOutPopup>();

            DataManager dataManager = GameManager.I.Data;
            dataManager.
        }
    }
}