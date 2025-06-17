// ClickableObject.cs (��õ ���)
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickableObject : MonoBehaviour
{
    public string targetSceneName = "TestScene";
    private GameObject myOriginalPrefab; // <--- �� ������ �߰��ϰ� Inspector���� ���� ������ �Ҵ�!

    // SpawnManager�� ȣ���Ͽ� ���� �������� �����ϴ� �޼���
    public void SetOriginalPrefab(GameObject prefab) // <--- ���ο� public �޼��� �߰�
    {
        myOriginalPrefab = prefab;
        Debug.Log($"ClickableObject '{gameObject.name}'�� ���� ������ '{prefab.name}'�� �����Ǿ����ϴ�.");
    }

    void OnMouseDown()
    {
        Debug.Log($"'{gameObject.name}' ������Ʈ�� Ŭ��(��ġ)�Ǿ����ϴ�.");

        // --- ���⼭ myOriginalPrefab�� ���� null���� Ȯ�� ---
        if (myOriginalPrefab == null)
        {
            Debug.LogError($"����: ClickableObject '{gameObject.name}'�� myOriginalPrefab�� �Ҵ�Ǿ� ���� �ʽ��ϴ�! Inspector�� �ٽ� Ȯ���ϼ���.");
            // �� ������ �߸� Inspector �Ҵ� ����
            return; // �� �̻� �������� ����
        }
        else
        {
            Debug.Log($"ClickableObject '{gameObject.name}'�� myOriginalPrefab�� '{myOriginalPrefab.name}'�Դϴ�. (null �ƴ�)");
        }
        // -----------------------------------------------------

        IDamagotchi selectedTama = GetComponent<IDamagotchi>();
        if (selectedTama != null)
        {
            if (GameManager.Instance != null)
            {
                if (myOriginalPrefab != null)
                {
                    GameManager.Instance.SetSelectedTamagotchi(selectedTama, myOriginalPrefab);
                    Debug.Log($"�ٸ���ġ '{selectedTama.Name}' ������ ������ '{myOriginalPrefab.name}'�� GameManager�� �����߽��ϴ�.");
                }
                else
                {
                    Debug.LogError("ClickableObject�� 'My Original Prefab' �ʵ忡 �������� �Ҵ���� �ʾҽ��ϴ�!");
                }
            }
            else
            {
                Debug.LogError("GameManager �ν��Ͻ��� ã�� �� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("Ŭ���� ������Ʈ�� IDamagotchi�� �����ϴ� ��ũ��Ʈ�� �����ϴ�.");
        }

        SceneManager.LoadScene(targetSceneName);
    }
}