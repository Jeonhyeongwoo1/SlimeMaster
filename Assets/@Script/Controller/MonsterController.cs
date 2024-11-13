using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SlimeMaster.InGame.Controller
{
    public class MonsterController : CreatureController
    {
        public MonsterType MonsterType => _monsterType;
        
        [SerializeField] private MonsterType _monsterType;
        
        protected PlayerController _player;
        
        private CancellationTokenSource _takeDamageCts;
        private CancellationTokenSource _moveCts;

        public override void Initialize(CreatureData creatureData, Sprite sprite, List<SkillData> skillDataList)
        {
            base.Initialize(creatureData, sprite, skillDataList);

            _creatureType = CreatureType.Monster;
            if (skillDataList != null && skillDataList.Count > 0)
            {
                _skillBook.UseAllSkillList(true, true, GameManager.I.Object.Player);
            }

            HP = 1;
        }

        private void Awake()
        {
            TryGetComponent(out _rigidbody);
        }

        private void Start()
        {
        }

        protected virtual void OnEnable()
        {
            GameManager.I.Event.AddEvent(GameEventType.DeadPlayer, OnDeadPlayer);
            GameManager.I.Event.AddEvent(GameEventType.ResurrectionPlayer, OnResurrectionPlayer);
        }

        protected virtual void OnDisable()
        {
            GameManager.I.Event.RemoveEvent(GameEventType.DeadPlayer, OnDeadPlayer);
            GameManager.I.Event.RemoveEvent(GameEventType.ResurrectionPlayer, OnResurrectionPlayer);
           
            AllCancelCancellationTokenSource();
        }

        private void OnDeadPlayer(object value)
        {
            AllCancelCancellationTokenSource();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collider2D)
        {
            if (collider2D.gameObject.layer != _player.Layer)
            {
                return;
            }
            
            TakeDamageAsync().Forget();
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            Utils.SafeCancelCancellationTokenSource(ref _takeDamageCts);
        }

        private async UniTaskVoid TakeDamageAsync()
        {
            _takeDamageCts = new CancellationTokenSource();
            CancellationToken token = _takeDamageCts.Token;

            while (true)
            {
                _player.TakeDamage(_creatureData.Atk, this);
                
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
            
            while (_moveCts != null && !_moveCts.IsCancellationRequested)
            {
                Vector3 prevPosition = _rigidbody.position;
                Vector3 direction = (_player.transform.position - prevPosition).normalized;
                Vector3 nextPosition = (Vector3) _rigidbody.position + direction;
                Vector3 lerp = Vector3.Lerp(prevPosition, nextPosition, Time.fixedDeltaTime * _moveSpeed);
                _rigidbody.MovePosition(lerp);
                SetSpriteFlipX(direction.x >= 0);
                
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

        private void OnResurrectionPlayer(object value)
        {
            MoveToPlayer().Forget();
        }

        private void AllCancelCancellationTokenSource()
        {
            Utils.SafeCancelCancellationTokenSource(ref _takeDamageCts);
            Utils.SafeCancelCancellationTokenSource(ref _moveCts);
        }

        public virtual void Spawn(Vector3 spawnPosition, PlayerController player)
        {
            transform.position = spawnPosition;
            _player = player;
            gameObject.SetActive(true);
            MoveToPlayer().Forget();
        }

        public override void TakeDamage(float damage, CreatureController attacker)
        {
            if (IsDeadState)
            {
                return;
            }

            bool isCritical = false;
            if (attacker is PlayerController player)
            {
                if (player != null)
                {
                    float ratio = Random.value;
                    if (ratio < player.CriRate)
                    {
                        damage *= player.CriticalDamage;
                        isCritical = true;
                    }
                }
            }
            
            base.TakeDamage(damage, attacker);
            GameManager.I.Object.ShowDamageFont(Position, damage, 0, transform, isCritical);
        }

        protected override async void Dead()
        {
            if (CreatureStateType.Dead == _creatureStateType)
            {
                return;
            }
            
            base.Dead();
            await DeadAnimation();
            
            HP = 0;
            UpdateCreatureState(CreatureStateType.Dead);
            Debug.Log($"Dead {transform.GetInstanceID()}");
            GameManager.I.Event.Raise(GameEventType.DeadMonster, this);
            AllCancelCancellationTokenSource();
        }

        public void ForceKill()
        {
            Dead();
        }
    }
}