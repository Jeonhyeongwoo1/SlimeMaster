using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlimeMaster.UISubItemElement
{
    public abstract class UI_SubItemElement : MonoBehaviour
    {
        public bool IsInitialized { get; protected set; }

        public virtual void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            IsInitialized = true;
        }
    }
}