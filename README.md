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

<p>
- `Manager` 클래스는 싱글톤 패턴으로 설계 <br>
- 메모리 오버헤드 및 인스턴스 생성 비용을 최적화 <br>
- 전역적으로 단 하나의 인스턴스만 보장하기 위함 <br>
</p>
