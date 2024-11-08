using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.InGame.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class EletronicField : RepeatSkill
    {
        public override void StopSkillLogic()
        {
            Utils.SafeCancelCancellationTokenSource(ref _skillLogicCts);
        }

        protected override async UniTask UseSkill()
        {
            GameObject prefab = GameManager.I.Resource.Instantiate(_skillData.PrefabLabel);
            var shootable = prefab.GetComponent<IGeneratable>();
            shootable.OnHit = OnHit;
            shootable.Generate(_owner.Position, Vector3.zero, _skillData, _owner);

            await UniTask.WaitUntil(() => _skillLogicCts.IsCancellationRequested);
        }
    }
}