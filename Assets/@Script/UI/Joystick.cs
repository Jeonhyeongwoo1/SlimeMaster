using System;
using System.Collections;
using SlimeMaster.InGame.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SlimeMaster.InGame.View
{
    public class Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform _bgRectTransform;
        [SerializeField] private RectTransform _handlerRectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;

        private Vector2 _inputVector;
        private Coroutine _fadeCor;

        private void OnEnable()
        {
            InputHandler.onActivateInputHandlerAction += OnActivate;
            
            if (_fadeCor != null)
            {
                StopCoroutine(_fadeCor);
            }
        }

        private void OnDisable()
        {
            InputHandler.onActivateInputHandlerAction -= OnActivate;
            
            if (_fadeCor != null)
            {
                StopCoroutine(_fadeCor);
            }
        }

        private void OnActivate(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
        
        // 드래그 시 호출
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 pos;
            // 배경 안에서 핸들의 위치 계산
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _bgRectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out pos
            );

            pos.x = (pos.x / _bgRectTransform.sizeDelta.x);
            pos.y = (pos.y / _bgRectTransform.sizeDelta.y);

            // _handlerRectTransform.anchoredPosition = pos;
            _inputVector = new Vector2(pos.x * 2, pos.y * 2);
            _inputVector = (_inputVector.magnitude > 1.0f) ? _inputVector.normalized : _inputVector;

            // Debug.Log(_inputVector);
            _handlerRectTransform.anchoredPosition = _bgRectTransform.anchoredPosition + new Vector2(
                _inputVector.x * (_bgRectTransform.sizeDelta.x / 2),
                _inputVector.y * (_bgRectTransform.sizeDelta.y / 2)
            );

            InputHandler.onInputAction?.Invoke(_inputVector);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _bgRectTransform.position = eventData.position;
            OnDrag(eventData);
            
            if (_fadeCor != null)
            {
                StopCoroutine(_fadeCor);
            }
            
            Debug.LogWarning("OnpointerDown");
            _fadeCor = StartCoroutine(DoFadeCor(true, 0.1f));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_fadeCor != null)
            {
                StopCoroutine(_fadeCor);
            }
            
            _fadeCor = StartCoroutine(DoFadeCor(false, 0.3f));
            // InputHandler.onPointerUpAction?.Invoke(Vector2.zero);
            InputHandler.onInputAction?.Invoke(Vector2.zero);
        }

        private IEnumerator DoFadeCor(bool isFadeOut, float duration)
        {
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
             
                float endValue = isFadeOut ? 1 : 0;
                float starValue = isFadeOut ? 0 : 1;

                _canvasGroup.alpha = Mathf.Lerp(starValue, endValue, elapsed / duration);
                yield return null;
            }
        }
    }
}
















