// GameManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public IDamagotchi SelectedTamagotchiInstance { get; private set; }
    public GameObject SelectedTamagotchiPrefab { get; private set; }

    // --- 새로 추가된 부분 ---
    public List<GameObject> CapturedTamagotchiPrefabs { get; private set; } = new List<GameObject>(); // 포획된 다마고치 프리팹 리스트

    // 게임을 다시 시작할 때 호출되는지 확인 (옵션)
    private bool isGameRestarted = false;

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

        // 게임 매니저가 처음 생성될 때 포획 리스트 초기화 (혹은 저장된 데이터 불러오기)
        if (CapturedTamagotchiPrefabs == null) // null 체크는 한 번만 (초기화시)
        {
            CapturedTamagotchiPrefabs = new List<GameObject>();
        }
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

        // 맵 씬으로 돌아왔을 때, 포획 정보 로그 출력 (확인용)
        if (scene.name == "MapScene") // "MapScene"은 실제 맵 씬 이름으로 변경하세요.
        {
            Debug.Log($"[GameManager] 현재 포획된 다마고치 수: {CapturedTamagotchiPrefabs.Count}");
            foreach (var prefab in CapturedTamagotchiPrefabs)
            {
                Debug.Log($"[GameManager] 포획된 다마고치: {prefab.name}");
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

    // --- 새로 추가된 함수 ---
    public void AddCapturedTamagotchi(GameObject capturedPrefab)
    {
        if (capturedPrefab != null && !CapturedTamagotchiPrefabs.Contains(capturedPrefab))
        {
            CapturedTamagotchiPrefabs.Add(capturedPrefab);
            Debug.Log($"[GameManager] 다마고치 '{capturedPrefab.name}'이(가) 포획 목록에 추가되었습니다.");
        }
        else if (capturedPrefab == null)
        {
            Debug.LogWarning("[GameManager] AddCapturedTamagotchi: 추가하려는 프리팹이 null입니다.");
        }
        else if (CapturedTamagotchiPrefabs.Contains(capturedPrefab))
        {
            Debug.LogWarning($"[GameManager] 다마고치 '{capturedPrefab.name}'은(는) 이미 포획 목록에 있습니다.");
        }
    }

    // 포획된 다마고치인지 확인하는 함수
    public bool IsTamagotchiCaptured(GameObject prefab)
    {
        return CapturedTamagotchiPrefabs.Contains(prefab);
    }

    // 모든 포획 목록을 초기화하는 함수 (예: 새 게임 시작 시)
    public void ClearAllCapturedTamagotchi()
    {
        CapturedTamagotchiPrefabs.Clear();
        Debug.Log("[GameManager] 모든 포획 다마고치 목록이 초기화되었습니다.");
    }
    // --- End 새로 추가된 함수 ---
}