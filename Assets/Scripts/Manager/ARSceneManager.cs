
using UnityEngine.XR.ARFoundation;
using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;

public class ARSceneManager : MonoBehaviour
{
    [SerializeField] private ARSession arSession; // AR Session ������Ʈ �Ҵ�
    [SerializeField] private XROrigin arSessionOrigin; // AR Session Origin ������Ʈ �Ҵ�

    void OnEnable()
    {
        // AR ������ ���� ���¸� ������ �� �̺�Ʈ ����
        ARSession.stateChanged += OnARSessionStateChanged;
    }

    void OnDisable()
    {
        ARSession.stateChanged -= OnARSessionStateChanged;
    }

    private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        if (args.state == ARSessionState.SessionTracking || args.state == ARSessionState.SessionInitializing)
        {
            Debug.Log($"[ARSceneManager] AR ���� ���� ����: {args.state}");
            // ������ Ʈ��ŷ ���� �� �Ǵ� �ʱ�ȭ ���� �� ��ġ�� �ٸ���ġ���� ó��
            CheckAndRelocatePlacedTamagotchis();
        }
    }

    private void CheckAndRelocatePlacedTamagotchis()
    {
        if (GameManager.Instance == null || GameManager.Instance.PlacedTamagotchiInstances == null)
        {
            Debug.LogWarning("[ARSceneManager] GameManager �Ǵ� ��ġ�� �ٸ���ġ �ν��Ͻ� ����Ʈ�� ã�� �� �����ϴ�.");
            return;
        }

        Debug.Log($"[ARSceneManager] ��ġ�� �ٸ���ġ ���ġ Ȯ�� ��. �� {GameManager.Instance.PlacedTamagotchiInstances.Count}��.");

        foreach (GameObject tamagotchiInstance in GameManager.Instance.PlacedTamagotchiInstances)
        {
            if (tamagotchiInstance != null)
            {
                // ������Ʈ Ȱ��ȭ Ȯ��
                if (!tamagotchiInstance.activeInHierarchy)
                {
                    tamagotchiInstance.SetActive(true);
                    Debug.Log($"[ARSceneManager] �ٸ���ġ '{tamagotchiInstance.name}' Ȱ��ȭ��.");
                }

                // ���� ����: AR ���� �������� ���� ��ġ ������ (������ ��� ���)
                // ����� DontDestroyOnLoad�� �������̸� �ʿ� ���� �� ������,
                // ī�޶� ���µ� �� ������ ��ġ�� ���� ī�޶� �þ� �ۿ� �ִٸ� ������ �����Ƿ�
                // �뷫���� ī�޶� ��/�ֺ����� �̵���Ű�� ������ �߰��� �� ����.
                // ���� ���:
                // if (arSessionOrigin != null && arSessionOrigin.camera != null)
                // {
                //     // ���� AR ī�޶��� �ٷ� �� Ư�� ��ġ�� �ű�� (�׽�Ʈ��)
                //     // tamagotchiInstance.transform.position = arSessionOrigin.camera.transform.position + arSessionOrigin.camera.transform.forward * 1.5f;
                //     // tamagotchiInstance.transform.LookAt(arSessionOrigin.camera.transform.position); // ī�޶� �ٶ󺸰�
                //     // Debug.Log($"[ARSceneManager] �ٸ���ġ '{tamagotchiInstance.name}'�� ī�޶� ������ ���ġ (�׽�Ʈ).");
                // }
            }
        }
        // ��ġ ��ǥ�� �ʿ� ���ٸ� ��Ȱ��ȭ
        if (arSession != null && arSession.gameObject.GetComponent<ARPlacementManager>() != null && arSession.gameObject.GetComponent<ARPlacementManager>().placementIndicator != null)
        {
            // arPlacementManager.placementIndicator.SetActive(false); // �̹� ������Ʈ���� ó���� ���� ����
        }
    }
}