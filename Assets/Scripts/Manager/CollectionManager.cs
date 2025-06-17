using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Button, ScrollRect 등을 위해 필요
using TMPro; // TextMeshProUGUI를 위해 필요
using UnityEngine.SceneManagement; // 씬 전환을 위해 필요

public class CollectionManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject tamagotchiButtonPrefab; // 각 다마고치를 표시할 버튼 프리팹 (아래 설명 참조)
    public Transform contentParent; // Scroll View의 Content 오브젝트 (버튼들이 들어갈 부모)
    public TextMeshProUGUI detailInfoText; // 상세 정보를 표시할 TextMeshProUGUI
    public GameObject detailPanel; // 상세 정보 패널 (초기 비활성화)

    [Header("Scene Management")]
    public string gpsSceneName = "GPSScene"; // 돌아갈 GPS 씬 이름

    private List<GameObject> createdButtons = new List<GameObject>(); // 생성된 버튼들을 관리하는 리스트

    void Start()
    {
        // UI 요소들이 할당되었는지 확인
        if (tamagotchiButtonPrefab == null || contentParent == null || detailInfoText == null || detailPanel == null)
        {
            Debug.LogError("CollectionManager UI References가 모두 할당되지 않았습니다. Inspector를 확인해주세요.");
            enabled = false; // 스크립트 비활성화
            return;
        }

        detailPanel.SetActive(false); // 상세 정보 패널은 시작 시 비활성화
        DisplayCapturedTamagotchis(); // 포획된 다마고치 목록 표시
    }

    void DisplayCapturedTamagotchis()
    {
        // 기존에 생성된 버튼이 있다면 모두 제거
        foreach (GameObject buttonGO in createdButtons)
        {
            Destroy(buttonGO);
        }
        createdButtons.Clear();

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance를 찾을 수 없습니다.");
            return;
        }

        List<GameObject> capturedPrefabs = GameManager.Instance.CapturedTamagotchiPrefabs;

        if (capturedPrefabs.Count == 0)
        {
            detailInfoText.text = "아직 포획한 다마고치가 없습니다.";
            Debug.Log("포획된 다마고치가 없습니다.");
            return;
        }

        Debug.Log($"포획된 다마고치 수: {capturedPrefabs.Count}");

        foreach (GameObject prefab in capturedPrefabs)
        {
            if (prefab != null)
            {
                // 버튼 생성
                IDamagotchi tamagotchiData = prefab.GetComponent<IDamagotchi>();

                if (tamagotchiData == null)
                {
                    Debug.LogWarning($"프리팹 '{prefab.name}'에 IDamagotchi 컴포넌트를 찾을 수 없습니다. 버튼 이름을 프리팹 이름으로 설정합니다.");
                    // IDamagotchi가 없으면 prefab.name을 기본값으로 사용
                    // 또는 이 경우 버튼을 생성하지 않을 수도 있습니다. (선택)
                    // continue; // 이 다마고치는 건너뛰기
                }


                GameObject buttonGO = Instantiate(tamagotchiButtonPrefab, contentParent);
                createdButtons.Add(buttonGO); // 생성된 버튼 리스트에 추가

                // 버튼의 TextMeshProUGUI 컴포넌트 찾기 (버튼 프리팹 내부에 TextMeshProUGUI가 있다고 가정)
                TextMeshProUGUI buttonText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    if (tamagotchiData != null)
                    {
                        buttonText.text = tamagotchiData.Name; // IDamagotchi에서 설정된 이름 사용
                    }
                    else
                    {
                        buttonText.text = prefab.name; // IDamagotchi가 없으면 프리팹 이름으로 폴백
                    }
                }
                else
                {
                    Debug.LogWarning($"버튼 프리팹 '{tamagotchiButtonPrefab.name}'에 TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.");
                }

                // 버튼 클릭 이벤트 추가
                Button button = buttonGO.GetComponent<Button>();
                if (button != null)
                {
                    // 클로저 문제를 방지하기 위해 로컬 변수로 캡처
                    GameObject currentPrefab = prefab;
                    button.onClick.AddListener(() => OnTamagotchiSelected(currentPrefab));
                }
                else
                {
                    Debug.LogWarning($"버튼 프리팹 '{tamagotchiButtonPrefab.name}'에 Button 컴포넌트를 찾을 수 없습니다.");
                }
            }
        }
    }

    // 다마고치 버튼 클릭 시 호출될 함수
    void OnTamagotchiSelected(GameObject selectedPrefab)
    {
        Debug.Log($"다마고치 선택됨: {selectedPrefab.name}");
        detailPanel.SetActive(true); // 상세 정보 패널 활성화

        // 선택된 다마고치 프리팹의 IDamagotchi 스크립트 컴포넌트를 가져옴
        // 주의: 이 시점에는 실제 인스턴스가 아니므로, 프리팹 자체에 붙어있는 스크립트 정보를 읽어야 합니다.
        IDamagotchi tamagotchiData = selectedPrefab.GetComponent<IDamagotchi>();

        if (tamagotchiData != null)
        {
            // 정보를 UI 텍스트에 표시
            detailInfoText.text =
                $"이름: {tamagotchiData.Name}\n" +
                $"ID: {tamagotchiData.ID}\n" +
                $"체력: {tamagotchiData.Health}\n" +
                $"배고픔: {tamagotchiData.Hunger}\n" +
                $"행복도: {tamagotchiData.Happiness}\n";
        }
        else
        {
            detailInfoText.text = $"선택된 다마고치 ({selectedPrefab.name})에 IDamagotchi 스크립트가 없습니다.";
            Debug.LogWarning($"프리팹 '{selectedPrefab.name}'에 IDamagotchi 컴포넌트가 없습니다.");
        }
    }

    // 상세 정보 패널의 "닫기" 또는 "확인" 버튼에 연결할 함수
    public void OnDetailPanelClose()
    {
        detailPanel.SetActive(false);
    }

    // 뒤로 가기 버튼에 연결할 함수
    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene(gpsSceneName);
    }
}