using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public abstract class RepeatSkill : BaseSkill
    {
        protected CancellationTokenSource _skillLogicCts;
        
        public override async UniTask StartSkillLogicProcessAsync()
        {
            _skillLogicCts = new CancellationTokenSource();
            var token = _skillLogicCts.Token;
            while (_skillLogicCts != null && !_skillLogicCts.IsCancellationRequested)
            {
                try
                {
                    await UniTask.WaitForSeconds(_skillData.CoolTime, cancellationToken: _skillLogicCts.Token);
                    await UseSkill();
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"error {nameof(StartSkillLogicProcessAsync)} log : {e}");
                    break;
                }
            }
        }
    }
}