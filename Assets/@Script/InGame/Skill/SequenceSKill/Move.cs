using System;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Enum;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class Move : SequenceSkill
    {
        protected override async UniTask UseSkill()
        {
            if (_targetCreature == null)
            {
                return;
            }

            float elapsed = 0;
            while (!_owner.IsDeadState || _targetCreature.IsDeadState)
            {
                _owner.UpdateStateAndAnimation(CreatureStateType.Skill, "Move");
                
                Vector3 myPos = _owner.Position;
                Vector3 direction = (_targetCreature.Position - myPos).normalized;
                Vector3 targetPos = _targetCreature.Position +
                                         direction * UnityEngine.Random.Range(SkillData.MinCoverage,
                                             SkillData.MaxCoverage);

                float distance = Vector2.Distance(myPos, targetPos);
                if (distance < 0.1f)
                {
                    break;
                }

                elapsed += Time.fixedDeltaTime;
                if (elapsed > 3f)
                {
                    break;
                }

                Vector3 dir = (targetPos - myPos).normalized * (SkillData.ProjSpeed * Time.fixedDeltaTime);
                _owner.RigidBodyMovePosition(dir);

                try
                {
                    await UniTask.WaitForFixedUpdate(cancellationToken: _skillLogicCts.Token);
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"error {nameof(StartSkillLogicProcessAsync)} log : {e}");
                    Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
                }
            }
        }
    }
}