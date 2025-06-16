using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour, IDamagotchi
{
    public string Name => "������";
    public int ID => 002;

    [Header("Status")]
    [SerializeField] private int _health = 100;
    [SerializeField] private int _hunger = 50;
    [SerializeField] private int _happiness = 80;

    public int Health { get { return _health; } }
    public int Hunger { get { return _hunger; } }
    public int Happiness { get { return _happiness; } }
    public bool IsHappyMaxed => _happiness >= 100;

    // --- IDamagotchi �������̽� ���� (�ൿ) ---
    public void Feed(int amount)
    {
        _hunger = Mathf.Max(0, _hunger - amount); // ����� ���� (0 �̸����� �� ��������)
        _health = Mathf.Min(100, _health + (amount / 2)); // ü�� �ణ ���� (100 �ʰ� �� �ǰ�)
        Debug.Log($"���������� {amount}��ŭ ���̸� ����ϴ�. ���� �����: {_hunger}");
    }

    public void Play(int duration)
    {
        _happiness = Mathf.Min(100, _happiness + (duration * 2)); // �ູ ����
        _hunger = Mathf.Min(100, _hunger + (duration / 5)); // ��鼭 �������
        Debug.Log($"�������� {duration}�� ��ҽ��ϴ�. ���� �ູ: {_happiness}");
    }

    public void Sleep(int duration)
    {
        _health = Mathf.Min(100, _health + (duration * 3)); // ü�� ȸ��
        _happiness = Mathf.Max(0, _happiness - (duration / 3)); // �ڸ� �� �ɽ����� �� ����
        Debug.Log($"�������� {duration}�� �������ϴ�. ���� ü��: {_health}");
    }

    public void UpdateStatus()
    {
        // �ð��� ������ ���� ���� ��ȭ (��: �� �ʸ��� ȣ��)
        _hunger = Mathf.Min(100, _hunger + 1); // �ð��� �������� �������
        _happiness = Mathf.Max(0, _happiness); // �ð��� �������� �ູ ����
        _health = Mathf.Max(0, _health - (_hunger > 80 ? 2 : 1)); // ������� ü�� �� ����
        Debug.Log($"������ ���� ������Ʈ: ü��={_health}, �����={_hunger}, �ູ={_happiness}");
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
