using UnityEngine;
using TMPro; // UI Text (Text Mesh Pro)�� ����� ���

public class TamagotchiDisplayManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI tamagotchiNameText;
    public TextMeshProUGUI tamagotchiStatusText;
    public Transform tamagotchiSpawnPoint; // �ٸ���ġ ���� ������ ��ġ (�� Gameobject)

    private IDamagotchi currentTamagotchiData; // �����͸� ����
    private GameObject currentTamagotchiModel; // ���� ������ �� �ν��Ͻ�

    private float statusUpdateTimer = 0f;
    public float statusUpdateInterval = 1f;

    void Start()
    {
        
        if (GameManager.Instance != null && GameManager.Instance.SelectedTamagotchiInstance != null && GameManager.Instance.SelectedTamagotchiPrefab != null)
        {
            currentTamagotchiData = GameManager.Instance.SelectedTamagotchiInstance;
            GameObject selectedPrefab = GameManager.Instance.SelectedTamagotchiPrefab;

            Debug.Log($"���� ������ �޾ƿ� �ٸ���ġ: {currentTamagotchiData.Name} (������: {selectedPrefab.name})");

            // ���õ� �������� ���Ӱ� �ν��Ͻ�ȭ (���� ���)
            if (tamagotchiSpawnPoint != null)
            {
                currentTamagotchiModel = Instantiate(selectedPrefab, tamagotchiSpawnPoint.position, tamagotchiSpawnPoint.rotation);
                // ������ ���� �θ� ���� ����Ʈ�� �����Ͽ� ����ϰ� ���� (���� ����)
                currentTamagotchiModel.transform.SetParent(tamagotchiSpawnPoint);
                currentTamagotchiModel.transform.localPosition = Vector3.zero; // �θ� ���� 0,0,0

                // (���� ����) ������ ���� Collider ��Ȱ��ȭ (Ŭ������ �� �ٸ� �� �̵� ���� ��)
                Collider modelCollider = currentTamagotchiModel.GetComponent<Collider>();
                if (modelCollider != null) modelCollider.enabled = false;
                // (���� ����) ������ ���� ClickableObject ��ũ��Ʈ ��Ȱ��ȭ
                ClickableObject clickScript = currentTamagotchiModel.GetComponent<ClickableObject>();
                if (clickScript != null) clickScript.enabled = false;

                // ������ ���� �������� ���� ���� ���� �� �߰� ���� ����
                // MeshRenderer modelRenderer = currentTamagotchiModel.GetComponent<MeshRenderer>();
                // if (modelRenderer != null) modelRenderer.material.color = currentTamagotchiData.SpeciesColor;

            }
            else
            {
                Debug.LogError("Tamagotchi Spawn Point�� �������� �ʾҽ��ϴ�!");
            }

            // UI ������Ʈ
            UpdateUI();

            // GameManager�� �����ʹ� �ѹ� ��������� �ʱ�ȭ�ϴ� ���� �����ϴ� (���� ����)
            GameManager.Instance.ClearSelectedTamagotchi();
        }
        else
        {
            Debug.LogError("���õ� �ٸ���ġ ���� �Ǵ� �������� ã�� �� �����ϴ�. �⺻ �ٸ���ġ�� �����մϴ�.");
            // ���� ó��: �⺻ �ٸ���ġ �ν��Ͻ� ���� �Ǵ� ���� ������ ���ư���
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
                Debug.Log($"{currentTamagotchiData.Name}��(��) �׾����ϴ�.");
                // ���� ���� ó��
            }
        }
    }

    void UpdateUI()
    {
        if (currentTamagotchiData == null) return;

        if (tamagotchiNameText != null)
        {
            tamagotchiNameText.text = $"�̸�: {currentTamagotchiData.Name} (ID: {currentTamagotchiData.ID})";
        }
        if (tamagotchiStatusText != null)
        {
            tamagotchiStatusText.text =
                $"ü��: {currentTamagotchiData.Health}\n" +
                $"������: {currentTamagotchiData.Hunger}\n" +
                $"�ູ��: {currentTamagotchiData.Happiness}";
        }
    }

    // --- UI ��ư�� ������ �Լ��� ---
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