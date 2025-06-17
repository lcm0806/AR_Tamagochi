using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tortoise : MonoBehaviour, IDamagotchi
{
    public string Name => "�ź���";
    public int ID => 008;

    [Header("Status")]
    [SerializeField] private int _health = 100;
    [SerializeField] private int _hunger = 50;
    [SerializeField] private int _happiness = 30;

    public int Health { get { return _health; } }
    public int Hunger { get { return _hunger; } }
    public int Happiness { get { return _happiness; } }
    public bool IsHappyMaxed => _happiness >= 1000;

    // --- IDamagotchi �������̽� ���� (�ൿ) ---
    public void Feed(int amount)
    {
        _hunger = Mathf.Max(0, _hunger - amount); // ����� ���� (0 �̸����� �� ��������)
        _health = Mathf.Min(100, _health + (amount / 2)); // ü�� �ణ ���� (100 �ʰ� �� �ǰ�)
        Debug.Log($"��ѱ⿡�� {amount}��ŭ ���̸� ����ϴ�. ���� �����: {_hunger}");
    }

    public void Play(int duration)
    {
        _happiness = Mathf.Min(100, _happiness + (duration * 2)); // �ູ ����
        _hunger = Mathf.Min(100, _hunger + (duration / 5)); // ��鼭 �������
        Debug.Log($"��ѱ�� {duration}�� ��ҽ��ϴ�. ���� �ູ: {_happiness}");
    }

    public void Sleep(int duration)
    {
        _health = Mathf.Min(100, _health + (duration * 3)); // ü�� ȸ��
        _happiness = Mathf.Max(0, _happiness - (duration / 3)); // �ڸ� �� �ɽ����� �� ����
        Debug.Log($"��ѱⰡ {duration}�� �������ϴ�. ���� ü��: {_health}");
    }

    public void UpdateStatus()
    {
        // �ð��� ������ ���� ���� ��ȭ (��: �� �ʸ��� ȣ��)
        _hunger = Mathf.Min(100, _hunger + 1); // �ð��� �������� �������
        if (_hunger >= 50)
        {
            _happiness = Mathf.Max(0, _happiness); // �ð��� �������� �ູ ����
        }
        else
        {
            _happiness = Mathf.Max(0, _happiness - 2);
        }
        _health = Mathf.Max(0, _health - (_hunger > 80 ? 2 : 1)); // ������� ü�� �� ����
        Debug.Log($"��ѱ� ���� ������Ʈ: ü��={_health}, �����={_hunger}, �ູ={_happiness}");
    }

    public bool IsDead()
    {
        return _health <= 0 || _hunger >= 100; // ü���� ���ų� �ʹ� ������� ���
    }

    // --- MonoBehaviour ����������Ŭ �Լ� (���� ����) ---
    private void Start()
    {

    }

    private void Update()
    {

    }
}
