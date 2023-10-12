# Sparta3DGame
스파르타 3D 게임

## 🟡 프로젝트 소개

---

###**Project :**  **Sparta 3D Game
###**Use : Unity**

<aside>
 통나무 집 주변에서 생존과 모험을 경험할 기회를 제공합니다. 게임 내에서는 다양한 도구를 사용하여 자원을 채취하고, 이를 활용하여 아이템을 생성합니다. 주요 요소 중 하나는 적들과의 전투로, 플레이어는 다양한 무기와 방어구를 사용하여 적을 물리칩니다. 게임 내에서 낮과 밤의 주기가 구분되어 더욱 입체적인 플레이를 할 수 있습니다. 이 게임은 창의력과 생존 능력을 요구하며, 플레이어는 자신만의 세계를 구축하거나 새로운 모험을 즐길 수 있습니다.

</aside>

## 🟡 기능 구현

---

1. **인트로 씬 구성**
    - 시작 버튼 추가

1. **캐릭터 1인칭 시점**
    - 카메라 제어(Player 오브젝트의 하위 객체에 main 카메라 설치)

1. **Environment 꾸미기**
    - ground: 에셋 스토어에서 ‘통나무 집 마을’ 다운로드
        
        [rpg-poly-pack-lite-148410](https://assetstore.unity.com/packages/3d/environments/landscapes/rpg-poly-pack-lite-148410)
        
    - Sky: Skybox Material 설정(하늘 색 변경)

1. **키보드와 마우스 제어**
    - **Input Action** 활용
    - 키보드: WASD로 Player 이동, space bar로 점프, Tab로 Inventory 띄우기, E로 아이템 줍기
    - 마우스: Look, 좌 클릭으로 공격

1. **Player 상태 UI**
    - 상태: Health, Hunger, Stamina
    - UI는 Condition Bar 이미지 활용
    - PlayerConditions 스크립트에 interface와 상속 활용

1. **접촉 오브젝트로 인한 데미지 처리**
    - 불에 닿으면 Health 감소하며, 화면 경고(빨간색) 깜박임

1. **낮과 밤 구현**
    - Sun과 Moon
    - DayNightCycle 스크립트
        - `**AnimationCurve`** 메서드 사용

1. **아이템** 
    - 아이템 데이터 저장: `**ScriptableObject**`
        - 자원(Resource): Wood, Rock
        - 음식(Consumable): Carrot
        - 무기(Equipable): Sword, Axe
    - `**raycast**`로 아이템 인식
        - Player Input Event - Item 줍기 설정(E 키)

1. **Inventory UI와 아이템 사용**
    - 인벤토리 UI
        - 아이템 이름과 아이템 정보
        - 아이템 개수
        - 버튼: 아이템, 사용, 해체, 장착, 버림
        - 클릭 시자워, 아이템 종류마다 버튼과 아이템 정보 변경
        - Player Input Event - Inventory 설정(Tab 키)
    - 아이템 사용
        - 음식(Consumable) 사용하여 Health와 Hunger 채우기
        - 무기(Equipable) 장착하기
            - `**Instantiate**`로 장착 기능 구현
        - 아이템을 Player 앞에 버리기

1. **아이템 장착 애니메이션** 
    - 장착한 무기 애니메이션 실행
        - Attack과 Idle 애니메이션 제작
        - Player Input Event - Attack 설정(좌 클릭)

1. **자원 채취**
    - 도끼(Axe)에 접촉할 때마다 자원 생성
        - `**raycast`**  활용
        - `**Instantiate**`로 자원 생성
        - 도끼 Attack때마다 Stamian 감소하다 회복

1. **적 생성과 로직**
</aside>

1. **자원 활용**
    - 집 짓기

1. **Inventory UI 디자인**
    - 컨셉에 맞게 디자인 변경

1. **다양한 아이템 제공**

1. **공격 모션 업그레이드**
    - 공격할 때 파티클 생성
    - 다양한 공격 모션 제공

1. **게임 종료 모드**
    - 게임 종료 씬
        - 다시하기, 끝내기 버튼
</aside>
