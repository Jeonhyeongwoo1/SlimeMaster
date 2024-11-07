using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlimeMaster.Popup
{
    public abstract class BasePopup : MonoBehaviour
    {
        public bool IsInitialize { get; protected set; }

        public virtual void Initialize()
        {
            if (IsInitialize)
            {
                return;
            }

            IsInitialize = true;
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