using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        if (prefabsToSpawn == null || prefabsToSpawn.Count == 0)
        {
            Debug.LogError("소환할 프리팹이 List에 할당되지 않았습니다. Prefabs To Spawn 리스트를 채워주세요.");
            enabled = false; // 스크립트 비활성화
            return;
        }

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
        int attempts = 0;
        Vector3 potentialSpawnPosition = Vector3.zero;
        bool positionFound = false;

        // maxSpawnAttempts 횟수만큼 반복하며 겹치지 않는 위치를 찾습니다.
        while (attempts < maxSpawnAttempts && !positionFound)
        {
            potentialSpawnPosition = new Vector3(
                Random.Range(-spawnRangeX / 2, spawnRangeX / 2),
                0.5f, // Plain 위에 살짝 띄워서 소환 (Plain의 Y 위치에 따라 조절)
                Random.Range(-spawnRangeZ / 2, spawnRangeZ / 2)
            );

            // 해당 위치에 이미 다른 오브젝트가 있는지 확인 (겹침 검사)
            // Physics.CheckSphere는 주어진 위치에 특정 반경의 구를 그려 충돌하는 콜라이더가 있는지 확인합니다.
            // spawnableLayer에 설정된 레이어에 있는 오브젝트만 검사합니다.
            if (!Physics.CheckSphere(potentialSpawnPosition, overlapCheckRadius, spawnableLayer))
            {
                positionFound = true; // 겹치지 않는 유효한 위치를 찾음
            }
            attempts++;
        }

        if (positionFound)
        {
            // 회전 값은 Y축 145도로 고정
            Quaternion rotationToApply = Quaternion.Euler(0, 145, 0);

            int randomIndex = Random.Range(0, prefabsToSpawn.Count);
            GameObject selectedPrefab = prefabsToSpawn[randomIndex];

            // 프리팹을 유효한 위치와 회전으로 소환
            GameObject spawnedInstance = Instantiate(selectedPrefab, potentialSpawnPosition, rotationToApply);
            // --- 여기가 핵심! ---
            // 소환된 인스턴스의 ClickableObject 컴포넌트를 찾아서 원본 프리팹을 설정
            ClickableObject clickObjectScript = spawnedInstance.GetComponent<ClickableObject>();
            if (clickObjectScript != null)
            {
                clickObjectScript.SetOriginalPrefab(selectedPrefab); // <--- 'selectedPrefab' (원본)을 전달!
            }
            else
            {
                Debug.LogWarning($"소환된 오브젝트 '{spawnedInstance.name}'에 ClickableObject 스크립트가 없습니다.");
            }
            // ---------------------
            currentSpawnCount++;
            Debug.Log("Gameobject 소환됨. 현재 소환 개수: " + currentSpawnCount);
            return true; // 소환 성공
        }
        else
        {
            // 유효한 위치를 찾지 못함
            Debug.LogWarning($"최대 시도 횟수({maxSpawnAttempts}) 내에 유효한 소환 위치를 찾지 못했습니다. 주변에 공간이 부족할 수 있습니다.");
            return false; // 소환 실패
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
