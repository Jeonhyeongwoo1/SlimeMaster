## 목차
[1.프로젝트 개요](#프로젝트-개요)<br/>
[2.프로젝트 아키텍처](#프로젝트-아키텍처)<br/>
[3.트러블 슈팅](#트러블-슈팅)<br/>
[4.Managers 구조(싱글톤 패턴)](#Managers-구조)<br/>
[5.UI 구조(MVP 패턴)](#UI-구조)<br/>
[6.팩토리 패턴](#팩토리-패턴)<br/>
[6.Client-Server 구조](#Client-Server-구조)<br/>

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
    - Unity(2022.3.55f1)
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
 
## Managers 구조

<div align="center">
    <img width="904" alt="Image" src="https://github.com/user-attachments/assets/8ecf3215-a6c5-4140-8b47-91a257466afc" />
</div>

- Manager 클래스는 싱글톤 패턴으로 설계 <br>
- 메모리 오버헤드 및 인스턴스 생성 비용을 최적화 <br>
- 전역적으로 단 하나의 인스턴스만 보장하기 위함 <br>

## UI 구조

<div align="center">
    <img width="445" alt="Image" src="https://github.com/user-attachments/assets/96eaec8c-900c-43db-b52b-07ae81b4580d" />
</div>

- Model, View, Presenter간의 명확한 분리를 통해 코드의 구조 개선
- 각 모듈이 분류되어서 구현되었기 때문에 서로간의 결합도가 낮고 확장성과 유연성이 높아지므로 코드 관리가 쉬움
- View - Presenter 1 : 1 관계
  
## 팩토리 패턴
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


## Client-Server 구조

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
    <img width="600" alt="Image" src="https://github.com/user-attachments/assets/200524fc-cd24-4c49-879e-fa5d17cb8c7a" />
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
     


