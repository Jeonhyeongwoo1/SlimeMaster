using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.InGame.Controller;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class MonsterRangedAttackSkill : RepeatSkill
    {
        private PlayerController _player;
        
        public override void Initialize(SkillType skillType, CreatureController owner, SkillData skillData)
        {
            base.Initialize(skillType, owner, skillData);

            _player = GameManager.I.Object.Player;
        }

        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
        }

        protected override async UniTask UseSkill()
        {
            if (_player == null || _player.IsDead)
            {
                return;
            }
            
            float distance = Vector3.Distance(_owner.Position, _player.Position);
            if (distance > _skillData.RecognitionRange)
            {
                return;
            }
            
            int projectileCount = _skillData.NumProjectiles;
            for (int i = 0; i < projectileCount; i++)
            {
                Vector3 direction = (_player.Position - _owner.Position).normalized;
                GameObject prefab = GameManager.I.Resource.Instantiate(_skillData.PrefabLabel);
                var shootable = prefab.GetComponent<IGeneratable>();
                shootable.OnHit = OnHit;
                shootable.Generate(_owner.Position, direction, _skillData, _owner);

                try
                {
                    await UniTask.WaitForSeconds(_skillData.ProjectileSpacing, cancellationToken: _skillLogicCts.Token);
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"error {nameof(UseSkill)} log : {e}");
                    break;
                }
            }
        }
    }
}