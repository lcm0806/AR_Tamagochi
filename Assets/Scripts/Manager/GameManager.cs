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
    public List<GameObject> PlacedTamagotchiInstances = new List<GameObject>();

    private int _currentPlacementIndex = 0;

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
        Debug.Log($"[{name}] OnSceneLoaded: ���� ��ġ�� �ٸ���ġ �ν��Ͻ� ��: {PlacedTamagotchiInstances.Count}");

        // ��� ��ġ�� �ٸ���ġ �ν��Ͻ��� Ȱ��ȭ ���¸� ������Ʈ
        foreach (GameObject tamagotchiInstance in PlacedTamagotchiInstances)
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

    // ARPlacementManager���� ȣ���Ͽ� ���� ��ġ�� �ٸ���ġ�� �������� ������
    public GameObject GetNextTamagotchiToPlace()
    {
        if (CapturedTamagotchiPrefabs == null || CapturedTamagotchiPrefabs.Count == 0)
        {
            Debug.LogWarning("[GameManager] GetNextTamagotchiToPlace: ��ȹ�� �ٸ���ġ�� �����ϴ�.");
            return null;
        }

        if (_currentPlacementIndex < CapturedTamagotchiPrefabs.Count)
        {
            GameObject nextTamagotchi = CapturedTamagotchiPrefabs[_currentPlacementIndex];
            Debug.Log($"[GameManager] ���� ��ġ�� �ٸ���ġ: {nextTamagotchi.name} (�ε���: {_currentPlacementIndex})");
            return nextTamagotchi;
        }
        else
        {
            Debug.Log($"[GameManager] ��� �ٸ���ġ�� ��ġ�߽��ϴ�. ���� �ε���: {_currentPlacementIndex}, �� ����: {CapturedTamagotchiPrefabs.Count}");
            return null; // ��� �ٸ���ġ�� �� ��ġ����
        }
    }

    // �ٸ���ġ�� ��ġ�� �� ���� ������ �ε��� ����
    public void AdvancePlacementIndex()
    {
        _currentPlacementIndex++;
        Debug.Log($"[GameManager] ��ġ �ε��� ����. ���� �ε���: {_currentPlacementIndex}");
    }

    // ��ġ �ε����� �ʱ�ȭ (��: ���� ���� �� �Ǵ� Ư�� ��Ȳ����)
    public void ResetPlacementIndex()
    {
        _currentPlacementIndex = 0;
        Debug.Log("[GameManager] ��ġ �ε��� �ʱ�ȭ��.");
    }

    public int GetCurrentPlacementIndex()
    {
        return _currentPlacementIndex;
    }

    public int GetTotalCapturedTamagotchisCount()
    {
        return CapturedTamagotchiPrefabs.Count;
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
        if (instance != null && !PlacedTamagotchiInstances.Contains(instance))
        {
            PlacedTamagotchiInstances.Add(instance);
            Debug.Log($"[{name}] Placed Tamagotchi Instance Added: {instance.name}. Total placed: {PlacedTamagotchiInstances.Count}");
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
        foreach (GameObject instance in PlacedTamagotchiInstances)
        {
            if (instance != null)
            {
                Destroy(instance);
            }
        }
        PlacedTamagotchiInstances.Clear();
        Debug.Log($"[{name}] All Placed Tamagotchi Instances cleared and destroyed.");
    }
}