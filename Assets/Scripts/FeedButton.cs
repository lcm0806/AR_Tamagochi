using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedButton : MonoBehaviour
{
    // �� ��ư�� ��ȣ�ۿ��� IDamagotchi �ν��Ͻ� ����
    private IDamagotchi targetTamagotchi;

    // ���̸� �� ��
    public int feedAmount = 10;

    // �� ��ũ��Ʈ�� Ȱ��ȭ�� �� (���� ������ ��) TamagotchiDisplayManager�κ��� �����͸� �޾ƿ� �غ� �մϴ�.
    // Public �Լ��� ���� �ܺο��� �� �ٸ���ġ ���� �����͸� ������ �� �ְ� �մϴ�.
    public void SetTargetTamagotchi(IDamagotchi tamaData)
    {
        targetTamagotchi = tamaData;
        Debug.Log($"FeedButton ��ũ��Ʈ�� �ٸ���ġ '{targetTamagotchi.Name}' �����Ͱ� �����Ǿ����ϴ�.");
    }

    // ������ ���̸� �ִ� ����. �� �Լ��� UI ��ư Ŭ�� �̺�Ʈ�� 3D �� Ŭ�� �̺�Ʈ�� ����� �� �ֽ��ϴ�.
    public void OnFeedClicked()
    {
        if (targetTamagotchi == null)
        {
            Debug.LogError("FeedButton: ��ȣ�ۿ��� �ٸ���ġ �����Ͱ� �������� �ʾҽ��ϴ�!");
            return;
        }

        Debug.Log($"FeedButton: {targetTamagotchi.Name}���� �����ֱ� ��ư�� Ŭ���Ǿ����ϴ�.");
        targetTamagotchi.Feed(feedAmount);

        // ���̸� �� �� UI�� ������Ʈ�ؾ� �ϹǷ�, TamagotchiDisplayManager�� �˸�
        // ���� �����ϰų�, �̺�Ʈ�� �߻���Ű�ų�, �ƴϸ� Update()���� �ֱ������� ���ŵǵ��� �� �� �ֽ��ϴ�.
        // ���⼭�� TamagotchiDisplayManager�� Update()���� �ֱ������� UI�� �����Ѵٰ� �����մϴ�.
        // �Ǵ� TamagotchiDisplayManager�� Public UpdateUI() �Լ��� ����� ���⼭ ȣ���� ���� �ֽ��ϴ�.
        // ��: FindObjectOfType<TamagotchiDisplayManager>()?.UpdateUI(); (���ɿ� ���� ����)
        // �� ���� ����� TamagotchiDisplayManager�� currentTamagotchiData�� ������ �����ϵ��� �ϴ� ���Դϴ�.
    }

    // (���� ����) 3D �� ��ü�� Ŭ������ �� ���̸� �ַ��� Collider�� OnMouseDown()�� ����մϴ�.
    // ����� UI ��ư Ŭ�� ����̹Ƿ� �� �κ��� �ʿ� ���� �� �ֽ��ϴ�.
    /*
    void OnMouseDown()
    {
        OnFeedClicked(); // 3D �� Ŭ�� �� ���� �ֱ� �Լ� ȣ��
    }
    */
}
