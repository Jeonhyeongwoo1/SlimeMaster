using System;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class SpinShot : SequenceSkill
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

                await UniTask.WaitForSeconds(0.5f, cancellationToken: _skillLogicCts.Token);
                float projectileSpawnInterval = 0.25f;
                Vector3 myPos = _owner.Position;
                Vector3 direction = (_targetCreature.Position - myPos).normalized;
                for (int i = 0; i < _skillData.NumProjectiles; i++)
                {
                    direction = Quaternion.Euler(0, 0, _skillData.RoatateSpeed) * direction;
                    GameObject prefab = GameManager.I.Resource.Instantiate(_skillData.PrefabLabel);
                    var shootable = prefab.GetComponent<IGeneratable>();
                    shootable.OnHit = OnHit;
                    shootable.Generate(_owner.Position, direction, _skillData, _owner);
                    
                    try
                    {
                        await UniTask.WaitForSeconds(projectileSpawnInterval, cancellationToken: _skillLogicCts.Token);
                    }
                    catch (Exception e) when (!(e is OperationCanceledException))
                    {
                        Debug.LogError($"error {nameof(UseSkill)} log : {e}");
                        Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
                    }
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