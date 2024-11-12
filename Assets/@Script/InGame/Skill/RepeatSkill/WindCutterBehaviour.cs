using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SlimeMaster.Data;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Skill;
using UnityEngine;

namespace SlimeMaster.InGame.Entity
{
    public class WindCutterBehaviour : Projectile
    {
        private Vector3 _destinationPosition;
        private Sequence _sequence;
        private List<Collider2D> _monsterColliderList = new();
        private CancellationTokenSource _applyDamagedCts = new();
        
        public override void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData, CreatureController owner)
        {
            _monsterColliderList.Clear();
            transform.position = spawnPosition;
            _destinationPosition = owner.Position + direction * skillData.ProjSpeed;
            gameObject.SetActive(true);

            transform.localScale = Vector3.zero;
            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DOScale(skillData.ScaleMultiplier, 0.4f));
            
            _rigidbody.velocity = direction * (skillData.ProjSpeed * 2);
            ApplyDamagedAsync(_applyDamagedCts, () => _monsterColliderList.ForEach(v => OnHit?.Invoke(v, this)))
                .Forget();
        }

        public async UniTaskVoid OnReturnToOwner(CreatureController owner, float duration)
        {
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float time = elapsed / duration;
                Vector3 prevPosition = transform.position;
                Vector3 prevScale = transform.localScale;
                transform.position = Vector3.Lerp(prevPosition, owner.Position, time);
                transform.localScale = Vector3.Lerp(prevScale, Vector3.zero , time);
                await UniTask.Yield();
            }
            
            Release();
        }

        private void Update()
        {
            float distance = Vector3.Distance(transform.position, _destinationPosition);
            if (distance < 0.3f)
            {
                _rigidbody.velocity = Vector3.zero;
            }
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Tag.Monster))
            {
                _monsterColliderList.Add(other);
            }
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(Tag.Monster))
            {
                if (_monsterColliderList.Contains(other))
                {
                    _monsterColliderList.Remove(other);
                }
            }
        }
        
        public override void Release()
        {
            base.Release();

            if (_sequence != null)
            {
                _sequence.Kill();
            }
        }
    }
}