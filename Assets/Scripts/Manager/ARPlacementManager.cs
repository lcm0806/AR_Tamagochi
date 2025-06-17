using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems; // TrackableType ��� �� �ʿ�

public class ARPlacementManager : MonoBehaviour
{
    [Header("AR References")]
    public ARRaycastManager arRaycastManager;
    public GameObject placementIndicator; // ��ġ ��ǥ
    public Camera arCamera;

    [Header("Tamagotchi Placement")]
    public float placementDistance = 1.0f; // ī�޶�κ����� ��ġ �Ÿ� (����)
    public Vector3 placementOffset = new Vector3(0, -0.5f, 0); // ��ġ �� ������Ʈ�� y�� ������

    [Header("Tamagotchi Customization")] // ���� �߰��� �κ�
    public Vector3 defaultTamagotchiScale = new Vector3(0.2f, 0.2f, 0.2f); // �⺻ ��ġ ������
    public Vector3 additionalTamagotchiRotation = new Vector3(0, 180, 0); // ��ġ �� �߰� ȸ�� (���Ϸ� ����)

    private Pose placementPose;
    private bool placementPoseIsValid = false; // Raycast ���� ���� (��ǥ�� ���̴���)

    void Start()
    {
        Debug.Log("[ARPlacementManager] Start ȣ���.");
        if (arRaycastManager == null)
        {
            Debug.LogError("[ARPlacementManager] Start: ARRaycastManager�� �Ҵ���� �ʾҽ��ϴ�. AR Session Origin�� ARRaycastManager ������Ʈ�� �ִ��� Ȯ���ϼ���.");
            this.enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
            return;
        }
        if (placementIndicator != null)
        {
            placementIndicator.SetActive(false);
            Debug.Log("[ARPlacementManager] Start: ��ġ ��ǥ �ʱ� ���� ��Ȱ��ȭ.");
        }
        else
        {
            Debug.LogWarning("[ARPlacementManager] Start: placementIndicator�� �Ҵ���� �ʾҽ��ϴ�. ��ġ ��ǥ�� �� �� �����ϴ�.");
        }
    }

    void Update()
    {
        // AR ��� ���� �� ��ǥ ������Ʈ ����
        UpdatePlacementPose();

        // ��ġ �Է� ����
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began) // ��ġ ���� ����
            {
                Debug.Log($"[ARPlacementManager] Update: ��ġ ���� ������. placementPoseIsValid (��ǥ ����): {placementPoseIsValid}");

                // Raycast ���� ���ο� ���� ��ġ ��� ����
                if (!placementPoseIsValid) // Raycast�� ����� ã�� ������ ��
                {
                    Debug.LogWarning("[ARPlacementManager] Update: AR ����� ã�� ���߽��ϴ�. ī�޶� �տ� ������ ��ġ �õ�.");
                    //PlaceTamagotchiInFrontOfCamera();
                }
                else // Raycast�� ����� ã���� �� (��ǥ�� ���� ��)
                {
                    Debug.Log("[ARPlacementManager] Update: AR ��� ������. ��鿡 �ٸ���ġ ��ġ �õ�.");
                    PlaceTamagotchiOnPlane();
                }
            }
        }
    }

    void UpdatePlacementPose()
    {
        // **** ����: Camera.current ��� arCamera ��� ****
        if (arCamera == null || !arCamera.enabled) // arCamera�� null�̰ų� ��Ȱ��ȭ ����
        {
            Debug.LogWarning("[ARPlacementManager] UpdatePlacementPose: �Ҵ�� AR ī�޶� NULL�̰ų� ��Ȱ��ȭ �����Դϴ�. AR ī�޶� �غ���� �ʾҽ��ϴ�.");
            placementPoseIsValid = false;
            if (placementIndicator != null) placementIndicator.SetActive(false);
            return;
        }

        var screenCenter = arCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        // Raycast �õ�
        bool raycastSuccessful = arRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon);

        placementPoseIsValid = raycastSuccessful && hits.Count > 0; // Raycast ���� �� ����� �־�� ��ȿ

        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
            var cameraForward = arCamera.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);

            if (placementIndicator != null)
            {
                placementIndicator.SetActive(true);
                placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
            }
            Debug.Log($"[ARPlacementManager] UpdatePlacementPose: Raycast ����! ��ǥ Ȱ��ȭ. ��ġ: {placementPose.position}");
        }
        else
        {
            if (placementIndicator != null)
                placementIndicator.SetActive(false);
            Debug.Log($"[ARPlacementManager] UpdatePlacementPose: Raycast ���� �Ǵ� ��� ����. ��ǥ ��Ȱ��ȭ. RaycastResult: {raycastSuccessful}, HitsCount: {hits.Count}");
        }
    }

    void PlaceTamagotchiOnPlane()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        GameObject tamagotchiToPlace = GameManager.Instance.GetNextTamagotchiToPlace();

        if (tamagotchiToPlace != null)
        {
            GameObject instantiatedTamagotchi = Instantiate(tamagotchiToPlace, placementPose.position, placementPose.rotation);

            instantiatedTamagotchi.transform.localScale = defaultTamagotchiScale;
            instantiatedTamagotchi.transform.rotation *= Quaternion.Euler(additionalTamagotchiRotation);

            DontDestroyOnLoad(instantiatedTamagotchi);
            GameManager.Instance.AddPlacedTamagotchiInstance(instantiatedTamagotchi);
            Debug.Log($"[ARPlacementManager] PlaceTamagotchiOnPlane: �ٸ���ġ '{tamagotchiToPlace.name}' ��ġ �Ϸ� �� DontDestroyOnLoad/GameManager ��ϵ�.");

            // ��ġ �� ���� �ٸ���ġ�� ���� �ε��� ����
            GameManager.Instance.AdvancePlacementIndex();

            // ��� �ٸ���ġ�� �� ��ġ�ߴ��� Ȯ��
            if (GameManager.Instance.GetCurrentPlacementIndex() >= GameManager.Instance.GetTotalCapturedTamagotchisCount())
            {
                Debug.Log("[ARPlacementManager] ��� ��ȹ�� �ٸ���ġ�� ��ġ�߽��ϴ�. GPS ������ �̵��մϴ�.");
                // ���� ����: ��ġ ��� ���� �޽��� ǥ�� ��
                // placementIndicator.SetActive(false); // ��ǥ ��Ȱ��ȭ
                //SceneManager.LoadScene("GPSScene"); // ��� ��ġ �Ϸ� �� GPS ������ �̵�
            }
        }
        else
        {
            // GetNextTamagotchiToPlace()�� null�� ��ȯ�����Ƿ�, �� �̻� ��ġ�� �ٸ���ġ�� ���ٴ� �ǹ�
            Debug.LogWarning("[ARPlacementManager] �� �̻� ��ġ�� ��ȹ�� �ٸ���ġ�� �����ϴ�. GPS ������ �̵��մϴ�.");
            // �̹� ��� �ٸ���ġ�� ��ġ�߰ų�, ��ȹ�� �ٸ���ġ�� ���� ���
            SceneManager.LoadScene("GPSScene");
        }
    }
}