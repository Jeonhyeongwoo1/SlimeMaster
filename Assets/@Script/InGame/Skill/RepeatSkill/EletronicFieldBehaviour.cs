using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Skill;
using UnityEngine;

namespace SlimeMaster.InGame.Entity
{
    public class EletronicFieldBehaviour : Projectile
    {
        private CreatureController _owner;
        private bool _isActivated;
        private List<Collider2D> _monsterColliderList = new();
        private CancellationTokenSource _applyDamagedCts = new();

        [SerializeField] private GameObject _maxLevelParticleObject;
        [SerializeField] private GameObject _normalLevelParticleObject;
        
        public override void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData, CreatureController owner)
        {
            transform.position = spawnPosition;
            _owner = owner;

            if (!_isActivated)
            {
                transform.localScale = Vector3.zero;
            }

            bool isMaxLevel = Level == Const.MAX_SKILL_Level;
            _normalLevelParticleObject.SetActive(!isMaxLevel);
            _maxLevelParticleObject.SetActive(isMaxLevel);
            _monsterColliderList.Clear();
            gameObject.SetActive(true);
            transform.DOKill();
            transform.DOScale(Vector3.one * skillData.ScaleMultiplier, 0.5f)
                .OnComplete(() =>
                {
                    _isActivated = true;
                    _applyDamagedCts = new CancellationTokenSource();
                    ApplyDamagedAsync(_applyDamagedCts, () => _monsterColliderList.ForEach(v => OnHit?.Invoke(v, this)))
                        .Forget();
                });
        }

        private void Update()
        {
            transform.position = _owner.transform.position + Vector3.up;
        }

        private void OnDisable()
        {
            Sleep();
        }

        public override void Sleep()
        {
            base.Sleep();
            Utils.SafeCancelCancellationTokenSource(ref _applyDamagedCts);
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
    }
}