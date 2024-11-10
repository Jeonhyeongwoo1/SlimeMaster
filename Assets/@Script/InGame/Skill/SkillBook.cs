using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using SlimeMaster.Common;
using SlimeMaster.Data;
using SlimeMaster.InGame.Data;
using SlimeMaster.InGame.Enum;
using SlimeMaster.InGame.Manager;
using UnityEngine;

namespace SlimeMaster.InGame.Skill
{
    public class SkillBook
    {
        public List<BaseSkill> ActivateSkillList => _skillList.FindAll(v => v.IsLearn);
        
        private List<BaseSkill> _skillList = new();
        private CancellationTokenSource _useSequenceSkillCts;
        private CreatureController _owner;

        public SkillBook(CreatureController owner, List<SkillData> skillList)
        {
            _owner = owner;
            AddSkillList(skillList);
        }

        private void AddSkillList(List<SkillData> skillList)
        {
            // foreach (var skillData in skillList)
            // {
            //     Debug.Log(skillData.DataId);
            // }

            //미리 모든 스킬들을 초기화 시켜서 셋업
            foreach (SkillData skillData in skillList)
            {
                BaseSkill skill = null;
                AddSkill(skillData, ref skill);
                if (skill != null)
                {
                    Debug.Log(skillData.DataId);
                    _skillList.Add(skill);
                }
            }
        }

        public void UseAllSkillList(bool useRepeatSkill, bool useSequenceSkill, CreatureController targetCreature)
        {
            if (useRepeatSkill)
            {
                var repeatSkillList = _skillList.FindAll(v => v is RepeatSkill);
                repeatSkillList.ForEach(v=> UpgradeOrAddSkill(v.SkillData));
            }

            if (useSequenceSkill)
            {
                UseSequenceSkill(targetCreature);
            }
        }

        private async void UseSequenceSkill(CreatureController targetCreature)
        {
            var sequenceSkillList = _skillList.FindAll(v => v is SequenceSkill);
            if (sequenceSkillList.Count == 0)
            {
                return;
            }
            
            sequenceSkillList.ForEach(v=>
            {
                UpgradeOrAddSkill(v.SkillData);
                (v as SequenceSkill).SetTargetCreature(targetCreature);
            });
            
            _useSequenceSkillCts = new CancellationTokenSource();
            
            try
            {
                //잠깐의 유예기간을 준다.
                await UniTask.WaitForSeconds(1f, cancellationToken: _useSequenceSkillCts.Token);
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Debug.LogError($"error {nameof(UseSequenceSkill)} log : {e}");
                return;
            }
            
            int index = 0;
            while (_useSequenceSkillCts != null && !_useSequenceSkillCts.IsCancellationRequested)
            {
                var sequenceSkill = sequenceSkillList[index];
                Debug.Log("NextSkill " + sequenceSkill.SkillType + " count : " + sequenceSkillList.Count);
                try
                {
                    await sequenceSkill.StartSkillLogicProcessAsync(_useSequenceSkillCts);
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    Debug.LogError($"error {nameof(UseSequenceSkill)} log : {e}");
                    break;
                }
                    
                index++;
                index %= sequenceSkillList.Count;
                
            }
        }
        
        public void StopSequenceSkill()
        {
            Utils.SafeCancelCancellationTokenSource(ref _useSequenceSkillCts);
        }

        public void UpgradeOrAddSkill(SkillData skillData)
        {
            var skill = _skillList.Find(v => v.SkillData.DataId == skillData.DataId);
            if (skill.IsMaxLevel)
            {
                Debug.LogWarning("is max level :" + skill.SkillData.DataId);
                return;
            }

            if (!skill.IsLearn)
            {
                skill.Learn();
                UseSkill(skill);
                return;
            }
            
            SkillData upgradeSkillData = GameManager.I.Data.SkillDict[skillData.DataId + 1];
            skill.UpdateSkillData(upgradeSkillData);
            UseSkill(skill);
        }

        private SkillType GetSkillType(int id)
        {
            SkillType skillType = SkillType.None;
            if (id > (int)SkillType.BossSkill)
            {
                Array array = System.Enum.GetValues(typeof(SkillType));
                foreach (var obj in array)
                {
                    int value = (int)obj;
                    int range = 4 + value;
                    if (value <= id && id <= range)
                    {
                        return (SkillType)value;
                    }
                }
            }
            else
            {
                return (SkillType)id;
            }

            Debug.LogError("Failed get skill type : " + id);
            return SkillType.None;
        }
        
        private void AddSkill(SkillData skillData, ref BaseSkill baseSkill)
        {
            SkillType skillType = GetSkillType(skillData.DataId);
            Debug.Log("SkillType : " + skillType.ToString());
            if (skillType == SkillType.None)
            {
                return;
            }
            
            string skillName = $"{typeof(BaseSkill).Namespace}.{skillType}";
            try
            {
                var skill = Activator.CreateInstance(Type.GetType(skillName)) as BaseSkill;
                // Debug.Log($"{skill} / {skillName} / {skill == null}");
                if (skill == null)
                {
                    return;
                }

                skill.Initialize(skillType, _owner, skillData);
                baseSkill = skill;
            }
            catch (Exception e)
            {
                
            }
        }
        
        public void UseSkill(BaseSkill baseSkill)
        {
            baseSkill.StopSkillLogic();
            baseSkill.StartSkillLogicProcessAsync().Forget();
        }

        public void StopAllSkillLogic()
        {
            _skillList.ForEach(v=> v.StopSkillLogic());
        }

        public List<BaseSkill> GetRecommendSkillList(int recommendSkillCount = 3)
        {
            List<BaseSkill> list = ActivateSkillList.Count == Const.MAX_SKILL_COUNT
                ? ActivateSkillList.FindAll(v => v.CurrentLevel < Const.MAX_SKILL_Level)
                : _skillList.FindAll(v => v.CurrentLevel < Const.MAX_SKILL_Level);
            
            list.Shuffle();
            return list.Take(recommendSkillCount).ToList();
        }
    }
}