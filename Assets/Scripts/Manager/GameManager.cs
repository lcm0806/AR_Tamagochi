// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public IDamagotchi SelectedTamagotchiInstance { get; private set; }
    public GameObject SelectedTamagotchiPrefab { get; private set; }

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
    }

    public void SetSelectedTamagotchi(IDamagotchi tamagotchiInstance, GameObject tamagotchiPrefab)
    {
        SelectedTamagotchiInstance = tamagotchiInstance;
        SelectedTamagotchiPrefab = tamagotchiPrefab;

        if (tamagotchiPrefab == null)
        {
            Debug.LogError("[GameManager] SetSelectedTamagotchi: 'tamagotchiPrefab'�� null�� ���޵Ǿ����ϴ�!");
        }
        else
        {
            Debug.Log($"[GameManager] SetSelectedTamagotchi: ������ '{tamagotchiPrefab.name}'�� ���������� ����Ǿ����ϴ�.");
        }
    }

    public void ClearSelectedTamagotchi()
    {
        SelectedTamagotchiInstance = null;
        SelectedTamagotchiPrefab = null;
        Debug.Log("[GameManager] Selected information cleared.");
    }
}