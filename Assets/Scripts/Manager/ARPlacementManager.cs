using System.Collections.Generic;
using UnityEngine;
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
        // **** �߰�: AR ī�޶� ���� Ȯ�� ****
        if (arCamera == null)
        {
            Debug.LogError("[ARPlacementManager] Start: AR Camera�� �Ҵ���� �ʾҽ��ϴ�. �ν����Ϳ��� �Ҵ����ּ���.");
            this.enabled = false;
            return;
        }
        else
        {
            Debug.Log("ī�޶� �Ҵ� �Ǿ����ϴ�.");
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

    //void PlaceTamagotchiInFrontOfCamera()
    //{
    //    Debug.Log("[ARPlacementManager] PlaceTamagotchiInFrontOfCamera() ȣ���.");
    //    if (GameManager.Instance == null)
    //    {
    //        Debug.LogError("[ARPlacementManager] PlaceTamagotchiInFrontOfCamera: GameManager.Instance�� ã�� �� �����ϴ�. �ٸ���ġ�� ��ġ�� �� �����ϴ�.");
    //        return;
    //    }

    //    List<GameObject> capturedTamagotchis = GameManager.Instance.CapturedTamagotchiPrefabs;
    //    Debug.Log($"[ARPlacementManager] PlaceTamagotchiInFrontOfCamera: GameManager���� ������ ��ȹ�� �ٸ���ġ ��: {capturedTamagotchis.Count}");

    //    if (capturedTamagotchis.Count > 0)
    //    {
    //        GameObject tamagotchiToPlace = capturedTamagotchis[0];

    //        if (tamagotchiToPlace != null)
    //        {
    //            if (arCamera == null)
    //            {
    //                Debug.LogError("[ARPlacementManager] PlaceTamagotchiInFrontOfCamera: AR Camera�� �Ҵ���� �ʾҽ��ϴ�. ��ġ �Ұ�.");
    //                return;
    //            }
    //            Transform cameraTransform = arCamera.transform; // Camera.current ��� arCamera ���
    //            Vector3 spawnPosition = cameraTransform.position + cameraTransform.forward * placementDistance;
    //            spawnPosition += placementOffset;

    //            Quaternion spawnRotation = Quaternion.LookRotation(new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z));

    //            Debug.Log($"[ARPlacementManager] PlaceTamagotchiInFrontOfCamera: �ٸ���ġ '{tamagotchiToPlace.name}'��(��) �ν��Ͻ�ȭ �õ�. ��ġ: {spawnPosition}, ȸ��: {spawnRotation}");
    //            GameObject instantiatedTamagotchi = Instantiate(tamagotchiToPlace, spawnPosition, spawnRotation);
    //            Debug.Log($"[ARPlacementManager] PlaceTamagotchiInFrontOfCamera: �ٸ���ġ '{instantiatedTamagotchi.name}'��(��) AR ������ ���������� ��ġ�Ǿ����ϴ�.");
    //            Debug.Log($"[ARPlacementManager] PlaceTamagotchiInFrontOfCamera: ��ġ�� ������Ʈ ���� ��ġ: {instantiatedTamagotchi.transform.position}, ������: {instantiatedTamagotchi.transform.localScale}");
    //        }
    //        else
    //        {
    //            Debug.LogError("[ARPlacementManager] PlaceTamagotchiInFrontOfCamera: ��ȹ�� �ٸ���ġ ����Ʈ�� ��ȿ���� ���� (NULL) �������� �ֽ��ϴ�. GameManager�� �߰��� �������� �ùٸ��� Ȯ���ϼ���.");
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogWarning("[ARPlacementManager] PlaceTamagotchiInFrontOfCamera: ��ȹ�� �ٸ���ġ�� �����ϴ�. AR�� ��ġ�� �� �����ϴ�. (���� �ٸ���ġ�� ��ȹ�ϼ���!)");
    //    }
    //}

    void PlaceTamagotchiOnPlane()
    {
        Debug.Log("[ARPlacementManager] PlaceTamagotchiOnPlane() ȣ���.");
        if (GameManager.Instance == null)
        {
            Debug.LogError("[ARPlacementManager] PlaceTamagotchiOnPlane: GameManager.Instance�� ã�� �� �����ϴ�. �ٸ���ġ�� ��ġ�� �� �����ϴ�.");
            return;
        }

        List<GameObject> capturedTamagotchis = GameManager.Instance.CapturedTamagotchiPrefabs;
        Debug.Log($"[ARPlacementManager] PlaceTamagotchiOnPlane: GameManager���� ������ ��ȹ�� �ٸ���ġ ��: {capturedTamagotchis.Count}");

        if (capturedTamagotchis.Count > 0)
        {
            GameObject tamagotchiToPlace = capturedTamagotchis[0];

            if (tamagotchiToPlace != null)
            {
                Debug.Log($"[ARPlacementManager] PlaceTamagotchiOnPlane: �ٸ���ġ '{tamagotchiToPlace.name}'��(��) �ν��Ͻ�ȭ �õ�. ��ġ: {placementPose.position}, ȸ��: {placementPose.rotation}");
                GameObject instantiatedTamagotchi = Instantiate(tamagotchiToPlace, placementPose.position, placementPose.rotation);
                Debug.Log($"[ARPlacementManager] PlaceTamagotchiOnPlane: �ٸ���ġ '{instantiatedTamagotchi.name}'��(��) AR ������ ���������� ��ġ�Ǿ����ϴ�.");
                Debug.Log($"[ARPlacementManager] PlaceTamagotchiOnPlane: ��ġ�� ������Ʈ ���� ��ġ: {instantiatedTamagotchi.transform.position}, ������: {instantiatedTamagotchi.transform.localScale}");
            }
            else
            {
                Debug.LogError("[ARPlacementManager] PlaceTamagotchiOnPlane: ��ȹ�� �ٸ���ġ ����Ʈ�� ��ȿ���� ���� (NULL) �������� �ֽ��ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("[ARPlacementManager] PlaceTamagotchiOnPlane: ��ȹ�� �ٸ���ġ�� �����ϴ�. AR�� ��ġ�� �� �����ϴ�.");
        }
    }
}