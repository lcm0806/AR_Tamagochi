// GameManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public IDamagotchi SelectedTamagotchiInstance { get; private set; }
    public GameObject SelectedTamagotchiPrefab { get; private set; }

    // --- ���� �߰��� �κ� ---
    public List<GameObject> CapturedTamagotchiPrefabs { get; private set; } = new List<GameObject>(); // ��ȹ�� �ٸ���ġ ������ ����Ʈ

    // ������ �ٽ� ������ �� ȣ��Ǵ��� Ȯ�� (�ɼ�)
    private bool isGameRestarted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log($"[GameManager] Awake: Instance set. Object '{gameObject.name}' will not be destroyed on load.");
        }
        else if (Instance != this) // �̹� �ٸ� �ν��Ͻ��� �����ϸ� ���� �ν��Ͻ� �ı�
        {
            Debug.LogWarning($"[GameManager] Awake: Duplicate GameManager found. Destroying '{gameObject.name}'.");
            Destroy(gameObject);
        }
        // �̺�Ʈ�� Awake���� �����ϴ� ���� ���� �����մϴ�.
        SceneManager.sceneLoaded -= OnSceneLoaded; // �ߺ� ���� ����
        SceneManager.sceneLoaded += OnSceneLoaded;

        // ���� �Ŵ����� ó�� ������ �� ��ȹ ����Ʈ �ʱ�ȭ (Ȥ�� ����� ������ �ҷ�����)
        if (CapturedTamagotchiPrefabs == null) // null üũ�� �� ���� (�ʱ�ȭ��)
        {
            CapturedTamagotchiPrefabs = new List<GameObject>();
        }
    }

    private void OnDestroy()
    {
        // ������Ʈ�� �ı��� ���� ���� ���� (�ߺ� ���� ���� ����)
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] SceneLoaded: Scene '{scene.name}' loaded. SelectedPrefab: {(SelectedTamagotchiPrefab != null ? SelectedTamagotchiPrefab.name : "NULL")}");
        // ���⼭ SelectedTamagotchiPrefab�� ���¸� �ٽ� �ѹ� Ȯ��

        // �� ������ ���ƿ��� ��, ��ȹ ���� �α� ��� (Ȯ�ο�)
        if (scene.name == "MapScene") // "MapScene"�� ���� �� �� �̸����� �����ϼ���.
        {
            Debug.Log($"[GameManager] ���� ��ȹ�� �ٸ���ġ ��: {CapturedTamagotchiPrefabs.Count}");
            foreach (var prefab in CapturedTamagotchiPrefabs)
            {
                Debug.Log($"[GameManager] ��ȹ�� �ٸ���ġ: {prefab.name}");
            }
        }
    }

    public void SetSelectedTamagotchi(IDamagotchi tamagotchiInstance, GameObject tamagotchiPrefab)
    {
        SelectedTamagotchiInstance = tamagotchiInstance;
        SelectedTamagotchiPrefab = tamagotchiPrefab;
    }

    public void ClearSelectedTamagotchi()
    {
        SelectedTamagotchiInstance = null;
        SelectedTamagotchiPrefab = null;
        Debug.Log("[GameManager] Selected information cleared.");
    }

    // --- ���� �߰��� �Լ� ---
    public void AddCapturedTamagotchi(GameObject capturedPrefab)
    {
        if (capturedPrefab != null && !CapturedTamagotchiPrefabs.Contains(capturedPrefab))
        {
            CapturedTamagotchiPrefabs.Add(capturedPrefab);
            Debug.Log($"[GameManager] �ٸ���ġ '{capturedPrefab.name}'��(��) ��ȹ ��Ͽ� �߰��Ǿ����ϴ�.");
        }
        else if (capturedPrefab == null)
        {
            Debug.LogWarning("[GameManager] AddCapturedTamagotchi: �߰��Ϸ��� �������� null�Դϴ�.");
        }
        else if (CapturedTamagotchiPrefabs.Contains(capturedPrefab))
        {
            Debug.LogWarning($"[GameManager] �ٸ���ġ '{capturedPrefab.name}'��(��) �̹� ��ȹ ��Ͽ� �ֽ��ϴ�.");
        }
    }

    // ��ȹ�� �ٸ���ġ���� Ȯ���ϴ� �Լ�
    public bool IsTamagotchiCaptured(GameObject prefab)
    {
        return CapturedTamagotchiPrefabs.Contains(prefab);
    }

    // ��� ��ȹ ����� �ʱ�ȭ�ϴ� �Լ� (��: �� ���� ���� ��)
    public void ClearAllCapturedTamagotchi()
    {
        CapturedTamagotchiPrefabs.Clear();
        Debug.Log("[GameManager] ��� ��ȹ �ٸ���ġ ����� �ʱ�ȭ�Ǿ����ϴ�.");
    }
    // --- End ���� �߰��� �Լ� ---
}