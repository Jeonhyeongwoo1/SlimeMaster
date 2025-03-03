## 목차
[1.프로젝트 개요](#프로젝트-개요)<br/>
[2.프로젝트 아키텍처](#프로젝트-아키텍처)<br/>

## 프로젝트 개요

- 개발 기간
    - 2024.11.1 ~12.1
- 게임 장르
    - 캐주얼
- 타겟 플랫폼
    - 모바일
- 개발 인원
    - 1인
- 목표
    - 뱀파이어 서바이벌 장르 게임을 구현한다.
    - 제공받은 리소스을 활용하되 소스 코드는 새롭게 구현하여 프로젝트를 구현한다.
- Github 주소
    - 클라이언트 - https://github.com/Jeonhyeongwoo1/SlimeMaster
    - 서버 - https://github.com/Jeonhyeongwoo1/slime_master_server
- 사용 기술
    - Unity
    - ASP.NET
    - firebase
- 게임 설명
    - 뱀파이어 서바이벌 장르의 모바일용 게임
    - 캐릭터 성장에 따른 스킬 강화 및 캐릭터 능력치 상승하는 방식
    - 재화 및 뽑기를 활용해서 캐릭터 장비를 구매하여 캐릭터 강화
- 게임 화면
  - Title
    - <img width="260" alt="Image" src="https://github.com/user-attachments/assets/e963ca37-b80c-4968-abe5-24bfd56a45e5" />
  - Lobby
    - <img width="255" alt="Image" src="https://github.com/user-attachments/assets/c26e2de9-6f57-456b-9ca7-d49b871ed3fe" />
    - <img width="261" alt="Image" src="https://github.com/user-attachments/assets/e2353c77-880d-4434-91a4-a480d4bd1499" />
  - Game
    - https://github.com/user-attachments/assets/8d2e1186-03e0-4321-b16f-e11c33b8b20d

## 프로젝트 아키텍처
<div align="center">
    <img width="445" alt="Image" src="https://github.com/user-attachments/assets/973b5249-ed3b-4a26-a6e2-6e2599094358" />
</div>

<p>
<b>Client (Unity)</b> - 로그인, 게임 데이터 업데이트 요청 및 조회 - RESTful 방식 사용 <br>
<b>Server (ASP.NET)</b> - Firebase와 연동 및 게임 데이터 업데이트 및 조회 - 사용자 UID 인증 <br>
<b>Storage (Firebase)</b> - 유저 데이터 관리 <br>
<b>Client - Server - Storage</b> - Sequence Diagram <br>
<div align="center">
    <img width="445" alt="Image" src="https://github.com/user-attachments/assets/60813373-32a2-4802-9915-02e287f4fd90" />
</div>
</p>

## 트러블 슈팅
- 몬스터가 장애물에 걸려서 이동하지 못하는 현상
   - https://github.com/Jeonhyeongwoo1/SlimeMaster/issues/2
- 길찾기 알고리즘 적용 후 프레임 드랍 현상
   - https://github.com/Jeonhyeongwoo1/SlimeMaster/issues/1
 
## Managers 구조(싱글톤 패턴)

<div align="center">
    <img width="904" alt="Image" src="https://github.com/user-attachments/assets/8ecf3215-a6c5-4140-8b47-91a257466afc" />
</div>

- Manager 클래스는 싱글톤 패턴으로 설계 <br>
- 메모리 오버헤드 및 인스턴스 생성 비용을 최적화 <br>
- 전역적으로 단 하나의 인스턴스만 보장하기 위함 <br>

## Model, Presenter(팩토리 패턴)
- Model, Presenter는 전역적으로 접근해야할 수 있으므로 한 곳에서 관리하지 않으면 코드 관리가 어려워지므로 Factory내에서 관리 및 생성
- 모든 Model, Presenter들을 중앙에서 관리 및 생성하여 중앙 집중화
```csharp
public static class ModelFactory
{
    private static readonly Dictionary<Type, IModel> _modelDict = new();

    public static T CreateOrGetModel<T>() where T : IModel, new ()
    {
        if (!_modelDict.TryGetValue(typeof(T), out var model))
        {
            model = new T();
            _modelDict.Add(typeof(T), model);
            return (T) model;
        }

        return (T)model;
    }

    public static void ClearCachedDict()
    {
        _modelDict.Clear();
    }
}
```
## UI 구조(MVP 패턴)

<div align="center">
    <img width="445" alt="Image" src="https://github.com/user-attachments/assets/96eaec8c-900c-43db-b52b-07ae81b4580d" />
</div>

- Model, View, Presenter간의 명확한 분리를 통해 코드의 구조 개선
- 각 모듈이 분류되어서 구현되었기 때문에 서로간의 결합도가 낮고 확장성과 유연성이 높아지므로 코드 관리가 쉬움
- View - Presenter 1 : 1 관계

## 리소스 관리

---

### Addressables를 이용한 리소스 관리

- 씬에 모든 리소스를 배치시키는 방식은 로딩 시간이 오래 걸리고 사용될지도 모르는 상황이기 때문에 메모리 낭비가 발생함
- 따라서 Addressables를 활용하여 필요할 때 리소스를 로드시키고 필요에 따라 풀링하는 방법으로 구현
- 각 AssetBundle은 라벨을 추가하여 관리할 수 있으므로 리소스 관리에 용이함.
- 관련 코드 주소 : https://github.com/Jeonhyeongwoo1/SlimeMaster/blob/main/Assets/%40Script/Manager/ResourcesManager.cs
### DataSheet를 활용한 게임 데이터 관리

- 게임 내에 필요한 데이터(몬스터 체력, 스테이지 등)을 관리하기 위함
- 더 나아가 필요한 리소스 이름을 시트에 적어 놓으면 미리 로드 시킨 리소스와 이름을 일치시켜서 필요할 때 `ResourcesManager` 통해서 리소스를 불러 올 수 있으므로 유연하게 리소스 변경에 대처할 수 있음.


## Client - Server 구조

---

### 개요

- 목표 :
    - Unity에서 로그인 및 게임 데이터를 관리하기 위해 [ASP.NET](http://ASP.NET) 서버와 Firebase를 활용하는 시스템 구축
- 목적 :
    - 클라이언트
        - 로그인 및 유저 데이터를 UI를 통해 표시
    - Server
        - 클라이언트의 요청을 처리하고 인증 및 데이터 검증, 가공
    - Firebase
        - 유저 정보를 저장하여 일관된 데이터를 제공

### Login 과정

- Sequence diagram

<div align="center">
    <img width="1427" alt="Image" src="https://github.com/user-attachments/assets/200524fc-cd24-4c49-879e-fa5d17cb8c7a" />
</div>

  - 설명
    1. 클라이언트(Unity)
        - 클라이언트에서 저장된 UID가 있는지 확인한 다음에 서버로 UID를 전달(없으면 Null로 전달)
            - UID는 디바이스내에 저장
    2. 서버(ASP.NET)
        - 서버는 클라이언트에게 전달받은 UID를 기준으로 UID가 있으면 Firebase를 통해서 UID 검증, UID가 없을 경우에는 Firebase에 새롭게 UID를 생성
        - Firebase로부터 받은 UID를 기준으로 JWT를 생성
    3. 서버 → 클라이언트 응답
        - 생성한 JWT와 UID를 클라이언트에게 전달
        - 이 후에는 클라이언트는 JWT, UID 저장하여 API 요청에 사용
    - JWT 발급 이유
        - 클라이언트로부터 요청이 들어왔을 때 서버는 해당 클라이언트가 유효한지 판단해야하므로 JWT를 발급하여 클라이언트를 인증
        - 이후 클라이언트는 모든 API 요청에 JWT를 포함하여 서버에 전달하며, 서버는 Firebase 조회 없이 JWT의 서명을 검증하여 클라이언트의 신원을 확인할 수 있음.
     

## Sklill 설계

---

### 개요

- 목표: 재사용성과 확장성이 뛰어난 시스템 설계를하여 다양한 상황에서 손쉽게 사용 및 관리할 수 있도록 한다
- 목적: 게임 내에 몬스터, 플레이어가 사용할 다양한 스킬을 설계한다.

<div align="center">
    <img width="1007" alt="Image" src="https://github.com/user-attachments/assets/8c85e3e7-8d30-42b1-a9d4-3b5786aa2923" />
</div>

- `Other…` : 기타 스킬(Basic Attack, ChainLighting..등)
1. `SkillBook`  
    - 플레이어의 현재 사용되고 있는 Skill들을 관리
    - Skill 사용 및 레벨 업등을 관리
2. `BaseSkill` 
    - 모든 스킬의 기본적인 동작을 정의하는 부모 클래스
    - 스킬 실행을 관리하는 클래스(쿨타임, 공격 방향 로직, 투사체 생성, 히트 판정 후 로직 실행)
    - SkillData를 관리
    1. `SequecenSkill` :
        - 순차적으로 진행되는 Skill의 기본적인 동작을 정의하는 부모 클래스(CircleShot → Dash → CircleShot …)
    2. `RepeatSkill` : 
        - 쿨타임이 존재하고 반복적으로 사용되는 스킬의 기본적인 동작을 정의하는 부모 클래스
3. `Projectile`  : 
    - 모든 투사체의 기본적은 동작을 정의하는 부모 클래스
    - 투사체 이동, 물리 엔진 적용, 충돌 판정, 파티클 & 사운드 효과 처리

### 스킬 추가 구현

- 스킬을 추가할 때에는 스킬에 대한 정보가 필요하다. 스킬에 대한 정보는 데이터 시트에서 관리를 하기 때문에 데이터 시트를 통해서 스킬에 대한 정보를 얻어와야 한다.
- 이 때 Skill의 DataID를 기준으로 어떤 스킬인지 판단할 수 있다.
<div align="center">
   <img width="727" alt="Image" src="https://github.com/user-attachments/assets/8676c3ae-2f3c-4775-b854-a72956ea2b14" /> 
</div>

- 위의 시트에 명시된 것처럼 에너지 볼트는 DataId가 10001이므로 숫자임을 알 수 있다. 숫자로 명시했기 때문에 코드 내에서 enum으로 관리하면 어떤 DataID가 어떤 SKillType인지 쉽게 알 수 있으므로 해당 DataId에 맞춰서 SkillType을 명시한다.
```csharp
public enum SkillType
{
  None = 0,
  EnergyBolt = 10001,       //100001 ~ 100005 
  IcicleArrow = 10011,          //100011 ~ 100015 
  PoisonField = 10021,      //100021 ~ 100025 
  EletronicField = 10031,   //100031 ~ 100035 
  ...
}
```
- 스킬을 추가하는 방법은 다음과 같은 방법이 있다.
    1. 인게임이 실행될 때 오브젝트가 사용할 수 있는 모든 스킬을 미리 생성한 다음에 게임 진행 도중에 활성화되는 스킬만 따로 관리하여 스킬을 관리한다.
        1. 문제점
            - 모든 스킬을 미리 생성하면 추가될 때마다 코드를 변경해야하는 까다로움이 있다.
            - 불필요하게 힙에 스킬이 할당되므로 메모리 낭비가 발생한다.
    2. 인게임이 진행되는 도중에 필요한 스킬만 추가한다.
        - 사용하는 스킬만 관리하면 되기 때문에 관리가 용이하며 불필요한 메모리 낭비가 발생하지 않는다.
    
    따라서 2번 방식으로 스킬을 추가한다.
    

- C#의 리플렉션 기능을 이용한 Skill 추가
    - 스킬들을 미리 프리팹으로 만들어서 리소스를 로드시켜서 추가하거나, 직렬화를 통해서 인스펙터에 부착하는 방법으로 할 수도 있지만 매번 스킬이 추가될 때마다 개발자가 직접 추가해야하는 번거로움이 있다.
    - 하지만 C#의 리플렉션 기능을 활용하면 클래스 이름만 알아도 런타임에서 해당 클래스에 대해서 알 수 있다.
    - 따라서 `var skill = Activator.CreateInstance(Type.GetType(skillName)) as BaseSkill;` 처럼 인스턴스화하여 Skill 클래스를 가져오는 방식을 활용
```csharp
//SKillBook.cs
private void AddSkill(SkillData skillData, ref BaseSkill baseSkill)
{
  SkillType skillType = GetSkillType(skillData.DataId);
  if (skillType == SkillType.None)
  {
      Debug.LogWarning("SkillType is None / skill id : " + skillData.DataId);
      return;
  }
  
  string skillName = $"{typeof(BaseSkill).Namespace}.{skillType}";
  var skill = Activator.CreateInstance(Type.GetType(skillName)) as BaseSkill;
  if (skill == null)
  {
      Debug.LogWarning("skill is null : " + skillData.DataId);
      return;
  }

  skill.Initialize(skillType, _owner, skillData);
  baseSkill = skill;
}
```

### 스킬 사용 구현

- 스킬이 추가되고나면 스킬이 바로 실행되어야 한다.
- 따라서 SKillBook에서 스킬이 추가되면 스킬이 바로 사용되어야 한다.
- 스킬을 사용할 때에는 스킬 로직을 먼저 실행 후, 스킬 사용 단계로 진행되어야한다.
    - 스킬 로직 실행
        - 먼저 Skill Logic 토큰을 발급한다.
        - 그 후에 Skill의 쿨타임이 있는지 확인 후 스킬 사용
- 소스코드 : https://github.com/Jeonhyeongwoo1/SlimeMaster/blob/main/Assets/%40Script/InGame/Skill/BaseSkill.cs

- 투사체 발사
    - 스킬에 의해서 생성된 프리팹에는 투사체와 관련된 클래스가 컴포넌트로 부착되어야한다.
    - 투사체 클래스에서는 투사체 이동, 물리 엔진 적용, 충돌 판정, 파티클 & 사운드 효과 처리를 담당한다.
- 소스코드 : https://github.com/Jeonhyeongwoo1/SlimeMaster/blob/main/Assets/%40Script/InGame/Skill/RepeatSkill/Projectile.cs



