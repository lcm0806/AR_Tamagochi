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
        else if (Instance != this) // 이미 다른 인스턴스가 존재하면 현재 인스턴스 파괴
        {
            Debug.LogWarning($"[GameManager] Awake: Duplicate GameManager found. Destroying '{gameObject.name}'.");
            Destroy(gameObject);
        }
        // 이벤트를 Awake에서 구독하는 것이 가장 안전합니다.
        SceneManager.sceneLoaded -= OnSceneLoaded; // 중복 구독 방지
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // 오브젝트가 파괴될 때만 구독 해제 (중복 구독 해제 방지)
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] SceneLoaded: Scene '{scene.name}' loaded. SelectedPrefab: {(SelectedTamagotchiPrefab != null ? SelectedTamagotchiPrefab.name : "NULL")}");
        // 여기서 SelectedTamagotchiPrefab의 상태를 다시 한번 확인
    }

    public void SetSelectedTamagotchi(IDamagotchi tamagotchiInstance, GameObject tamagotchiPrefab)
    {
        SelectedTamagotchiInstance = tamagotchiInstance;
        SelectedTamagotchiPrefab = tamagotchiPrefab;

        if (tamagotchiPrefab == null)
        {
            Debug.LogError("[GameManager] SetSelectedTamagotchi: 'tamagotchiPrefab'이 null로 전달되었습니다!");
        }
        else
        {
            Debug.Log($"[GameManager] SetSelectedTamagotchi: 프리팹 '{tamagotchiPrefab.name}'이 성공적으로 저장되었습니다.");
        }
    }

    public void ClearSelectedTamagotchi()
    {
        SelectedTamagotchiInstance = null;
        SelectedTamagotchiPrefab = null;
        Debug.Log("[GameManager] Selected information cleared.");
    }
}