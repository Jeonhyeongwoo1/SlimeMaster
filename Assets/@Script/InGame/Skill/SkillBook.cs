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
using Random = UnityEngine.Random;

namespace SlimeMaster.InGame.Skill
{
    public class SkillBook
    {
        public List<BaseSkill> ActivateSkillList => _skillList.FindAll(v => v.IsLearn);
        public int SupportSkillCount => _activateSupportSkillDataList.Count;
        public List<SupportSkill> ActivateSupportSkillDataList => _activateSupportSkillDataList;
        
        private List<BaseSkill> _skillList = new();
        private List<SupportSkill> _activateSupportSkillDataList = new();
        
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

        public int GetActivateSkillLevel(int skillId)
        {
            var skill = ActivateSkillList.Find(v => v.SkillData.DataId == skillId);
            if (skill == null)
            {
                Debug.LogError("Failed get skill id " + skillId);
                return 0;
            }

            return skill.CurrentLevel;
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

        public void AddSupportSkill(SupportSkillData supportSkillData)
        {
            var skill = new SupportSkill(supportSkillData);
            _activateSupportSkillDataList.Add(skill);
        }

        public List<SupportSkillData> GetRecommendSupportSkillDataList()
        {
            /*
             *  총 4개의 리스트를 뽑느다.
             *   1. 고정 시킨 스킬을 먼저 get
             *   2. 남은 것들 중에서 랜덤으로 등급을 먼저 선출
             *   3. 선출된 등급을 기준으로 스킬을 get
             *   4. 스킬이 없으면 그 가장 아랫단계의 등급부터 다시 스킬을 찾아 스킬이 있으면 Get 없으면 reutrn
             */

            int count = Const.SUPPORT_ITEM_USEABLECOUNT;
            var recommendSkillDataList = new List<SupportSkillData>();
            recommendSkillDataList.AddRange(GameManager.I.lockSupportSkillDataList);

            count -= recommendSkillDataList.Count;
            for (int i = 0; i < count; i++)
            {
                SupportSkillGrade grade = GetRandomSupportSkillGrade();
                List<SupportSkillData> skillList = GetSupportSkillData(grade, recommendSkillDataList);
                if (skillList.Count > 0)
                {
                    int random = Random.Range(0, skillList.Count);
                    recommendSkillDataList.Add(skillList[random]);
                }
                else
                {
                    AddRecommendSkillData(0, ref recommendSkillDataList);            
                }
            }

            return recommendSkillDataList;
        }

        private void AddRecommendSkillData(int depth, ref List<SupportSkillData> skillDataList)
        {
            int length = System.Enum.GetNames(typeof(SupportSkillGrade)).Length;
            if (depth > length)
            {
                return;
            }

            var list = GetSupportSkillData((SupportSkillGrade)depth, skillDataList);
            if (list.Count > 0)
            { 
                int random = Random.Range(0, list.Count);
                skillDataList.Add(list[random]);
                return;
            }

            AddRecommendSkillData(depth + 1, ref skillDataList);
        }

        private List<SupportSkillData> GetSupportSkillData(SupportSkillGrade grade, List<SupportSkillData> addedSupportSkillDataList)
        {
            var skillDataList = GameManager.I.Data.SupportSkillDataDict.Values.ToList();
            var excludeList =
                new HashSet<SupportSkillData>().Concat(_activateSupportSkillDataList.Select(c => c.SupportSkillData))
                    .Concat(GameManager.I.lockSupportSkillDataList)
                    .Concat(addedSupportSkillDataList);
            
            return skillDataList.Where(x => !excludeList.Contains(x) && x.SupportSkillGrade == grade).ToList();
        }

        private SupportSkillGrade GetRandomSupportSkillGrade()
        {
            float select = Random.value;
            if (select < Const.SUPPORTSKILL_GRADE_PROB[(int)SupportSkillGrade.Legend])
            {
                return SupportSkillGrade.Legend;
            }

            if (select < Const.SUPPORTSKILL_GRADE_PROB[(int)SupportSkillGrade.Epic])
            {
                return SupportSkillGrade.Epic;
            }

            if (select < Const.SUPPORTSKILL_GRADE_PROB[(int)SupportSkillGrade.Rare])
            {
                return SupportSkillGrade.Rare;
            }

            if (select < Const.SUPPORTSKILL_GRADE_PROB[(int)SupportSkillGrade.Uncommon])
            {
                return SupportSkillGrade.Uncommon;
            }

            return SupportSkillGrade.Common;
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