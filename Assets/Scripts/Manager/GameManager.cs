using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public IDamagotchi SelectedTamagotchiInstance { get; private set; }
    public GameObject SelectedTamagotchiPrefab { get; private set; }

    // CapturedTamagotchiPrefabs 리스트 초기화 (새 인스턴스마다 초기화됨)
    public List<GameObject> CapturedTamagotchiPrefabs { get; private set; } = new List<GameObject>();

    private void Awake()
    {
        // 어떤 GameManager GameObject가 Awake를 호출했는지 확인
        Debug.Log($"[{name}] Awake 호출됨. 현재 싱글톤 Instance: {(Instance != null ? Instance.name : "NULL")}");

        if (Instance == null)
        {
            // 첫 번째 GameManager 인스턴스가 생성될 때
            Instance = this;
            DontDestroyOnLoad(gameObject); // 이 오브젝트는 씬 전환 시 파괴되지 않음
            Debug.Log($"[{name}] Awake: GameManager 싱글톤 생성 및 DontDestroyOnLoad 호출됨.");
        }
        else if (Instance != this) // 이미 다른 인스턴스가 존재하면 (이게 문제입니다!)
        {
            // 중복된 GameManager 인스턴스를 발견했으므로, 자신을 파괴
            Debug.LogWarning($"[{name}] Awake: 중복된 GameManager 인스턴스 '{gameObject.name}'가 감지되어 파괴합니다. (기존 GameManager '{Instance.name}' 유지)");
            Destroy(gameObject);
            return; // 메서드 종료하여 더 이상 코드 실행 방지
        }

        // 씬 로드 이벤트 구독 (중복 구독 방지를 위해 해제 후 다시 구독)
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log($"[{name}] Awake: SceneLoaded 이벤트 구독 완료.");

        // CapturedTamagotchiPrefabs 리스트가 null인 경우는 없겠지만, 안전을 위해 초기화 로직
        if (CapturedTamagotchiPrefabs == null)
        {
            CapturedTamagotchiPrefabs = new List<GameObject>();
            Debug.Log($"[{name}] CapturedTamagotchiPrefabs 리스트가 초기화되었습니다.");
        }
    }

    private void OnDestroy()
    {
        Debug.Log($"[{name}] OnDestroy 호출됨.");
        // 이 오브젝트가 현재 싱글톤 Instance라면 이벤트 구독 해제
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Debug.Log($"[{name}] OnDestroy: SceneLoaded 이벤트 구독 해제됨.");
            // 여기서 Instance = null; 을 하지 않는 것이 일반적입니다. (게임 종료 시나리오)
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 이 로그는 항상 살아있는 GameManager 인스턴스가 찍을 것입니다.
        Debug.Log($"[{name}] OnSceneLoaded: 씬 '{scene.name}' 로드됨. SelectedPrefab: {(SelectedTamagotchiPrefab != null ? SelectedTamagotchiPrefab.name : "NULL")}");
        Debug.Log($"[{name}] OnSceneLoaded: 현재 포획된 다마고치 수: {CapturedTamagotchiPrefabs.Count}");

        // GPSScene으로 돌아왔을 때, 포획된 다마고치 목록을 상세히 로그
        if (scene.name == "GPSScene")
        {
            Debug.Log($"[{name}] GPSScene 로드 완료. 포획된 다마고치 목록:");
            if (CapturedTamagotchiPrefabs.Count > 0)
            {
                foreach (var prefab in CapturedTamagotchiPrefabs)
                {
                    Debug.Log($"[{name}] - {prefab.name}");
                }
            }
            else
            {
                Debug.Log($"[{name}] - 포획된 다마고치가 없습니다.");
            }
        }
    }

    // --- 기타 함수들 (변경 없음) ---
    public void SetSelectedTamagotchi(IDamagotchi tamagotchiInstance, GameObject tamagotchiPrefab)
    {
        SelectedTamagotchiInstance = tamagotchiInstance;
        SelectedTamagotchiPrefab = tamagotchiPrefab;
        Debug.Log($"[{name}] SetSelectedTamagotchi: 인스턴스 '{tamagotchiInstance?.Name}', 프리팹 '{tamagotchiPrefab?.name}' 설정됨.");
    }

    public void ClearSelectedTamagotchi()
    {
        SelectedTamagotchiInstance = null;
        SelectedTamagotchiPrefab = null;
        Debug.Log($"[{name}] 선택 정보가 초기화되었습니다.");
    }

    public void AddCapturedTamagotchi(GameObject capturedPrefab)
    {
        if (capturedPrefab != null)
        {
            if (!CapturedTamagotchiPrefabs.Contains(capturedPrefab))
            {
                CapturedTamagotchiPrefabs.Add(capturedPrefab);
                Debug.Log($"[{name}] 다마고치 '{capturedPrefab.name}'이(가) 포획 목록에 추가되었습니다. 총 {CapturedTamagotchiPrefabs.Count}개.");
            }
            else
            {
                Debug.LogWarning($"[{name}] 다마고치 '{capturedPrefab.name}'은(는) 이미 포획 목록에 있습니다.");
            }
        }
        else
        {
            Debug.LogWarning($"[{name}] AddCapturedTamagotchi: 추가하려는 프리팹이 null입니다.");
        }
    }

    public bool IsTamagotchiCaptured(GameObject prefab)
    {
        return CapturedTamagotchiPrefabs.Contains(prefab);
    }

    public void ClearAllCapturedTamagotchi()
    {
        CapturedTamagotchiPrefabs.Clear();
        Debug.Log($"[{name}] 모든 포획 다마고치 목록이 초기화되었습니다.");
    }
}