using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.InGame.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class ArrowShot : RepeatSkill
    {
        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
        }

        protected override async UniTask UseSkill()
        {
            int projectile = _skillData.NumProjectiles;
            for (int i = 0; i < projectile; i++)
            {
                GameObject prefab = GameManager.I.Resource.Instantiate(_skillData.PrefabLabel);
                var generatable = prefab.GetComponent<IGeneratable>();
                generatable.OnHit = OnHit;
                generatable.Generate(_owner.Position, _owner.GetDirection(), _skillData, _owner);
                try
                {
                    await UniTask.WaitForSeconds(_skillData.ProjectileSpacing, cancelImmediately: true);
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"error {nameof(UseSkill)} log : {e.Message}");
                    StopSkillLogic();
                    break;
                }
            }
        }
    }
}