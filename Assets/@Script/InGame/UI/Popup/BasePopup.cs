using System;
using System.Collections;
using System.Collections.Generic;
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
                SafeButtonAddListener(ref _bgCloseButton, ClosePopup);
            }
            
            IsInitialize = true;
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
            gameObject.SetActive(false);
        }
    }
}