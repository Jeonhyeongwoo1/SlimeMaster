using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.Managers;
using UnityEngine;

//죽었을 때, 새로운 웨이브가 시작되었을때 스폰
namespace SlimeMaster.InGame.Controller
{
    public abstract class DropItemController : MonoBehaviour
    {
        public DropableItemType DropableItemType => dropableItemType;
        
        [SerializeField] protected DropableItemType dropableItemType;
        [SerializeField] protected SpriteRenderer _spriteRenderer;

        protected bool IsRelease { get; private set; }
        
        private DropItemData _dropItemData;
        private CancellationTokenSource _moveToTargetCts;
        
        //스폰, 먹었을 때 처리
        public virtual void Spawn(Vector3 spawnPosition)
        {
            transform.position = spawnPosition;
            IsRelease = false;
            gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            Utils.SafeCancelCancellationTokenSource(ref _moveToTargetCts);
        }

        public void SetInfo(DropItemData dropItemData)
        {
            _dropItemData = dropItemData;
        }

        public virtual void Release()
        {
            if (IsRelease)
            {
                return;
            }
            
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
                case DropableItemType.Soul:
                    name = Const.Soul;
                    break;
                default:
                        return;
            }
            
            IsRelease = true;
            Utils.SafeCancelCancellationTokenSource(ref _moveToTargetCts);
            Managers.Manager.I.Pool.ReleaseObject(name, gameObject);
        }

        public virtual void GetItem(Transform target, Action callback = null)
        {
            if (_moveToTargetCts != null)
            {
                return;
            }
            
            Utils.SafeCancelCancellationTokenSource(ref _moveToTargetCts);
            MoveToTargetAsync(target, callback).Forget();
        }
        
        protected async UniTaskVoid MoveToTargetAsync(Transform target, Action callback)
        {
            if (IsRelease)
            {
                return;
            }
            
            _moveToTargetCts = new CancellationTokenSource();
            CancellationToken token = _moveToTargetCts.Token;
            // float elapsed = 0;
            // float duration = 1f;
            // float step = 1f / (duration / Time.deltaTime);
            // Vector3 startPosition = transform.position;
            float speed = DropableItemType == DropableItemType.Soul ? 100 : 15;
            while (!IsRelease)
            {
                // elapsed += step;
                // float t = Mathf.SmoothStep(0, 1, elapsed);
                Vector3 position = transform.position;
                transform.position = Vector3.MoveTowards(position, target.position, Time.deltaTime * speed);

                if (Vector2.Distance(transform.position, target.position) < 1.5f)
                {
                    break;
                }
                
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
                Managers.Manager.I.Event.Raise(GameEventType.ActivateDropItem, _dropItemData);
            }
        }
    }
}