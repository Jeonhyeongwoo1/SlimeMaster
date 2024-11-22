using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Enum;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public abstract class SequenceSkill : BaseSkill
    {
        public bool UseCollTime { get; protected set; }
        
        protected CreatureController _targetCreature;
        protected CancellationTokenSource _skillLogicCts;
        
        public override async UniTask StartSkillLogicProcessAsync(CancellationTokenSource cts = null)
        {
            _skillLogicCts = cts ?? new CancellationTokenSource();
            var token = _skillLogicCts.Token;
            
            try
            {
                await UseSkill();
                if (UseCollTime)
                {
                    await UniTask.WaitForSeconds(_skillData.CoolTime, cancellationToken: token);
                }
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Debug.LogError($"error {nameof(StartSkillLogicProcessAsync)} log : {e}");
            }
            
            _owner.UpdateStateAndAnimation(CreatureStateType.Idle, "Idle");
            Debug.LogWarning("Done");
        }
        
        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
        }

        public void SetTargetCreature(CreatureController targetCreature)
        {
            _targetCreature = targetCreature;
        }
    }
}