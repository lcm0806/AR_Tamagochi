// ClickableObject.cs (추천 방식)
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickableObject : MonoBehaviour
{
    public string targetSceneName = "TestScene";
    private GameObject myOriginalPrefab; // <--- 이 변수를 추가하고 Inspector에서 원본 프리팹 할당!

    // SpawnManager가 호출하여 원본 프리팹을 설정하는 메서드
    public void SetOriginalPrefab(GameObject prefab) // <--- 새로운 public 메서드 추가
    {
        myOriginalPrefab = prefab;
        Debug.Log($"ClickableObject '{gameObject.name}'에 원본 프리팹 '{prefab.name}'이 설정되었습니다.");
    }

    void OnMouseDown()
    {
        Debug.Log($"'{gameObject.name}' 오브젝트가 클릭(터치)되었습니다.");

        // --- 여기서 myOriginalPrefab이 실제 null인지 확인 ---
        if (myOriginalPrefab == null)
        {
            Debug.LogError($"오류: ClickableObject '{gameObject.name}'의 myOriginalPrefab이 할당되어 있지 않습니다! Inspector를 다시 확인하세요.");
            // 이 오류가 뜨면 Inspector 할당 문제
            return; // 더 이상 진행하지 않음
        }
        else
        {
            Debug.Log($"ClickableObject '{gameObject.name}'의 myOriginalPrefab은 '{myOriginalPrefab.name}'입니다. (null 아님)");
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
                    Debug.Log($"다마고치 '{selectedTama.Name}' 정보와 프리팹 '{myOriginalPrefab.name}'을 GameManager에 저장했습니다.");
                }
                else
                {
                    Debug.LogError("ClickableObject의 'My Original Prefab' 필드에 프리팹이 할당되지 않았습니다!");
                }
            }
            else
            {
                Debug.LogError("GameManager 인스턴스를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("클릭된 오브젝트에 IDamagotchi를 구현하는 스크립트가 없습니다.");
        }

        SceneManager.LoadScene(targetSceneName);
    }
}