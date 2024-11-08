using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class SavageSmash : RepeatSkill
    {
        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
        }

        protected override async UniTask UseSkill()
        {
            string prefabLabel = "SavageSmash";
            GameObject prefab = GameManager.I.Resource.Instantiate(prefabLabel);
            var generatable = prefab.GetComponent<IGeneratable>();
            generatable.OnHit = OnHit;
            generatable.Level = CurrentLevel;
            generatable.Generate(_owner.Position, Vector3.zero, _skillData, _owner);

            await UniTask.WaitForSeconds(_skillData.ProjectileSpacing, cancellationToken: _skillLogicCts.Token);
        }
    }
}