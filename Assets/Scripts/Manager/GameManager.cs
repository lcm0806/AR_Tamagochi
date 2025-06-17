using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public IDamagotchi SelectedTamagotchiInstance { get; private set; }
    public GameObject SelectedTamagotchiPrefab { get; private set; }

    // CapturedTamagotchiPrefabs ����Ʈ �ʱ�ȭ (�� �ν��Ͻ����� �ʱ�ȭ��)
    public List<GameObject> CapturedTamagotchiPrefabs { get; private set; } = new List<GameObject>();

    // ���� �߰�: ���� AR ���� ��ġ�� �ٸ���ġ �ν��Ͻ����� �����ϴ� ����Ʈ
    private List<GameObject> _placedTamagotchiInstances = new List<GameObject>();

    private void Awake()
    {

        if (Instance == null)
        {
            // ù ��° GameManager �ν��Ͻ��� ������ ��
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ������Ʈ�� �� ��ȯ �� �ı����� ����
        }
        else if (Instance != this) // �̹� �ٸ� �ν��Ͻ��� �����ϸ� (�̰� �����Դϴ�!)
        {
            Destroy(gameObject);
            return; // �޼��� �����Ͽ� �� �̻� �ڵ� ���� ����
        }

        // �� �ε� �̺�Ʈ ���� (�ߺ� ���� ������ ���� ���� �� �ٽ� ����)
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        // CapturedTamagotchiPrefabs ����Ʈ�� null�� ���� ��������, ������ ���� �ʱ�ȭ ����
        if (CapturedTamagotchiPrefabs == null)
        {
            CapturedTamagotchiPrefabs = new List<GameObject>();
        }
    }

    private void OnDestroy()
    {
        // �� ������Ʈ�� ���� �̱��� Instance��� �̺�Ʈ ���� ����
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[{name}] OnSceneLoaded: �� '{scene.name}' �ε��.");
        Debug.Log($"[{name}] OnSceneLoaded: ���� ��ȹ�� �ٸ���ġ ������ ��: {CapturedTamagotchiPrefabs.Count}");
        Debug.Log($"[{name}] OnSceneLoaded: ���� ��ġ�� �ٸ���ġ �ν��Ͻ� ��: {_placedTamagotchiInstances.Count}");

        // ��� ��ġ�� �ٸ���ġ �ν��Ͻ��� Ȱ��ȭ ���¸� ������Ʈ
        foreach (GameObject tamagotchiInstance in _placedTamagotchiInstances)
        {
            if (tamagotchiInstance != null)
            {
                // ���� ���� ���� AR ���̶�� Ȱ��ȭ�ϰ�, �׷��� �ʴٸ� ��Ȱ��ȭ
                bool isActiveInARScene = (scene.name == "ARScene"); // AR �� �̸� Ȯ��
                tamagotchiInstance.SetActive(isActiveInARScene);

                if (isActiveInARScene)
                {
                    // AR ������ ���ƿ��� ��, �ٸ���ġ�� ����� ���̵��� �ʿ��� �߰� ���� (���� ����)
                    // ���� AR ���� �簳 �� AR ������ ������ �ٲ�� ��ġ�� Ʋ�����ٸ�,
                    // ���⼭ ������ �����ߴ� ��� ��ġ ���� ����� �ٽ� �����ϴ� ������ �߰��� �� �ֽ��ϴ�.
                    // ������ DontDestroyOnLoad�� ������Ʈ�� ���� ��ġ�� �����ϹǷ�,
                    // ��κ��� ��� �⺻������ �� �۵��� ���Դϴ�.
                    Debug.Log($"[{name}] '{tamagotchiInstance.name}' Ȱ��ȭ�� (��: {scene.name})");
                }
                else
                {
                    Debug.Log($"[{name}] '{tamagotchiInstance.name}' ��Ȱ��ȭ�� (��: {scene.name})");
                }
            }
        }

        // GPSScene���� ���ƿ��� ��, ��ȹ�� �ٸ���ġ ������ ����� ���� �α�
        if (scene.name == "GPSScene")
        {
            Debug.Log($"[{name}] GPSScene �ε� �Ϸ�. ��ȹ�� �ٸ���ġ ������ ���:");
            if (CapturedTamagotchiPrefabs.Count > 0)
            {
                foreach (var prefab in CapturedTamagotchiPrefabs)
                {
                    Debug.Log($"[{name}] - {prefab.name}");
                }
            }
            else
            {
                Debug.Log($"[{name}] - ��ȹ�� �ٸ���ġ �������� �����ϴ�.");
            }
        }
    }

    // --- ��Ÿ �Լ��� (���� ����) ---
    public void SetSelectedTamagotchi(IDamagotchi tamagotchiInstance, GameObject tamagotchiPrefab)
    {
        SelectedTamagotchiInstance = tamagotchiInstance;
        SelectedTamagotchiPrefab = tamagotchiPrefab;
    }

    public void ClearSelectedTamagotchi()
    {
        SelectedTamagotchiInstance = null;
        SelectedTamagotchiPrefab = null;
    }

    public void AddCapturedTamagotchi(GameObject capturedPrefab)
    {
        if (capturedPrefab != null)
        {
            if (!CapturedTamagotchiPrefabs.Contains(capturedPrefab))
            {
                CapturedTamagotchiPrefabs.Add(capturedPrefab);
            }
        }
    }

    // ���� �߰�: AR ���� ��ġ�� ���� �ٸ���ġ �ν��Ͻ��� ����Ʈ�� �߰�
    public void AddPlacedTamagotchiInstance(GameObject instance)
    {
        if (instance != null && !_placedTamagotchiInstances.Contains(instance))
        {
            _placedTamagotchiInstances.Add(instance);
            Debug.Log($"[{name}] Placed Tamagotchi Instance Added: {instance.name}. Total placed: {_placedTamagotchiInstances.Count}");
        }
    }

    public bool IsTamagotchiCaptured(GameObject prefab)
    {
        return CapturedTamagotchiPrefabs.Contains(prefab);
    }

    public void ClearAllCapturedTamagotchi()
    {
        CapturedTamagotchiPrefabs.Clear();
    }

    // ��ġ�� ��� �ٸ���ġ �ν��Ͻ��� �����ϰ� ����Ʈ�� ���� �Լ� (��: ���� ����� ��)
    public void ClearAllPlacedTamagotchiInstances()
    {
        foreach (GameObject instance in _placedTamagotchiInstances)
        {
            if (instance != null)
            {
                Destroy(instance);
            }
        }
        _placedTamagotchiInstances.Clear();
        Debug.Log($"[{name}] All Placed Tamagotchi Instances cleared and destroyed.");
    }
}