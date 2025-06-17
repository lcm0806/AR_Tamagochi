using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tortoise : MonoBehaviour, IDamagotchi
{
    public string Name => "거북이";
    public int ID => 008;

    [Header("Status")]
    [SerializeField] private int _health = 100;
    [SerializeField] private int _hunger = 50;
    [SerializeField] private int _happiness = 30;

    public int Health { get { return _health; } }
    public int Hunger { get { return _hunger; } }
    public int Happiness { get { return _happiness; } }
    public bool IsHappyMaxed => _happiness >= 1000;

    // --- IDamagotchi 인터페이스 구현 (행동) ---
    public void Feed(int amount)
    {
        _hunger = Mathf.Max(0, _hunger - amount); // 배고픔 감소 (0 미만으로 안 내려가게)
        _health = Mathf.Min(100, _health + (amount / 2)); // 체력 약간 증가 (100 초과 안 되게)
        Debug.Log($"비둘기에게 {amount}만큼 먹이를 줬습니다. 현재 배고픔: {_hunger}");
    }

    public void Play(int duration)
    {
        _happiness = Mathf.Min(100, _happiness + (duration * 2)); // 행복 증가
        _hunger = Mathf.Min(100, _hunger + (duration / 5)); // 놀면서 배고파짐
        Debug.Log($"비둘기와 {duration}분 놀았습니다. 현재 행복: {_happiness}");
    }

    public void Sleep(int duration)
    {
        _health = Mathf.Min(100, _health + (duration * 3)); // 체력 회복
        _happiness = Mathf.Max(0, _happiness - (duration / 3)); // 자면 좀 심심해질 수 있음
        Debug.Log($"비둘기가 {duration}분 잠들었습니다. 현재 체력: {_health}");
    }

    public void UpdateStatus()
    {
        // 시간이 지남에 따라 상태 변화 (예: 매 초마다 호출)
        _hunger = Mathf.Min(100, _hunger + 1); // 시간이 지날수록 배고파짐
        if (_hunger >= 50)
        {
            _happiness = Mathf.Max(0, _happiness); // 시간이 지날수록 행복 감소
        }
        else
        {
            _happiness = Mathf.Max(0, _happiness - 2);
        }
        _health = Mathf.Max(0, _health - (_hunger > 80 ? 2 : 1)); // 배고프면 체력 더 감소
        Debug.Log($"비둘기 상태 업데이트: 체력={_health}, 배고픔={_hunger}, 행복={_happiness}");
    }

    public bool IsDead()
    {
        return _health <= 0 || _hunger >= 100; // 체력이 없거나 너무 배고프면 사망
    }

    // --- MonoBehaviour 라이프사이클 함수 (선택 사항) ---
    private void Start()
    {

    }

    private void Update()
    {

    }
}
