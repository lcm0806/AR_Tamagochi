using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // UI Text (Text Mesh Pro)를 사용할 경우

public class TamagotchiDisplayManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI tamagotchiNameText;
    public TextMeshProUGUI tamagotchiStatusText;
    public Transform tamagotchiSpawnPoint; // 다마고치 모델이 생성될 위치 (빈 Gameobject)

    private IDamagotchi currentTamagotchiData; // 데이터만 저장
    private GameObject currentTamagotchiModel; // 실제 생성된 모델 인스턴스
    private GameObject originalPrefabReference;

    private float statusUpdateTimer = 0f;
    public float statusUpdateInterval = 1f;

    private bool isCaptured = false; //포획상태 추적 변수 추가

    void Start()
    {
        
        if (GameManager.Instance != null && GameManager.Instance.SelectedTamagotchiInstance != null && GameManager.Instance.SelectedTamagotchiPrefab != null)
        {
            currentTamagotchiData = GameManager.Instance.SelectedTamagotchiInstance;
            GameObject selectedPrefab = GameManager.Instance.SelectedTamagotchiPrefab;
            originalPrefabReference = GameManager.Instance.SelectedTamagotchiPrefab;

            Debug.Log($"다음 씬에서 받아온 다마고치: {currentTamagotchiData.Name} (프리팹: {selectedPrefab.name})");

            // 선택된 프리팹을 새롭게 인스턴스화 (씬에 띄움)
            if (tamagotchiSpawnPoint != null)
            {
                currentTamagotchiModel = Instantiate(selectedPrefab, tamagotchiSpawnPoint.position, tamagotchiSpawnPoint.rotation);
                // 생성된 모델의 부모를 스폰 포인트로 설정하여 깔끔하게 관리 (선택 사항)
                currentTamagotchiModel.transform.SetParent(tamagotchiSpawnPoint);
                currentTamagotchiModel.transform.localPosition = Vector3.zero; // 부모 기준 0,0,0

                // (선택 사항) 생성된 모델의 Collider 비활성화 (클릭으로 또 다른 씬 이동 방지 등)
                Collider modelCollider = currentTamagotchiModel.GetComponent<Collider>();
                if (modelCollider != null) modelCollider.enabled = false;
                // (선택 사항) 생성된 모델의 ClickableObject 스크립트 비활성화
                ClickableObject clickScript = currentTamagotchiModel.GetComponent<ClickableObject>();
                if (clickScript != null) clickScript.enabled = false;

                // 생성된 모델의 렌더러를 통해 색상 변경 등 추가 설정 가능
                // MeshRenderer modelRenderer = currentTamagotchiModel.GetComponent<MeshRenderer>();
                // if (modelRenderer != null) modelRenderer.material.color = currentTamagotchiData.SpeciesColor;

            }
            else
            {
                Debug.LogError("Tamagotchi Spawn Point가 설정되지 않았습니다!");
            }

            // UI 업데이트
            UpdateUI();

            // GameManager의 데이터는 한번 사용했으면 초기화하는 것이 좋습니다 (선택 사항)
            GameManager.Instance.ClearSelectedTamagotchi();
        }
        else
        {
            Debug.LogError("선택된 다마고치 정보 또는 프리팹을 찾을 수 없습니다. 기본 다마고치로 시작합니다.");
            // 오류 처리: 기본 다마고치 인스턴스 생성 또는 이전 씬으로 돌아가기
        }
    }

    void Update()
    {
        if (currentTamagotchiData == null || isCaptured) return;

        statusUpdateTimer += Time.deltaTime;
        if (statusUpdateTimer >= statusUpdateInterval)
        {
            currentTamagotchiData.UpdateStatus();
            UpdateUI();
            statusUpdateTimer = 0f;

            // --- 포획 조건 확인 ---
            if (currentTamagotchiData.IsHappyMaxed) // <--- 행복도 100%를 감지
            {
                Debug.Log($"{currentTamagotchiData.Name}의 행복도가 100%가 되었습니다! 포획 성공!");
                isCaptured = true; // 포획 상태로 변경

                // 2. 행복도 Max 여부를 확인합니다.
                if (currentTamagotchiData.IsHappyMaxed) // <-- 여기서 IsHappyMaxed를 호출합니다.
                {
                    Debug.Log($"{currentTamagotchiData.Name}의 행복도가 100%가 되었습니다! 포획 성공!");
                    isCaptured = true; // 포획 상태로 전환하여 더 이상 업데이트되지 않도록 합니다.

                    // GameManager에 포획된 다마고치 프리팹을 추가합니다.
                    if (GameManager.Instance != null && originalPrefabReference != null)
                    {
                        GameManager.Instance.AddCapturedTamagotchi(originalPrefabReference);
                    }
                    else
                    {
                        Debug.LogError("GameManager 인스턴스 또는 원본 프리팹 참조가 null이어서 포획 정보를 추가할 수 없습니다.");
                    }

                    // 3. GpsScene으로 이동합니다.
                    SceneManager.LoadScene("GpsScene"); // <-- 이 줄이 실행되는지 확인!
                }
            }
            else if (currentTamagotchiData.IsDead()) // 사망 조건 확인 (포획보다 우선 순위 낮음)
            {
                Debug.Log($"{currentTamagotchiData.Name}이(가) 죽었습니다.");
                isCaptured = false; // 죽으면 더 이상 업데이트 안함
                SceneManager.LoadScene("GPSScene");
                // 게임 오버 처리 (예: SceneManager.LoadScene("GameOverScene");)
            }
        }
    }

    void UpdateUI()
    {
        if (currentTamagotchiData == null) return;

        if (tamagotchiNameText != null)
        {
            tamagotchiNameText.text = $"이름: {currentTamagotchiData.Name} (ID: {currentTamagotchiData.ID})";
        }
        if (tamagotchiStatusText != null)
        {
            tamagotchiStatusText.text =
                $"체력: {currentTamagotchiData.Health}\n" +
                $"포만감: {currentTamagotchiData.Hunger}\n" +
                $"행복도: {currentTamagotchiData.Happiness}";
        }
    }

    // --- UI 버튼에 연결할 함수들 ---
    public void OnFeedButtonClicked()
    {
        if (currentTamagotchiData != null)
        {
            currentTamagotchiData.Feed(10);
            UpdateUI();
        }
    }

    public void OnPlayButtonClicked()
    {
        if (currentTamagotchiData != null)
        {
            currentTamagotchiData.Play(5);
            UpdateUI();
        }
    }

    public void OnSleepButtonClicked()
    {
        if (currentTamagotchiData != null)
        {
            currentTamagotchiData.Sleep(10);
            UpdateUI();
        }
    }
}