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
    public class MonsterController : CreatureController
    {
        [SerializeField] private int _instanceId;
        private PlayerController _playerController;
        private CancellationTokenSource _takeDamageCts;
        private CancellationTokenSource _moveCts;
        
        public void Initialize(CreatureData creatureData, Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
            _instanceId = transform.GetInstanceID();
            UpdateData(creatureData);
            IsDead = false;
        }

        public void UpdateData(CreatureData creatureData)
        {
            _creatureData = creatureData;
            _currentHp = creatureData.MaxHp;
        }

        private void Awake()
        {
            TryGetComponent(out _rigidbody);
        }

        private void Start()
        {
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
            
            AllCancelCancellationTokenSource();
        }

        private void OnGameEnd(object value)
        {
            AllCancelCancellationTokenSource();
        }

        private void OnTriggerEnter2D(Collider2D collider2D)
        {
            if (collider2D.gameObject.layer != _playerController.Layer)
            {
                return;
            }
            
            TakeDamageAsync().Forget();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Utils.SafeCancelCancellationTokenSource(ref _takeDamageCts);
        }

        private async UniTaskVoid TakeDamageAsync()
        {
            _takeDamageCts = new CancellationTokenSource();
            CancellationToken token = _takeDamageCts.Token;

            while (true)
            {
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

        private void AllCancelCancellationTokenSource()
        {
            Utils.SafeCancelCancellationTokenSource(ref _takeDamageCts);
            Utils.SafeCancelCancellationTokenSource(ref _moveCts);
        }

        public void Spawn(Vector3 spawnPosition, PlayerController playerController)
        {
            transform.position = spawnPosition;
            _playerController = playerController;
            transform.localScale = Vector3.one;
            gameObject.SetActive(true);
            MoveToPlayer().Forget();
        }

        public override void TakeDamage(float damage)
        {
            if (IsDead)
            {
                return;
            }
            
            _currentHp -= damage;
            if (_currentHp <= 0)
            {
                Dead();
            }
            
            base.TakeDamage(damage);
        }

        protected override async void Dead()
        {
            base.Dead();
            await DeadAnimation();
            
            _currentHp = 0;
            GameManager.I.Event.Raise(GameEventType.DeadMonster, this);
            AllCancelCancellationTokenSource();
        }
    }
}