using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // 어디서든 접근 가능하도록 static 인스턴스

    // 다음 씬으로 넘길 데이터를 여기에 저장합니다.
    // 예: 클릭한 다마고치의 종류 (이름, ID 등), 능력치 등
    public string selectedTamagotchiName;
    public int selectedTamagotchiID;
    public Color selectedTamagotchiColor;
    // 필요에 따라 더 많은 데이터를 추가하세요. (예: public TamagotchiData selectedTamagotchiData;)

    private void Awake()
    {
        // 싱글톤 패턴 구현:
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 이 오브젝트는 씬이 바뀌어도 파괴되지 않음
        }
        else
        {
            // 이미 인스턴스가 존재하면 새로 생성된 오브젝트는 파괴 (중복 방지)
            Destroy(gameObject);
        }

        // 씬 로드될 때마다 데이터 초기화 (선택 사항, 필요에 따라)
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // 씬 로드 이벤트 구독 해제 (메모리 누수 방지)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드될 때마다 호출되는 함수 (선택 사항)
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"씬 '{scene.name}'이 로드되었습니다.");
        // 만약 다음 씬에서 특정 데이터가 필요 없다면 여기서 초기화할 수 있습니다.
        // 예: selectedTamagotchiName = null;
    }

    // 클릭된 다마고치 정보 설정 함수
    public void SetSelectedTamagotchi(string tamagotchiName, int tamagotchiID, Color tamagotchiColor)
    {
        selectedTamagotchiName = tamagotchiName;
        selectedTamagotchiID = tamagotchiID;
        selectedTamagotchiColor = tamagotchiColor;
        Debug.Log($"GameManager에 선택된 다마고치 정보 저장됨: {selectedTamagotchiName}");
    }
}
