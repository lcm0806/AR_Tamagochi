using UnityEngine; // Color�� ����ϱ� ���� �ʿ�

// 'I'�� �������̽����� ��Ÿ���� �Ϲ����� ��� ��Ģ�Դϴ�.
public interface IDamagotchi
{
    // --- �Ӽ� (Properties) ---
    // �� �ٸ���ġ�� ������ �̸�
    string Name { get; }
    // �ٸ���ġ�� ID �Ǵ� ���� ��ȣ
    int ID { get; }

    // --- �ٸ���ġ ���� (Properties) ---
    // ���� ü�� (�б� ���� �Ǵ� ���� ����)
    int Health { get; }
    // ���� ������ (�б� ���� �Ǵ� ���� ����)
    int Hunger { get; }
    // ���� �ູ�� (�б� ���� �Ǵ� ���� ����)
    int Happiness { get; }
    // �ູ���� ��������
    bool IsHappyMaxed { get; }

    // --- �ൿ (Methods) ---
    // ���̸� �ִ� �ൿ
    void Feed(int amount);
    // ����ִ� �ൿ
    void Play(int duration);
    // ���� �ൿ
    void Sleep(int duration);
    // �ð��� ������ ���� ���� ��ȭ�� ������Ʈ�ϴ� �ൿ
    void UpdateStatus();
    // �ٸ���ġ�� �׾����� ����
    bool IsDead();
}