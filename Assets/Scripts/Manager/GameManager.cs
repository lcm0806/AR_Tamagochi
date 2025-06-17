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

    // 새로 추가: 현재 AR 씬에 배치된 다마고치 인스턴스들을 저장하는 리스트
    private List<GameObject> _placedTamagotchiInstances = new List<GameObject>();

    private void Awake()
    {

        if (Instance == null)
        {
            // 첫 번째 GameManager 인스턴스가 생성될 때
            Instance = this;
            DontDestroyOnLoad(gameObject); // 이 오브젝트는 씬 전환 시 파괴되지 않음
        }
        else if (Instance != this) // 이미 다른 인스턴스가 존재하면 (이게 문제입니다!)
        {
            Destroy(gameObject);
            return; // 메서드 종료하여 더 이상 코드 실행 방지
        }

        // 씬 로드 이벤트 구독 (중복 구독 방지를 위해 해제 후 다시 구독)
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        // CapturedTamagotchiPrefabs 리스트가 null인 경우는 없겠지만, 안전을 위해 초기화 로직
        if (CapturedTamagotchiPrefabs == null)
        {
            CapturedTamagotchiPrefabs = new List<GameObject>();
        }
    }

    private void OnDestroy()
    {
        // 이 오브젝트가 현재 싱글톤 Instance라면 이벤트 구독 해제
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[{name}] OnSceneLoaded: 씬 '{scene.name}' 로드됨.");
        Debug.Log($"[{name}] OnSceneLoaded: 현재 포획된 다마고치 프리팹 수: {CapturedTamagotchiPrefabs.Count}");
        Debug.Log($"[{name}] OnSceneLoaded: 현재 배치된 다마고치 인스턴스 수: {_placedTamagotchiInstances.Count}");

        // 모든 배치된 다마고치 인스턴스의 활성화 상태를 업데이트
        foreach (GameObject tamagotchiInstance in _placedTamagotchiInstances)
        {
            if (tamagotchiInstance != null)
            {
                // 만약 현재 씬이 AR 씬이라면 활성화하고, 그렇지 않다면 비활성화
                bool isActiveInARScene = (scene.name == "ARScene"); // AR 씬 이름 확인
                tamagotchiInstance.SetActive(isActiveInARScene);

                if (isActiveInARScene)
                {
                    // AR 씬으로 돌아왔을 때, 다마고치가 제대로 보이도록 필요한 추가 설정 (선택 사항)
                    // 만약 AR 세션 재개 시 AR 공간의 원점이 바뀌어 위치가 틀어진다면,
                    // 여기서 이전에 저장했던 상대 위치 값을 사용해 다시 조정하는 로직을 추가할 수 있습니다.
                    // 하지만 DontDestroyOnLoad된 오브젝트는 월드 위치를 유지하므로,
                    // 대부분의 경우 기본적으로 잘 작동할 것입니다.
                    Debug.Log($"[{name}] '{tamagotchiInstance.name}' 활성화됨 (씬: {scene.name})");
                }
                else
                {
                    Debug.Log($"[{name}] '{tamagotchiInstance.name}' 비활성화됨 (씬: {scene.name})");
                }
            }
        }

        // GPSScene으로 돌아왔을 때, 포획된 다마고치 프리팹 목록을 상세히 로그
        if (scene.name == "GPSScene")
        {
            Debug.Log($"[{name}] GPSScene 로드 완료. 포획된 다마고치 프리팹 목록:");
            if (CapturedTamagotchiPrefabs.Count > 0)
            {
                foreach (var prefab in CapturedTamagotchiPrefabs)
                {
                    Debug.Log($"[{name}] - {prefab.name}");
                }
            }
            else
            {
                Debug.Log($"[{name}] - 포획된 다마고치 프리팹이 없습니다.");
            }
        }
    }

    // --- 기타 함수들 (변경 없음) ---
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

    // 새로 추가: AR 씬에 배치된 실제 다마고치 인스턴스를 리스트에 추가
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

    // 배치된 모든 다마고치 인스턴스를 제거하고 리스트를 비우는 함수 (예: 게임 재시작 시)
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