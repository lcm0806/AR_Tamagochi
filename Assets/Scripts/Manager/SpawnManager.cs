using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        if (prefabsToSpawn == null || prefabsToSpawn.Count == 0)
        {
            Debug.LogError("��ȯ�� �������� List�� �Ҵ���� �ʾҽ��ϴ�. Prefabs To Spawn ����Ʈ�� ä���ּ���.");
            enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
            return;
        }

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
        int attempts = 0;
        Vector3 potentialSpawnPosition = Vector3.zero;
        bool positionFound = false;

        // maxSpawnAttempts Ƚ����ŭ �ݺ��ϸ� ��ġ�� �ʴ� ��ġ�� ã���ϴ�.
        while (attempts < maxSpawnAttempts && !positionFound)
        {
            potentialSpawnPosition = new Vector3(
                Random.Range(-spawnRangeX / 2, spawnRangeX / 2),
                0.5f, // Plain ���� ��¦ ����� ��ȯ (Plain�� Y ��ġ�� ���� ����)
                Random.Range(-spawnRangeZ / 2, spawnRangeZ / 2)
            );

            // �ش� ��ġ�� �̹� �ٸ� ������Ʈ�� �ִ��� Ȯ�� (��ħ �˻�)
            // Physics.CheckSphere�� �־��� ��ġ�� Ư�� �ݰ��� ���� �׷� �浹�ϴ� �ݶ��̴��� �ִ��� Ȯ���մϴ�.
            // spawnableLayer�� ������ ���̾ �ִ� ������Ʈ�� �˻��մϴ�.
            if (!Physics.CheckSphere(potentialSpawnPosition, overlapCheckRadius, spawnableLayer))
            {
                positionFound = true; // ��ġ�� �ʴ� ��ȿ�� ��ġ�� ã��
            }
            attempts++;
        }

        if (positionFound)
        {
            // ȸ�� ���� Y�� 145���� ����
            Quaternion rotationToApply = Quaternion.Euler(0, 145, 0);

            int randomIndex = Random.Range(0, prefabsToSpawn.Count);
            GameObject selectedPrefab = prefabsToSpawn[randomIndex];

            // �������� ��ȿ�� ��ġ�� ȸ������ ��ȯ
            GameObject spawnedInstance = Instantiate(selectedPrefab, potentialSpawnPosition, rotationToApply);
            // --- ���Ⱑ �ٽ�! ---
            // ��ȯ�� �ν��Ͻ��� ClickableObject ������Ʈ�� ã�Ƽ� ���� �������� ����
            ClickableObject clickObjectScript = spawnedInstance.GetComponent<ClickableObject>();
            if (clickObjectScript != null)
            {
                clickObjectScript.SetOriginalPrefab(selectedPrefab); // <--- 'selectedPrefab' (����)�� ����!
            }
            else
            {
                Debug.LogWarning($"��ȯ�� ������Ʈ '{spawnedInstance.name}'�� ClickableObject ��ũ��Ʈ�� �����ϴ�.");
            }
            // ---------------------
            currentSpawnCount++;
            Debug.Log("Gameobject ��ȯ��. ���� ��ȯ ����: " + currentSpawnCount);
            return true; // ��ȯ ����
        }
        else
        {
            // ��ȿ�� ��ġ�� ã�� ����
            Debug.LogWarning($"�ִ� �õ� Ƚ��({maxSpawnAttempts}) ���� ��ȿ�� ��ȯ ��ġ�� ã�� ���߽��ϴ�. �ֺ��� ������ ������ �� �ֽ��ϴ�.");
            return false; // ��ȯ ����
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
