using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnManager : MonoBehaviour
{
    // 소환할 프리팹들을 Inspector에서 할당할 리스트
    public List<GameObject> prefabsToSpawn;

    public int maxSpawnCount = 5;    // 최대 소환 개수
    private int currentSpawnCount = 0; // 현재 소환된 개수
    public float spawnInterval = 2f; // 소환 간격 (초)

    // 소환할 위치를 Plain 위로 랜덤하게 지정하기 위한 변수
    // Plain의 크기에 따라 조절해야 합니다.
    public float spawnRangeX = 10f;
    public float spawnRangeZ = 10f;


    // 겹침 해소 관련 변수 추가
    [Header("Overlap Resolution")] // Inspector에서 보기 좋게 헤더 추가
    public LayerMask spawnableLayer; // 소환된 오브젝트 레이어 (Inspector에서 설정)
    public float overlapCheckRadius = 5f; // 겹침 검사 반경 (소환할 오브젝트 크기에 따라 조절)
    public int maxSpawnAttempts = 10; // 유효한 위치를 찾기 위한 최대 시도 횟수

    // --- 동적 스폰 로직 설정 ---
    [Header("Spawn Dynamic Logic")]
    public int initialRangeEndIndex = 2; // Cat, Dog, Dove의 마지막 인덱스 (포함)
    private int _currentExpansionLevel = 0; // 현재 확장 레벨 (잡힌 마리 수에 따라 결정)
    // ----------------------------

    void Start()
    {
        if (prefabsToSpawn == null || prefabsToSpawn.Count == 0)
        {
            Debug.LogError("소환할 프리팹이 List에 할당되지 않았습니다. Prefabs To Spawn 리스트를 채워주세요.");
            enabled = false; // 스크립트 비활성화
            return;
        }

        _currentExpansionLevel = 0; // 확장 레벨 초기화
        // spawnableLayer가 설정되지 않았다면 경고 메시지 출력
        if (spawnableLayer == 0) // LayerMask의 기본값은 0 (Nothing)
        {
            Debug.LogWarning("Spawnable Layer가 설정되지 않았습니다! Inspector에서 'SpawnManager' 오브젝트의 'Spawnable Layer'를 설정해주세요. (예: 새로 만든 'SpawnedObject' 레이어)");
        }

        StartCoroutine(SpawnCoroutine());
    }

    IEnumerator SpawnCoroutine()
    {
        while (currentSpawnCount < maxSpawnCount)
        {
            // 유효한 소환 위치를 찾을 때까지 시도
            bool spawnedSuccessfully = TrySpawnRandomPrefab();
            if (spawnedSuccessfully)
            {
                yield return new WaitForSeconds(spawnInterval); // 성공 시 지정된 간격만큼 대기
            }
            else
            {
                // 유효한 위치를 찾지 못했어도 다음 소환을 시도할 수 있도록 잠시 대기
                Debug.LogWarning("최대 시도 횟수 내에 유효한 소환 위치를 찾지 못했습니다. 다음 소환 시도 대기 중.");
                yield return new WaitForSeconds(spawnInterval / 2f); // 실패 시 조금 더 빠르게 재시도 가능
            }
        }
        Debug.Log("최대 소환 개수에 도달했습니다.");
    }

    bool TrySpawnRandomPrefab()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager 인스턴스를 찾을 수 없습니다. SpawnManager가 작동할 수 없습니다.");
            return false;
        }

        // --- 현재 활성 범위 내에서 포획된 프리팹 수 계산 및 확장 레벨 업데이트 ---
        int capturedCountInInitialRange = 0;
        for (int i = 0; i <= initialRangeEndIndex && i < prefabsToSpawn.Count; i++)
        {
            GameObject currentPrefab = prefabsToSpawn[i];
            if (currentPrefab != null && GameManager.Instance.IsTamagotchiCaptured(currentPrefab))
            {
                capturedCountInInitialRange++;
            }
        }

        _currentExpansionLevel = capturedCountInInitialRange; // 잡힌 마리수 만큼 확장 레벨 증가

        // --- 스폰 후보 리스트 생성 ---
        List<GameObject> currentSpawnCandidates = new List<GameObject>();

        // 스폰 대상의 실제 끝 인덱스 계산 (초기 범위 + 확장 레벨)
        int effectiveEndIndex = initialRangeEndIndex + _currentExpansionLevel;
        // prefabsToSpawn 리스트의 실제 크기를 넘지 않도록 조정
        effectiveEndIndex = Mathf.Min(effectiveEndIndex, prefabsToSpawn.Count - 1);


        Debug.Log($"--- 스폰 가능한 프리팹 후보 검사 시작 (인덱스 0 ~ {effectiveEndIndex}) ---"); // 시작은 항상 0
        for (int i = 0; i <= effectiveEndIndex && i < prefabsToSpawn.Count; i++)
        {
            GameObject currentPrefab = prefabsToSpawn[i];
            if (currentPrefab != null)
            {
                bool isCaptured = GameManager.Instance.IsTamagotchiCaptured(currentPrefab);
                Debug.Log($"인덱스 {i}: 프리팹 '{currentPrefab.name}', 포획 상태: {isCaptured}");

                if (!isCaptured) // 포획되지 않은 프리팹만 후보에 추가
                {
                    currentSpawnCandidates.Add(currentPrefab);
                }
            }
            else
            {
                Debug.LogWarning($"인덱스 {i}: Prefab이 null입니다.");
            }
        }
        Debug.Log($"--- 스폰 가능한 프리팹 후보 검사 종료. 현재 스폰 후보 수: {currentSpawnCandidates.Count} ---");

        // --- 디버그 로그 활성화 및 개선 ---
        if (currentSpawnCandidates.Count > 0)
        {
            string logMessage = "현재 최종 스폰 가능한 다마고치 프리팹: ";
            foreach (GameObject prefab in currentSpawnCandidates)
            {
                logMessage += prefab.name + ", ";
            }
            Debug.Log(logMessage.TrimEnd(',', ' '));
        }
        else
        {
            Debug.Log("현재 스폰 가능한 다마고치 프리팹이 없습니다.");
        }
        // ----------------------------------

        // 소환 가능한 프리팹이 하나도 없다면 소환 중단
        if (currentSpawnCandidates.Count == 0)
        {
            Debug.Log("모든 유효한 다마고치가 포획되었거나 소환할 수 없습니다. 더 이상 소환하지 않습니다.");
            StopCoroutine(SpawnCoroutine());
            return false;
        }

        int attempts = 0;
        Vector3 potentialSpawnPosition = Vector3.zero;
        bool positionFound = false;

        while (attempts < maxSpawnAttempts && !positionFound)
        {
            potentialSpawnPosition = new Vector3(
                Random.Range(-spawnRangeX / 2, spawnRangeX / 2),
                0.5f,
                Random.Range(-spawnRangeZ / 2, spawnRangeZ / 2)
            );

            if (!Physics.CheckSphere(potentialSpawnPosition, overlapCheckRadius, spawnableLayer))
            {
                positionFound = true;
            }
            attempts++;
        }

        if (positionFound)
        {
            Quaternion rotationToApply = Quaternion.Euler(0, 145, 0);

            int randomIndex = Random.Range(0, currentSpawnCandidates.Count);
            GameObject selectedPrefab = currentSpawnCandidates[randomIndex];

            GameObject spawnedInstance = Instantiate(selectedPrefab, potentialSpawnPosition, rotationToApply);

            ClickableObject clickObjectScript = spawnedInstance.GetComponent<ClickableObject>();
            if (clickObjectScript != null)
            {
                clickObjectScript.SetOriginalPrefab(selectedPrefab);
            }
            else
            {
                Debug.LogWarning($"소환된 오브젝트 '{spawnedInstance.name}'에 ClickableObject 스크립트가 없습니다.");
            }

            currentSpawnCount++;
            Debug.Log($"Gameobject 소환됨: {selectedPrefab.name}. 현재 소환 개수: {currentSpawnCount}");
            return true;
        }
        else
        {
            Debug.LogWarning($"최대 시도 횟수({maxSpawnAttempts}) 내에 유효한 소환 위치를 찾지 못했습니다. 주변에 공간이 부족할 수 있습니다.");
            return false;
        }
    }

    // 디버깅: 에디터에서 겹침 검사 반경 시각화
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        // spawnRangeX와 spawnRangeZ를 기반으로 소환 범위 표시
        Gizmos.DrawWireCube(transform.position + new Vector3(0, 0.5f, 0), new Vector3(spawnRangeX, 0.1f, spawnRangeZ));

        // 게임 플레이 중이 아니어도 overlapCheckRadius를 시각화 (도움이 됨)
        // 예를 들어, 마지막으로 시도했던 위치에 구를 그릴 수도 있지만,
        // 단순히 Inspector에서 overlapCheckRadius 값을 조정할 때 참고용으로 사용하는 것이 더 일반적입니다.
        // Gizmos.DrawWireSphere(Vector3.zero, overlapCheckRadius); // 0,0,0 위치에 그리면 헷갈릴 수 있음
    }
}
