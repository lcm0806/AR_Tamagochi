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

    private void Awake()
    {
        // � GameManager GameObject�� Awake�� ȣ���ߴ��� Ȯ��
        Debug.Log($"[{name}] Awake ȣ���. ���� �̱��� Instance: {(Instance != null ? Instance.name : "NULL")}");

        if (Instance == null)
        {
            // ù ��° GameManager �ν��Ͻ��� ������ ��
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ������Ʈ�� �� ��ȯ �� �ı����� ����
            Debug.Log($"[{name}] Awake: GameManager �̱��� ���� �� DontDestroyOnLoad ȣ���.");
        }
        else if (Instance != this) // �̹� �ٸ� �ν��Ͻ��� �����ϸ� (�̰� �����Դϴ�!)
        {
            // �ߺ��� GameManager �ν��Ͻ��� �߰������Ƿ�, �ڽ��� �ı�
            Debug.LogWarning($"[{name}] Awake: �ߺ��� GameManager �ν��Ͻ� '{gameObject.name}'�� �����Ǿ� �ı��մϴ�. (���� GameManager '{Instance.name}' ����)");
            Destroy(gameObject);
            return; // �޼��� �����Ͽ� �� �̻� �ڵ� ���� ����
        }

        // �� �ε� �̺�Ʈ ���� (�ߺ� ���� ������ ���� ���� �� �ٽ� ����)
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log($"[{name}] Awake: SceneLoaded �̺�Ʈ ���� �Ϸ�.");

        // CapturedTamagotchiPrefabs ����Ʈ�� null�� ���� ��������, ������ ���� �ʱ�ȭ ����
        if (CapturedTamagotchiPrefabs == null)
        {
            CapturedTamagotchiPrefabs = new List<GameObject>();
            Debug.Log($"[{name}] CapturedTamagotchiPrefabs ����Ʈ�� �ʱ�ȭ�Ǿ����ϴ�.");
        }
    }

    private void OnDestroy()
    {
        Debug.Log($"[{name}] OnDestroy ȣ���.");
        // �� ������Ʈ�� ���� �̱��� Instance��� �̺�Ʈ ���� ����
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Debug.Log($"[{name}] OnDestroy: SceneLoaded �̺�Ʈ ���� ������.");
            // ���⼭ Instance = null; �� ���� �ʴ� ���� �Ϲ����Դϴ�. (���� ���� �ó�����)
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �� �α״� �׻� ����ִ� GameManager �ν��Ͻ��� ���� ���Դϴ�.
        Debug.Log($"[{name}] OnSceneLoaded: �� '{scene.name}' �ε��. SelectedPrefab: {(SelectedTamagotchiPrefab != null ? SelectedTamagotchiPrefab.name : "NULL")}");
        Debug.Log($"[{name}] OnSceneLoaded: ���� ��ȹ�� �ٸ���ġ ��: {CapturedTamagotchiPrefabs.Count}");

        // GPSScene���� ���ƿ��� ��, ��ȹ�� �ٸ���ġ ����� ���� �α�
        if (scene.name == "GPSScene")
        {
            Debug.Log($"[{name}] GPSScene �ε� �Ϸ�. ��ȹ�� �ٸ���ġ ���:");
            if (CapturedTamagotchiPrefabs.Count > 0)
            {
                foreach (var prefab in CapturedTamagotchiPrefabs)
                {
                    Debug.Log($"[{name}] - {prefab.name}");
                }
            }
            else
            {
                Debug.Log($"[{name}] - ��ȹ�� �ٸ���ġ�� �����ϴ�.");
            }
        }
    }

    // --- ��Ÿ �Լ��� (���� ����) ---
    public void SetSelectedTamagotchi(IDamagotchi tamagotchiInstance, GameObject tamagotchiPrefab)
    {
        SelectedTamagotchiInstance = tamagotchiInstance;
        SelectedTamagotchiPrefab = tamagotchiPrefab;
        Debug.Log($"[{name}] SetSelectedTamagotchi: �ν��Ͻ� '{tamagotchiInstance?.Name}', ������ '{tamagotchiPrefab?.name}' ������.");
    }

    public void ClearSelectedTamagotchi()
    {
        SelectedTamagotchiInstance = null;
        SelectedTamagotchiPrefab = null;
        Debug.Log($"[{name}] ���� ������ �ʱ�ȭ�Ǿ����ϴ�.");
    }

    public void AddCapturedTamagotchi(GameObject capturedPrefab)
    {
        if (capturedPrefab != null)
        {
            if (!CapturedTamagotchiPrefabs.Contains(capturedPrefab))
            {
                CapturedTamagotchiPrefabs.Add(capturedPrefab);
                Debug.Log($"[{name}] �ٸ���ġ '{capturedPrefab.name}'��(��) ��ȹ ��Ͽ� �߰��Ǿ����ϴ�. �� {CapturedTamagotchiPrefabs.Count}��.");
            }
            else
            {
                Debug.LogWarning($"[{name}] �ٸ���ġ '{capturedPrefab.name}'��(��) �̹� ��ȹ ��Ͽ� �ֽ��ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning($"[{name}] AddCapturedTamagotchi: �߰��Ϸ��� �������� null�Դϴ�.");
        }
    }

    public bool IsTamagotchiCaptured(GameObject prefab)
    {
        return CapturedTamagotchiPrefabs.Contains(prefab);
    }

    public void ClearAllCapturedTamagotchi()
    {
        CapturedTamagotchiPrefabs.Clear();
        Debug.Log($"[{name}] ��� ��ȹ �ٸ���ġ ����� �ʱ�ȭ�Ǿ����ϴ�.");
    }
}