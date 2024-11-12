using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SlimeMaster.Data;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Manager;
using Unity.VisualScripting;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public abstract class Projectile : MonoBehaviour, IGeneratable
    {
        public Vector3 Velocity => _rigidbody.velocity;
        public Action<Collider2D, Projectile> OnHit { get; set; }
        public MonoBehaviour Mono => this;
        public Projectile ProjectileMono => this;

        public bool IsRelease { get; private set; }

        [SerializeField] protected Rigidbody2D _rigidbody;

        protected bool wantToSleepInTriggerEnter;

        public abstract void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData, CreatureController owner);
        public int Level { get; set; }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Tag.Monster))
            {
                OnHit?.Invoke(other, this);
                if (wantToSleepInTriggerEnter)
                {
                    Release();
                }
            }
        }

        private void OnEnable()
        {
            IsRelease = false;
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
        }

        public virtual void Release()
        {
            if (IsRelease)
            {
                return;
            }

            IsRelease = true;
            var go = gameObject;
            GameManager.I.Pool.ReleaseObject(go.name, go);
        }

        public virtual void OnChangedSkillData(SkillData skillData)
        {
            
        }

        protected async UniTaskVoid ApplyDamagedAsync(CancellationTokenSource cancellationTokenSource, Action damageAction)
        {
            float elapsed = 0;
            CancellationToken token = cancellationTokenSource.Token;
            float interval = 0.1f;
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                elapsed += interval;
                damageAction?.Invoke();
                
                try
                {
                    await UniTask.WaitForSeconds(interval, cancellationToken: token);
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"{nameof(ApplyDamagedAsync)} error {e.Message}");
                    break;
                }
            }
            
            Release();
        }
    }
}