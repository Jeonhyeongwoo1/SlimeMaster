using System.Collections;
using System.Collections.Generic;
using Script.InGame.UI.Popup;
using SlimeMaster.Data;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Skill;
using SlimeMaster.InGame.View;
using SlimeMaster.Popup;
using SlimeMaster.UISubItemElement;
using SlimeMaster.View;
using Unity.VisualScripting;
using UnityEngine;

namespace SlimeMaster.Managers
{
    public class UIManager
    {
        public BaseUI SceneUI => _sceneUI;
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

            GameObject prefab = Manager.I.Resource.Instantiate($"{name}");
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

            GameObject prefab = Manager.I.Resource.Instantiate($"{name}");
            T element = prefab.GetOrAddComponent<T>();
            element.Initialize();
            element.transform.SetParent(parent);
            return element;
        }

        public T OpenPopup<T>(string name = null, bool isPool = true) where T : BasePopup
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }
            
            GameObject prefab = Manager.I.Resource.Instantiate($"{name}", isPool);
            T popup = prefab.GetOrAddComponent<T>();
            popup.transform.SetParent(UIRootObject.transform);
            popup.transform.SetAsLastSibling();
            
            var canvas = popup.GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
            popup.Initialize();
            popup.OpenPopup();
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