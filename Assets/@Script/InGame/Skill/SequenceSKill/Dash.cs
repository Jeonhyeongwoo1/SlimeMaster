using System;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Enum;
using SlimeMaster.InGame.Entity;
using SlimeMaster.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class Dash : SequenceSkill
    {
        protected override async UniTask UseSkill()
        {
            if (_targetCreature == null)
            {
                return;
            }

            GameObject obj = GameManager.I.Resource.Instantiate(nameof(SkillRange));
            obj.transform.SetParent(_owner.transform);
            obj.transform.localPosition = Vector3.zero;
            var skillRange = obj.GetComponent<SkillRange>();

            float elapsed = 0;
            _owner.UpdateStateAndAnimation(CreatureStateType.Skill, "Dash");
            obj.SetActive(true);
            while (elapsed < _skillData.Duration)
            {
                if (_owner.IsDeadState || _targetCreature.IsDeadState)
                {
                    break;
                }

                elapsed += Time.deltaTime;
                Vector3 direction = (_targetCreature.Position - _owner.Position).normalized;
                float dist = Vector2.Distance(_owner.Position,
                    (_targetCreature.Position + direction * _skillData.MaxCoverage));
                skillRange.SetLineSizeAndRotation(dist, direction);

                try
                {
                    await UniTask.Yield(cancellationToken: _skillLogicCts.Token);
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"error {nameof(StartSkillLogicProcessAsync)} log : {e}");
                    Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
                }
            }

            GameManager.I.Pool.ReleaseObject(nameof(SkillRange), obj);
            obj.SetActive(false);

            Vector3 myPos = _owner.Position;
            Vector3 targetPos = _targetCreature.Position;

            float distance = Vector2.Distance(myPos, targetPos);
            while (distance > 2.0f)
            {
                if (_owner.IsDeadState || _targetCreature.IsDeadState)
                {
                    break;
                }

                myPos = _owner.Position;
                targetPos = _targetCreature.Position;
                distance = Vector2.Distance(myPos, targetPos);
                Vector3 direction = (targetPos - myPos).normalized;
                _owner.RigidBodyMovePosition(direction * (Time.fixedDeltaTime * _skillData.ProjSpeed));

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

            if (_owner.IsDeadState || _targetCreature.IsDeadState)
            {
                return;
            }
            
            try
            {
                await UniTask.WaitForSeconds(_skillData.AttackInterval, cancellationToken: _skillLogicCts.Token);
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Debug.LogError($"error {nameof(StartSkillLogicProcessAsync)} log : {e}");
                Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
            }
        }
    }
}