using SlimeMaster.Manager;
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

        public virtual void Release()
        {
            GameManager.I.Pool.ReleaseObject(gameObject.name, gameObject);
        }
    }
}