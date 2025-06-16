using UnityEngine; // Color를 사용하기 위해 필요

// 'I'는 인터페이스임을 나타내는 일반적인 명명 규칙입니다.
public interface IDamagotchi
{
    // --- 속성 (Properties) ---
    // 각 다마고치의 고유한 이름
    string Name { get; }
    // 다마고치의 ID 또는 종류 번호
    int ID { get; }

    // --- 다마고치 상태 (Properties) ---
    // 현재 체력 (읽기 전용 또는 쓰기 가능)
    int Health { get; }
    // 현재 포만감 (읽기 전용 또는 쓰기 가능)
    int Hunger { get; }
    // 현재 행복도 (읽기 전용 또는 쓰기 가능)
    int Happiness { get; }
    // 행복도가 가득차면
    bool IsHappyMaxed { get; }

    // --- 행동 (Methods) ---
    // 먹이를 주는 행동
    void Feed(int amount);
    // 놀아주는 행동
    void Play(int duration);
    // 재우는 행동
    void Sleep(int duration);
    // 시간이 지남에 따라 상태 변화를 업데이트하는 행동
    void UpdateStatus();
    // 다마고치가 죽었는지 여부
    bool IsDead();
}