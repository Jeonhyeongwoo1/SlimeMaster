using SlimeMaster.Common;
using SlimeMaster.Enum;
using SlimeMaster.Managers;
using SlimeMaster.UISubItemElement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SlimeMaster.Popup
{
    public abstract class BasePopup : MonoBehaviour
    {
        [SerializeField] protected Button _bgCloseButton;
        
        public bool IsInitialize { get; protected set; }

        public virtual void Initialize()
        {
            if (IsInitialize)
            {
                return;
            }
         
            if (_bgCloseButton != null)
            {
                _bgCloseButton.SafeAddButtonListener(()=> Manager.I.UI.ClosePopup());
            }
            IsInitialize = true;
        }

        public virtual void AddEvents()
        {
            
        }

        protected void SafeButtonAddListener(ref Button button, UnityAction callback)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(callback);
        }

        public void ReleaseSubItem<T>(Transform parent) where T : UI_SubItemElement
        {
            var childs = Utils.GetChildComponent<T>(parent);
            if (childs == null)
            {
                return;
            }
            
            foreach (T subItem in childs)
            {
                subItem.Release();
            }
        }

        public virtual void OpenPopup()
        {
            gameObject.SetActive(true);
        }

        public virtual void ClosePopup()
        {
            PlayPopupCommonCloseSound();
            Manager.I.Pool.ReleaseObject(gameObject.name, gameObject);
        }

        private void PlayPopupCommonCloseSound()
        {
            Manager.I.Audio.Play(Sound.Effect, "PopupClose_Common");
        }
    }
}