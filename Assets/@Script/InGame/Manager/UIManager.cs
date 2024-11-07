using System.Collections;
using System.Collections.Generic;
using SlimeMaster.InGame.View;
using SlimeMaster.Popup;
using SlimeMaster.UISubItemElement;
using SlimeMaster.View;
using Unity.VisualScripting;
using UnityEngine;

namespace SlimeMaster.InGame.Manager
{
    public class UIManager
    {
        private BaseUI _sceneUI;

        private GameObject UIRootObject
        {
            get
            {
                if (_uiRootObject == null)
                {
                    _uiRootObject = GameObject.Find("@UI_Root");
                }

                return _uiRootObject;
            }
        }

        private GameObject _uiRootObject;
        private Stack<BasePopup> _popupStack = new();

        public T ShowUI<T>(string name = null) where T : BaseUI
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }

            GameObject prefab = GameManager.I.Resource.Instantiate($"{name}");
            T ui = prefab.GetOrAddComponent<T>();
            _sceneUI = ui;
            ui.transform.SetParent(UIRootObject.transform);
            
            var canvas = ui.GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
            ui.Initialize();
            ui.gameObject.SetActive(true);

            return ui;
        }

        public T AddSubElementItem<T>(Transform parent, string name = null) where T : UI_SubItemElement
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }

            GameObject prefab = GameManager.I.Resource.Instantiate($"{name}");
            T element = prefab.GetOrAddComponent<T>();
            element.transform.SetParent(parent);
            return element;
        }

        public T OpenPopup<T>(string name = null) where T : BasePopup
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }
            
            GameObject prefab = GameManager.I.Resource.Instantiate($"{name}");
            T popup = prefab.GetOrAddComponent<T>();
            popup.transform.SetParent(UIRootObject.transform);
            popup.transform.SetAsLastSibling();
            
            var canvas = popup.GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
            popup.Initialize();
            _popupStack.Push(popup);
            return popup;
        }

        public void ClosePopup()
        {
            BasePopup popup = _popupStack.Pop();
            popup.ClosePopup();
        }
        
    }
}