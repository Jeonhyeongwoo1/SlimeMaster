using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Manager;
using UnityEngine;

//죽었을 때, 새로운 웨이브가 시작되었을때 스폰
namespace SlimeMaster.InGame.Controller
{
    public abstract class DropItemController : MonoBehaviour
    {
        public DropableItemType DropableItemType => dropableItemType;
        
        [SerializeField] protected DropableItemType dropableItemType;
        [SerializeField] protected SpriteRenderer _spriteRenderer;

        private DropItemData _dropItemData;
        private CancellationTokenSource _moveToTargetCts;
        
        //스폰, 먹었을 때 처리
        public virtual void Spawn(Vector3 spawnPosition)
        {
            transform.position = spawnPosition;
            gameObject.SetActive(true);
        }

        public void SetInfo(DropItemData dropItemData)
        {
            _dropItemData = dropItemData;
        }

        public virtual void Release()
        {
            Utils.SafeCancelCancellationTokenSource(ref _moveToTargetCts);

            string name = null;
            switch (dropableItemType)
            {
                case DropableItemType.Potion:
                case DropableItemType.Magnet:
                case DropableItemType.DropBox:
                case DropableItemType.Bomb:
                    name = dropableItemType.ToString();
                    break;
                case DropableItemType.Gem:
                    name = Const.ExpGem;
                    break;
                default:
                        return;
            }

            GameManager.I.Pool.ReleaseObject(name, gameObject);
            gameObject.SetActive(false);
        }

        public virtual void GetItem(Transform target, Action callback = null)
        {
            Utils.SafeCancelCancellationTokenSource(ref _moveToTargetCts);
            MoveToTargetAsync(target, callback).Forget();
        }

        protected async UniTaskVoid MoveToTargetAsync(Transform target, Action callback)
        {
            _moveToTargetCts = new CancellationTokenSource();
            CancellationToken token = _moveToTargetCts.Token;
            float elapsed = 0;
            float duration = 0.5f;
            float step = 1f / (duration / Time.deltaTime);
            Vector3 startPosition = transform.position;
            while (elapsed < 1f)
            {
                elapsed += step;
                float t = Mathf.SmoothStep(0, 1, elapsed);
                transform.position = Vector3.Lerp(startPosition, target.position, t);

                try
                {
                    await UniTask.Yield(cancellationToken: token);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    Debug.LogError($"{nameof(MoveToTargetAsync)} error {e.Message}");
                }
            }
            
            CompletedGetItem();
            callback?.Invoke();
        }
        
        private void CompletedGetItem()
        {
            Release();
            
            if (_dropItemData != null)
            {
                GameManager.I.Event.Raise(GameEventType.ActivateDropItem, _dropItemData);
            }
        }
    }
}