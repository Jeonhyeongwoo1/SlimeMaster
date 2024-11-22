using System;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Enum;
using SlimeMaster.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class ComboShot : SequenceSkill
    {
        protected override async UniTask UseSkill()
        {
            if (_targetCreature == null)
            {
                return;
            }

            float elapsed = 0;
            if (!_owner.IsDeadState || _targetCreature.IsDeadState)
            {
                _owner.UpdateStateAndAnimation(CreatureStateType.Skill, "Attack");

                try
                {
                    await UniTask.WaitForSeconds(0.5f, cancellationToken: _skillLogicCts.Token);
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"error {nameof(UseSkill)} log : {e}");
                    Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
                }
                
                Vector3 myPos = _owner.Position;
                Vector3 direction = (_targetCreature.Position - myPos).normalized;
                float angle = 360 / _skillData.NumProjectiles;
                for (int i = 0; i < _skillData.NumProjectiles; i++)
                {
                    var rot = Quaternion.Euler(0, 0, angle * i);
                    direction = rot * Vector3.up;
                    
                    GameObject prefab = GameManager.I.Resource.Instantiate(_skillData.PrefabLabel);
                    var shootable = prefab.GetComponent<IGeneratable>();
                    shootable.OnHit = OnHit;
                    shootable.Generate(_owner.Position, direction, _skillData, _owner);
                }

                try
                {
                    await UniTask.WaitForSeconds(_skillData.AttackInterval, cancellationToken: _skillLogicCts.Token);
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"error {nameof(UseSkill)} log : {e}");
                    Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
                }
            }
        }
    }
}