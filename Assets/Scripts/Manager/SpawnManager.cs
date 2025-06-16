using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnManager : MonoBehaviour
{
    // ��ȯ�� �����յ��� Inspector���� �Ҵ��� ����Ʈ
    public List<GameObject> prefabsToSpawn;

    public int maxSpawnCount = 5;    // �ִ� ��ȯ ����
    private int currentSpawnCount = 0; // ���� ��ȯ�� ����
    public float spawnInterval = 2f; // ��ȯ ���� (��)

    // ��ȯ�� ��ġ�� Plain ���� �����ϰ� �����ϱ� ���� ����
    // Plain�� ũ�⿡ ���� �����ؾ� �մϴ�.
    public float spawnRangeX = 10f;
    public float spawnRangeZ = 10f;


    // ��ħ �ؼ� ���� ���� �߰�
    [Header("Overlap Resolution")] // Inspector���� ���� ���� ��� �߰�
    public LayerMask spawnableLayer; // ��ȯ�� ������Ʈ ���̾� (Inspector���� ����)
    public float overlapCheckRadius = 5f; // ��ħ �˻� �ݰ� (��ȯ�� ������Ʈ ũ�⿡ ���� ����)
    public int maxSpawnAttempts = 10; // ��ȿ�� ��ġ�� ã�� ���� �ִ� �õ� Ƚ��

    // --- ���� ���� ���� ���� ---
    [Header("Spawn Dynamic Logic")]
    public int initialRangeEndIndex = 2; // Cat, Dog, Dove�� ������ �ε��� (����)
    private int _currentExpansionLevel = 0; // ���� Ȯ�� ���� (���� ���� ���� ���� ����)
    // ----------------------------

    void Start()
    {
        if (prefabsToSpawn == null || prefabsToSpawn.Count == 0)
        {
            Debug.LogError("��ȯ�� �������� List�� �Ҵ���� �ʾҽ��ϴ�. Prefabs To Spawn ����Ʈ�� ä���ּ���.");
            enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
            return;
        }

        _currentExpansionLevel = 0; // Ȯ�� ���� �ʱ�ȭ
        // spawnableLayer�� �������� �ʾҴٸ� ��� �޽��� ���
        if (spawnableLayer == 0) // LayerMask�� �⺻���� 0 (Nothing)
        {
            Debug.LogWarning("Spawnable Layer�� �������� �ʾҽ��ϴ�! Inspector���� 'SpawnManager' ������Ʈ�� 'Spawnable Layer'�� �������ּ���. (��: ���� ���� 'SpawnedObject' ���̾�)");
        }

        StartCoroutine(SpawnCoroutine());
    }

    IEnumerator SpawnCoroutine()
    {
        while (currentSpawnCount < maxSpawnCount)
        {
            // ��ȿ�� ��ȯ ��ġ�� ã�� ������ �õ�
            bool spawnedSuccessfully = TrySpawnRandomPrefab();
            if (spawnedSuccessfully)
            {
                yield return new WaitForSeconds(spawnInterval); // ���� �� ������ ���ݸ�ŭ ���
            }
            else
            {
                // ��ȿ�� ��ġ�� ã�� ���߾ ���� ��ȯ�� �õ��� �� �ֵ��� ��� ���
                Debug.LogWarning("�ִ� �õ� Ƚ�� ���� ��ȿ�� ��ȯ ��ġ�� ã�� ���߽��ϴ�. ���� ��ȯ �õ� ��� ��.");
                yield return new WaitForSeconds(spawnInterval / 2f); // ���� �� ���� �� ������ ��õ� ����
            }
        }
        Debug.Log("�ִ� ��ȯ ������ �����߽��ϴ�.");
    }

    bool TrySpawnRandomPrefab()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager �ν��Ͻ��� ã�� �� �����ϴ�. SpawnManager�� �۵��� �� �����ϴ�.");
            return false;
        }

        // --- ���� Ȱ�� ���� ������ ��ȹ�� ������ �� ��� �� Ȯ�� ���� ������Ʈ ---
        int capturedCountInInitialRange = 0;
        for (int i = 0; i <= initialRangeEndIndex && i < prefabsToSpawn.Count; i++)
        {
            GameObject currentPrefab = prefabsToSpawn[i];
            if (currentPrefab != null && GameManager.Instance.IsTamagotchiCaptured(currentPrefab))
            {
                capturedCountInInitialRange++;
            }
        }

        _currentExpansionLevel = capturedCountInInitialRange; // ���� ������ ��ŭ Ȯ�� ���� ����

        // --- ���� �ĺ� ����Ʈ ���� ---
        List<GameObject> currentSpawnCandidates = new List<GameObject>();

        // ���� ����� ���� �� �ε��� ��� (�ʱ� ���� + Ȯ�� ����)
        int effectiveEndIndex = initialRangeEndIndex + _currentExpansionLevel;
        // prefabsToSpawn ����Ʈ�� ���� ũ�⸦ ���� �ʵ��� ����
        effectiveEndIndex = Mathf.Min(effectiveEndIndex, prefabsToSpawn.Count - 1);


        Debug.Log($"--- ���� ������ ������ �ĺ� �˻� ���� (�ε��� 0 ~ {effectiveEndIndex}) ---"); // ������ �׻� 0
        for (int i = 0; i <= effectiveEndIndex && i < prefabsToSpawn.Count; i++)
        {
            GameObject currentPrefab = prefabsToSpawn[i];
            if (currentPrefab != null)
            {
                bool isCaptured = GameManager.Instance.IsTamagotchiCaptured(currentPrefab);
                Debug.Log($"�ε��� {i}: ������ '{currentPrefab.name}', ��ȹ ����: {isCaptured}");

                if (!isCaptured) // ��ȹ���� ���� �����ո� �ĺ��� �߰�
                {
                    currentSpawnCandidates.Add(currentPrefab);
                }
            }
            else
            {
                Debug.LogWarning($"�ε��� {i}: Prefab�� null�Դϴ�.");
            }
        }
        Debug.Log($"--- ���� ������ ������ �ĺ� �˻� ����. ���� ���� �ĺ� ��: {currentSpawnCandidates.Count} ---");

        // --- ����� �α� Ȱ��ȭ �� ���� ---
        if (currentSpawnCandidates.Count > 0)
        {
            string logMessage = "���� ���� ���� ������ �ٸ���ġ ������: ";
            foreach (GameObject prefab in currentSpawnCandidates)
            {
                logMessage += prefab.name + ", ";
            }
            Debug.Log(logMessage.TrimEnd(',', ' '));
        }
        else
        {
            Debug.Log("���� ���� ������ �ٸ���ġ �������� �����ϴ�.");
        }
        // ----------------------------------

        // ��ȯ ������ �������� �ϳ��� ���ٸ� ��ȯ �ߴ�
        if (currentSpawnCandidates.Count == 0)
        {
            Debug.Log("��� ��ȿ�� �ٸ���ġ�� ��ȹ�Ǿ��ų� ��ȯ�� �� �����ϴ�. �� �̻� ��ȯ���� �ʽ��ϴ�.");
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
                Debug.LogWarning($"��ȯ�� ������Ʈ '{spawnedInstance.name}'�� ClickableObject ��ũ��Ʈ�� �����ϴ�.");
            }

            currentSpawnCount++;
            Debug.Log($"Gameobject ��ȯ��: {selectedPrefab.name}. ���� ��ȯ ����: {currentSpawnCount}");
            return true;
        }
        else
        {
            Debug.LogWarning($"�ִ� �õ� Ƚ��({maxSpawnAttempts}) ���� ��ȿ�� ��ȯ ��ġ�� ã�� ���߽��ϴ�. �ֺ��� ������ ������ �� �ֽ��ϴ�.");
            return false;
        }
    }

    // �����: �����Ϳ��� ��ħ �˻� �ݰ� �ð�ȭ
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        // spawnRangeX�� spawnRangeZ�� ������� ��ȯ ���� ǥ��
        Gizmos.DrawWireCube(transform.position + new Vector3(0, 0.5f, 0), new Vector3(spawnRangeX, 0.1f, spawnRangeZ));

        // ���� �÷��� ���� �ƴϾ overlapCheckRadius�� �ð�ȭ (������ ��)
        // ���� ���, ���������� �õ��ߴ� ��ġ�� ���� �׸� ���� ������,
        // �ܼ��� Inspector���� overlapCheckRadius ���� ������ �� ��������� ����ϴ� ���� �� �Ϲ����Դϴ�.
        // Gizmos.DrawWireSphere(Vector3.zero, overlapCheckRadius); // 0,0,0 ��ġ�� �׸��� �򰥸� �� ����
    }
}
