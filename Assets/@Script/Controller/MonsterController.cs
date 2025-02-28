using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.Enum;
using SlimeMaster.InGame.Manager;
using SlimeMaster.Managers;
using UnityEngine;
using UnityEngine.Serialization;
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
                _skillBook.UseAllSkillList(true, true, Managers.Manager.I.Object.Player);
            }
        }

        private void Awake()
        {
            TryGetComponent(out _rigidbody);
        }

        protected virtual void OnEnable()
        {
            Managers.Manager.I.Event.AddEvent(GameEventType.DeadPlayer, OnDeadPlayer);
            Managers.Manager.I.Event.AddEvent(GameEventType.ResurrectionPlayer, OnResurrectionPlayer);
        }

        protected override void OnDisable()
        {
            Utils.SafeCancelCancellationTokenSource(ref _takeDamageCts);
            Utils.SafeCancelCancellationTokenSource(ref _moveCts);
            Managers.Manager.I.Event.RemoveEvent(GameEventType.DeadPlayer, OnDeadPlayer);
            Managers.Manager.I.Event.RemoveEvent(GameEventType.ResurrectionPlayer, OnResurrectionPlayer);
           
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

        private Vector3 _cellPos;

        private async UniTaskVoid FindPathToPlayer()
        {
            _moveCts = new CancellationTokenSource();
            CancellationToken token = _moveCts.Token;

            GameManager game = Managers.Manager.I.Game;
            float minSpeed = 1.5f;
            while (_moveCts != null && !_moveCts.IsCancellationRequested)
            {
                Vector3 myPos = transform.position;
                Vector3 playerPos = _player.Position;
                //일정거리 밑으로인경우에는 방향으로 밀기
                if ((myPos - playerPos).sqrMagnitude < 1.5f)
                {
                    Vector3 dir = (playerPos - myPos).normalized;
                    transform.position = Vector3.Lerp(myPos, myPos + dir, Time.deltaTime * minSpeed);
                    SetSpriteFlipX(dir.x >= 0);
                    await UniTask.Yield(cancellationToken: token);
                    continue;
                }

                // List<Vector3Int> list = new List<Vector3Int>();
                var list = game.PathFinding(game.WorldToCell(myPos), game.WorldToCell(playerPos));
                if (list == null || list.Count < 2)
                {
                    Vector3 dir = (playerPos - myPos).normalized;
                    transform.position = Vector3.Lerp(myPos, myPos + dir, Time.deltaTime * minSpeed);
                }
                else
                {
                    var pathQueue = new Queue<Vector3Int>(list);
                    // _pathQueue.Dequeue();
                    while (pathQueue.Count > 0)
                    {
                        var pos = pathQueue.Dequeue();
                        _cellPos = game.CellToWorld(pos);

                        //이전과의 거리 차이가 많이 난다면 종료 후에 다시 길 찾기
                        if ((game.WorldToCell(playerPos) - game.WorldToCell(_player.transform.position)).magnitude > 1)
                        {
                            break;
                        }
                    
                        while ((transform.position - _cellPos).sqrMagnitude >= 1f)
                        {
                            Vector3 dir = _cellPos - transform.position;
                            //일관성 있게 이동하기 위해서
                            float moveDist = Mathf.Min(dir.magnitude, MoveSpeed * Time.deltaTime);
                            dir.Normalize();
                            transform.position += dir * moveDist;
                            SetSpriteFlipX(dir.x >= 0);
                            await UniTask.Yield();
                        }
                    }
                }


                await UniTask.Yield();
            }
        }

        private void OnResurrectionPlayer(object value)
        {
            FindPathToPlayer().Forget();
        }

        private void AllCancelCancellationTokenSource()
        {
            Utils.SafeCancelCancellationTokenSource(ref _takeDamageCts);
            Utils.SafeCancelCancellationTokenSource(ref _moveCts);
        }

        public virtual void Spawn(Vector3 spawnPosition, PlayerController player)
        {
            transform.position = spawnPosition;
            _cellPos = Managers.Manager.I.Game.WorldToCell(spawnPosition);
            _player = player;
            gameObject.SetActive(true);
            FindPathToPlayer().Forget();
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
            Managers.Manager.I.Object.ShowDamageFont(Position, damage, 0, transform, isCritical);
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
            Managers.Manager.I.Event.Raise(GameEventType.DeadMonster, this);
            AllCancelCancellationTokenSource();
        }

        public void ForceKill()
        {
            Dead();
        }
    }
}