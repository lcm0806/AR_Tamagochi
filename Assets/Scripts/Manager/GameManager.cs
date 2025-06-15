using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // ��𼭵� ���� �����ϵ��� static �ν��Ͻ�

    // ���� ������ �ѱ� �����͸� ���⿡ �����մϴ�.
    // ��: Ŭ���� �ٸ���ġ�� ���� (�̸�, ID ��), �ɷ�ġ ��
    public string selectedTamagotchiName;
    public int selectedTamagotchiID;
    public Color selectedTamagotchiColor;
    // �ʿ信 ���� �� ���� �����͸� �߰��ϼ���. (��: public TamagotchiData selectedTamagotchiData;)

    private void Awake()
    {
        // �̱��� ���� ����:
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ������Ʈ�� ���� �ٲ� �ı����� ����
        }
        else
        {
            // �̹� �ν��Ͻ��� �����ϸ� ���� ������ ������Ʈ�� �ı� (�ߺ� ����)
            Destroy(gameObject);
        }

        // �� �ε�� ������ ������ �ʱ�ȭ (���� ����, �ʿ信 ����)
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // �� �ε� �̺�Ʈ ���� ���� (�޸� ���� ����)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ���� �ε�� ������ ȣ��Ǵ� �Լ� (���� ����)
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"�� '{scene.name}'�� �ε�Ǿ����ϴ�.");
        // ���� ���� ������ Ư�� �����Ͱ� �ʿ� ���ٸ� ���⼭ �ʱ�ȭ�� �� �ֽ��ϴ�.
        // ��: selectedTamagotchiName = null;
    }

    // Ŭ���� �ٸ���ġ ���� ���� �Լ�
    public void SetSelectedTamagotchi(string tamagotchiName, int tamagotchiID, Color tamagotchiColor)
    {
        selectedTamagotchiName = tamagotchiName;
        selectedTamagotchiID = tamagotchiID;
        selectedTamagotchiColor = tamagotchiColor;
        Debug.Log($"GameManager�� ���õ� �ٸ���ġ ���� �����: {selectedTamagotchiName}");
    }
}
