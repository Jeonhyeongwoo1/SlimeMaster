using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Script.InGame.Skill;
using SlimeMaster.Data;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class PoisonFieldBehaviour : Projectile
    {
        [SerializeField] private GameObject _posionFieldEffectObject;
        
        private List<Collider2D> _monsterColliderList = new();
        private CancellationTokenSource _applyDamagedCts = new();
        
        public override void Generate(Vector3 spawnPosition, Vector3 direction, SkillData skillData, CreatureController owner)
        {
            transform.position = spawnPosition;
            gameObject.SetActive(true);
            ShowEffect(skillData.ScaleMultiplier, skillData.Duration);
        }

        private void ShowEffect(float scaleMultiplier, float duration)
        {
            _posionFieldEffectObject.transform.localScale = Vector3.zero;
            _posionFieldEffectObject.SetActive(true);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_posionFieldEffectObject.transform.DOScale(Vector3.one * scaleMultiplier, 0.5f));
            sequence.OnComplete(() =>
            {
                ApplyDamagedAsync(duration).Forget();
            });
        }

        private async UniTaskVoid ApplyDamagedAsync(float duration)
        {
            float elapsed = 0;
            CancellationToken token = _applyDamagedCts.Token;
            float interval = 0.1f;
            while (elapsed < duration)
            {
                elapsed += interval;
                _monsterColliderList.ForEach(v=> OnHit?.Invoke(v, this));
                
                try
                {
                    await UniTask.WaitForSeconds(interval, cancellationToken: token);
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"{nameof(ApplyDamagedAsync)} error {e.Message}");
                    Sleep();
                    break;
                }
            }
            
            Sleep();
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Monster"))
            {
                _monsterColliderList.Add(other);
            }
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Monster"))
            {
                if (_monsterColliderList.Contains(other))
                {
                    _monsterColliderList.Remove(other);
                }
            }
        }
    }
}