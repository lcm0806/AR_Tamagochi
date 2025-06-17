using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Button, ScrollRect ���� ���� �ʿ�
using TMPro; // TextMeshProUGUI�� ���� �ʿ�
using UnityEngine.SceneManagement; // �� ��ȯ�� ���� �ʿ�

public class CollectionManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject tamagotchiButtonPrefab; // �� �ٸ���ġ�� ǥ���� ��ư ������ (�Ʒ� ���� ����)
    public Transform contentParent; // Scroll View�� Content ������Ʈ (��ư���� �� �θ�)
    public TextMeshProUGUI detailInfoText; // �� ������ ǥ���� TextMeshProUGUI
    public GameObject detailPanel; // �� ���� �г� (�ʱ� ��Ȱ��ȭ)

    [Header("Scene Management")]
    public string gpsSceneName = "GPSScene"; // ���ư� GPS �� �̸�

    private List<GameObject> createdButtons = new List<GameObject>(); // ������ ��ư���� �����ϴ� ����Ʈ

    void Start()
    {
        // UI ��ҵ��� �Ҵ�Ǿ����� Ȯ��
        if (tamagotchiButtonPrefab == null || contentParent == null || detailInfoText == null || detailPanel == null)
        {
            Debug.LogError("CollectionManager UI References�� ��� �Ҵ���� �ʾҽ��ϴ�. Inspector�� Ȯ�����ּ���.");
            enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
            return;
        }

        detailPanel.SetActive(false); // �� ���� �г��� ���� �� ��Ȱ��ȭ
        DisplayCapturedTamagotchis(); // ��ȹ�� �ٸ���ġ ��� ǥ��
    }

    void DisplayCapturedTamagotchis()
    {
        // ������ ������ ��ư�� �ִٸ� ��� ����
        foreach (GameObject buttonGO in createdButtons)
        {
            Destroy(buttonGO);
        }
        createdButtons.Clear();

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance�� ã�� �� �����ϴ�.");
            return;
        }

        List<GameObject> capturedPrefabs = GameManager.Instance.CapturedTamagotchiPrefabs;

        if (capturedPrefabs.Count == 0)
        {
            detailInfoText.text = "���� ��ȹ�� �ٸ���ġ�� �����ϴ�.";
            Debug.Log("��ȹ�� �ٸ���ġ�� �����ϴ�.");
            return;
        }

        Debug.Log($"��ȹ�� �ٸ���ġ ��: {capturedPrefabs.Count}");

        foreach (GameObject prefab in capturedPrefabs)
        {
            if (prefab != null)
            {
                // ��ư ����
                IDamagotchi tamagotchiData = prefab.GetComponent<IDamagotchi>();

                if (tamagotchiData == null)
                {
                    Debug.LogWarning($"������ '{prefab.name}'�� IDamagotchi ������Ʈ�� ã�� �� �����ϴ�. ��ư �̸��� ������ �̸����� �����մϴ�.");
                    // IDamagotchi�� ������ prefab.name�� �⺻������ ���
                    // �Ǵ� �� ��� ��ư�� �������� ���� ���� �ֽ��ϴ�. (����)
                    // continue; // �� �ٸ���ġ�� �ǳʶٱ�
                }


                GameObject buttonGO = Instantiate(tamagotchiButtonPrefab, contentParent);
                createdButtons.Add(buttonGO); // ������ ��ư ����Ʈ�� �߰�

                // ��ư�� TextMeshProUGUI ������Ʈ ã�� (��ư ������ ���ο� TextMeshProUGUI�� �ִٰ� ����)
                TextMeshProUGUI buttonText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    if (tamagotchiData != null)
                    {
                        buttonText.text = tamagotchiData.Name; // IDamagotchi���� ������ �̸� ���
                    }
                    else
                    {
                        buttonText.text = prefab.name; // IDamagotchi�� ������ ������ �̸����� ����
                    }
                }
                else
                {
                    Debug.LogWarning($"��ư ������ '{tamagotchiButtonPrefab.name}'�� TextMeshProUGUI ������Ʈ�� ã�� �� �����ϴ�.");
                }

                // ��ư Ŭ�� �̺�Ʈ �߰�
                Button button = buttonGO.GetComponent<Button>();
                if (button != null)
                {
                    // Ŭ���� ������ �����ϱ� ���� ���� ������ ĸó
                    GameObject currentPrefab = prefab;
                    button.onClick.AddListener(() => OnTamagotchiSelected(currentPrefab));
                }
                else
                {
                    Debug.LogWarning($"��ư ������ '{tamagotchiButtonPrefab.name}'�� Button ������Ʈ�� ã�� �� �����ϴ�.");
                }
            }
        }
    }

    // �ٸ���ġ ��ư Ŭ�� �� ȣ��� �Լ�
    void OnTamagotchiSelected(GameObject selectedPrefab)
    {
        Debug.Log($"�ٸ���ġ ���õ�: {selectedPrefab.name}");
        detailPanel.SetActive(true); // �� ���� �г� Ȱ��ȭ

        // ���õ� �ٸ���ġ �������� IDamagotchi ��ũ��Ʈ ������Ʈ�� ������
        // ����: �� �������� ���� �ν��Ͻ��� �ƴϹǷ�, ������ ��ü�� �پ��ִ� ��ũ��Ʈ ������ �о�� �մϴ�.
        IDamagotchi tamagotchiData = selectedPrefab.GetComponent<IDamagotchi>();

        if (tamagotchiData != null)
        {
            // ������ UI �ؽ�Ʈ�� ǥ��
            detailInfoText.text =
                $"�̸�: {tamagotchiData.Name}\n" +
                $"ID: {tamagotchiData.ID}\n" +
                $"ü��: {tamagotchiData.Health}\n" +
                $"�����: {tamagotchiData.Hunger}\n" +
                $"�ູ��: {tamagotchiData.Happiness}\n";
        }
        else
        {
            detailInfoText.text = $"���õ� �ٸ���ġ ({selectedPrefab.name})�� IDamagotchi ��ũ��Ʈ�� �����ϴ�.";
            Debug.LogWarning($"������ '{selectedPrefab.name}'�� IDamagotchi ������Ʈ�� �����ϴ�.");
        }
    }

    // �� ���� �г��� "�ݱ�" �Ǵ� "Ȯ��" ��ư�� ������ �Լ�
    public void OnDetailPanelClose()
    {
        detailPanel.SetActive(false);
    }

    // �ڷ� ���� ��ư�� ������ �Լ�
    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene(gpsSceneName);
    }
}