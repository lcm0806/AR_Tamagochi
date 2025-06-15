using UnityEngine;
using TMPro; // UI Text (Text Mesh Pro)를 사용할 경우

public class TamagotchiDisplayManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI tamagotchiNameText;
    public TextMeshProUGUI tamagotchiStatusText;
    public Transform tamagotchiSpawnPoint; // 다마고치 모델이 생성될 위치 (빈 Gameobject)

    private IDamagotchi currentTamagotchiData; // 데이터만 저장
    private GameObject currentTamagotchiModel; // 실제 생성된 모델 인스턴스

    private float statusUpdateTimer = 0f;
    public float statusUpdateInterval = 1f;

    void Start()
    {
        
        if (GameManager.Instance != null && GameManager.Instance.SelectedTamagotchiInstance != null && GameManager.Instance.SelectedTamagotchiPrefab != null)
        {
            currentTamagotchiData = GameManager.Instance.SelectedTamagotchiInstance;
            GameObject selectedPrefab = GameManager.Instance.SelectedTamagotchiPrefab;

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
        if (currentTamagotchiData == null) return;

        statusUpdateTimer += Time.deltaTime;
        if (statusUpdateTimer >= statusUpdateInterval)
        {
            currentTamagotchiData.UpdateStatus();
            UpdateUI();
            statusUpdateTimer = 0f;

            if (currentTamagotchiData.IsDead())
            {
                Debug.Log($"{currentTamagotchiData.Name}이(가) 죽었습니다.");
                // 게임 오버 처리
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