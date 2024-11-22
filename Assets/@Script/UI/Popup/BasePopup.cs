using SlimeMaster.Common;
using SlimeMaster.Manager;
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
                _bgCloseButton.SafeAddButtonListener(ClosePopup);
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

        public virtual void OpenPopup()
        {
            gameObject.SetActive(true);
        }

        public virtual void ClosePopup()
        {
            Debug.Log("ClosePopup");
            GameManager.I.Pool.ReleaseObject(gameObject.name, gameObject);
        }
    }
}