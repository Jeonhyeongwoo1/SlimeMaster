using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.InGame.Entity;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Controller
{
    public class MonsterController : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private TriggerNotifier _triggerNotifier;
        
        private Rigidbody2D _rigidbody;
        private PlayerController _playerController;
        private CancellationTokenSource _takeDamageCts;
        private CancellationTokenSource _moveCts;
        private CreatureData _creatureData;
        
        public void Initialize(CreatureData creatureData, Sprite sprite)
        {
            var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            var triggerNotifier = GetComponentInChildren<TriggerNotifier>();
            
            _triggerNotifier = triggerNotifier;
            _spriteRenderer = spriteRenderer;
            _spriteRenderer.sprite = sprite;
            
            UpdateData(creatureData);
        }

        public void UpdateData(CreatureData creatureData)
        {
            _creatureData = creatureData;
        }

        private void Awake()
        {
            TryGetComponent(out _rigidbody);
            _triggerNotifier = GetComponentInChildren<TriggerNotifier>();
        }

        private void Start()
        {
            _triggerNotifier.AddEvent(OnEnter, OnExit);
        }

        private void OnEnable()
        {
            GameManager.I.Event.AddEvent(GameEventType.GameOver, OnGameEnd);
        }

        private void OnDisable()
        {
            if (GameManager.I)
            {
                GameManager.I?.Event?.RemoveEvent(GameEventType.GameOver, OnGameEnd);
            }
        }

        private void OnGameEnd(object value)
        {
            AllCancelCancellationTokenSource();
        }

        private void OnEnter(Collider2D collider2D)
        {
            if (collider2D.gameObject.layer != _playerController.Layer)
            {
                return;
            }
            
            TakeDamageAsync().Forget();
        }

        private void OnExit(Collider2D collider2D)
        {
            Utils.SafeCancelCancellationTokenSource(ref _takeDamageCts);
        }

        private async UniTaskVoid TakeDamageAsync()
        {
            _takeDamageCts = new CancellationTokenSource();
            CancellationToken token = _takeDamageCts.Token;

            while (true)
            {
                Debug.Log("TakeDmager :" + _creatureData.Atk);
                _playerController.TakeDamage(_creatureData.Atk);
                
                try
                {
                    await UniTask.WaitForSeconds(0.1f, cancellationToken: token);
                }
                catch (Exception e) when(!(e is OperationCanceledException))
                {
                    Debug.LogError($"{nameof(TakeDamageAsync)} error {e}");
                    break;
                }
            }
        }

        private async UniTaskVoid MoveToPlayer()
        {
            _moveCts = new CancellationTokenSource();
            CancellationToken token = _moveCts.Token;
            
            while (true)
            {
                Vector3 prevPosition = _rigidbody.position;
                Vector3 direction = (_playerController.transform.position - prevPosition).normalized;
                Vector3 nextPosition = (Vector3) _rigidbody.position + direction;
                Vector3 lerp = Vector3.Lerp(prevPosition, nextPosition, Time.fixedDeltaTime * _creatureData.MoveSpeed);
                _rigidbody.MovePosition(lerp);
                _spriteRenderer.flipX = direction.x >= 0;
                
                try
                {
                    await UniTask.WaitForFixedUpdate(cancellationToken: token);
                }
                catch (Exception e) when(!(e is OperationCanceledException))
                {
                    Debug.LogError($"{nameof(MoveToPlayer)} error {e}");
                    Dead();
                    break;
                }
            }
        }

        public void Dead()
        {
            AllCancelCancellationTokenSource();
        }

        private void AllCancelCancellationTokenSource()
        {
            Utils.SafeCancelCancellationTokenSource(ref _takeDamageCts);
            Utils.SafeCancelCancellationTokenSource(ref _moveCts);
        }

        public void Spawn(Vector3 spawnPosition, PlayerController playerController)
        {
            transform.position = spawnPosition;
            _playerController = playerController;
            gameObject.SetActive(true);
            MoveToPlayer().Forget();
        }
    }
}