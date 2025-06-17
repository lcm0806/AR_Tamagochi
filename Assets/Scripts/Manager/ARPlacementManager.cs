using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems; // TrackableType 사용 시 필요

public class ARPlacementManager : MonoBehaviour
{
    [Header("AR References")]
    public ARRaycastManager arRaycastManager;
    public GameObject placementIndicator; // 배치 지표
    public Camera arCamera;

    [Header("Tamagotchi Placement")]
    public float placementDistance = 1.0f; // 카메라로부터의 배치 거리 (미터)
    public Vector3 placementOffset = new Vector3(0, -0.5f, 0); // 배치 후 오브젝트의 y축 오프셋

    private Pose placementPose;
    private bool placementPoseIsValid = false; // Raycast 성공 여부 (지표가 보이는지)

    void Start()
    {
        Debug.Log("[ARPlacementManager] Start 호출됨.");
        if (arRaycastManager == null)
        {
            Debug.LogError("[ARPlacementManager] Start: ARRaycastManager가 할당되지 않았습니다. AR Session Origin에 ARRaycastManager 컴포넌트가 있는지 확인하세요.");
            this.enabled = false; // 스크립트 비활성화
            return;
        }
        // **** 추가: AR 카메라 참조 확인 ****
        if (arCamera == null)
        {
            Debug.LogError("[ARPlacementManager] Start: AR Camera가 할당되지 않았습니다. 인스펙터에서 할당해주세요.");
            this.enabled = false;
            return;
        }
        else
        {
            Debug.Log("카메라가 할당 되었습니다.");
        }
        if (placementIndicator != null)
        {
            placementIndicator.SetActive(false);
            Debug.Log("[ARPlacementManager] Start: 배치 지표 초기 상태 비활성화.");
        }
        else
        {
            Debug.LogWarning("[ARPlacementManager] Start: placementIndicator가 할당되지 않았습니다. 배치 지표를 볼 수 없습니다.");
        }
    }

    void Update()
    {
        // AR 평면 감지 및 지표 업데이트 로직
        UpdatePlacementPose();

        // 터치 입력 감지
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began) // 터치 시작 시점
            {
                Debug.Log($"[ARPlacementManager] Update: 터치 시작 감지됨. placementPoseIsValid (지표 보임): {placementPoseIsValid}");

                // Raycast 성공 여부에 따라 배치 방식 결정
                if (!placementPoseIsValid) // Raycast로 평면을 찾지 못했을 때
                {
                    Debug.LogWarning("[ARPlacementManager] Update: AR 평면을 찾지 못했습니다. 카메라 앞에 강제로 배치 시도.");
                    //PlaceTamagotchiInFrontOfCamera();
                }
                else // Raycast로 평면을 찾았을 때 (지표가 보일 때)
                {
                    Debug.Log("[ARPlacementManager] Update: AR 평면 감지됨. 평면에 다마고치 배치 시도.");
                    PlaceTamagotchiOnPlane();
                }
            }
        }
    }

    void UpdatePlacementPose()
    {
        // **** 수정: Camera.current 대신 arCamera 사용 ****
        if (arCamera == null || !arCamera.enabled) // arCamera가 null이거나 비활성화 상태
        {
            Debug.LogWarning("[ARPlacementManager] UpdatePlacementPose: 할당된 AR 카메라가 NULL이거나 비활성화 상태입니다. AR 카메라가 준비되지 않았습니다.");
            placementPoseIsValid = false;
            if (placementIndicator != null) placementIndicator.SetActive(false);
            return;
        }

        var screenCenter = arCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();

        // Raycast 시도
        bool raycastSuccessful = arRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon);

        placementPoseIsValid = raycastSuccessful && hits.Count > 0; // Raycast 성공 및 결과가 있어야 유효

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
            Debug.Log($"[ARPlacementManager] UpdatePlacementPose: Raycast 성공! 지표 활성화. 위치: {placementPose.position}");
        }
        else
        {
            if (placementIndicator != null)
                placementIndicator.SetActive(false);
            Debug.Log($"[ARPlacementManager] UpdatePlacementPose: Raycast 실패 또는 평면 없음. 지표 비활성화. RaycastResult: {raycastSuccessful}, HitsCount: {hits.Count}");
        }
    }

    //void PlaceTamagotchiInFrontOfCamera()
    //{
    //    Debug.Log("[ARPlacementManager] PlaceTamagotchiInFrontOfCamera() 호출됨.");
    //    if (GameManager.Instance == null)
    //    {
    //        Debug.LogError("[ARPlacementManager] PlaceTamagotchiInFrontOfCamera: GameManager.Instance를 찾을 수 없습니다. 다마고치를 배치할 수 없습니다.");
    //        return;
    //    }

    //    List<GameObject> capturedTamagotchis = GameManager.Instance.CapturedTamagotchiPrefabs;
    //    Debug.Log($"[ARPlacementManager] PlaceTamagotchiInFrontOfCamera: GameManager에서 가져온 포획된 다마고치 수: {capturedTamagotchis.Count}");

    //    if (capturedTamagotchis.Count > 0)
    //    {
    //        GameObject tamagotchiToPlace = capturedTamagotchis[0];

    //        if (tamagotchiToPlace != null)
    //        {
    //            if (arCamera == null)
    //            {
    //                Debug.LogError("[ARPlacementManager] PlaceTamagotchiInFrontOfCamera: AR Camera가 할당되지 않았습니다. 배치 불가.");
    //                return;
    //            }
    //            Transform cameraTransform = arCamera.transform; // Camera.current 대신 arCamera 사용
    //            Vector3 spawnPosition = cameraTransform.position + cameraTransform.forward * placementDistance;
    //            spawnPosition += placementOffset;

    //            Quaternion spawnRotation = Quaternion.LookRotation(new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z));

    //            Debug.Log($"[ARPlacementManager] PlaceTamagotchiInFrontOfCamera: 다마고치 '{tamagotchiToPlace.name}'을(를) 인스턴스화 시도. 위치: {spawnPosition}, 회전: {spawnRotation}");
    //            GameObject instantiatedTamagotchi = Instantiate(tamagotchiToPlace, spawnPosition, spawnRotation);
    //            Debug.Log($"[ARPlacementManager] PlaceTamagotchiInFrontOfCamera: 다마고치 '{instantiatedTamagotchi.name}'이(가) AR 공간에 성공적으로 배치되었습니다.");
    //            Debug.Log($"[ARPlacementManager] PlaceTamagotchiInFrontOfCamera: 배치된 오브젝트 최종 위치: {instantiatedTamagotchi.transform.position}, 스케일: {instantiatedTamagotchi.transform.localScale}");
    //        }
    //        else
    //        {
    //            Debug.LogError("[ARPlacementManager] PlaceTamagotchiInFrontOfCamera: 포획된 다마고치 리스트에 유효하지 않은 (NULL) 프리팹이 있습니다. GameManager에 추가된 프리팹이 올바른지 확인하세요.");
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogWarning("[ARPlacementManager] PlaceTamagotchiInFrontOfCamera: 포획된 다마고치가 없습니다. AR에 배치할 수 없습니다. (먼저 다마고치를 포획하세요!)");
    //    }
    //}

    void PlaceTamagotchiOnPlane()
    {
        Debug.Log("[ARPlacementManager] PlaceTamagotchiOnPlane() 호출됨.");
        if (GameManager.Instance == null)
        {
            Debug.LogError("[ARPlacementManager] PlaceTamagotchiOnPlane: GameManager.Instance를 찾을 수 없습니다. 다마고치를 배치할 수 없습니다.");
            return;
        }

        List<GameObject> capturedTamagotchis = GameManager.Instance.CapturedTamagotchiPrefabs;
        Debug.Log($"[ARPlacementManager] PlaceTamagotchiOnPlane: GameManager에서 가져온 포획된 다마고치 수: {capturedTamagotchis.Count}");

        if (capturedTamagotchis.Count > 0)
        {
            GameObject tamagotchiToPlace = capturedTamagotchis[0];

            if (tamagotchiToPlace != null)
            {
                Debug.Log($"[ARPlacementManager] PlaceTamagotchiOnPlane: 다마고치 '{tamagotchiToPlace.name}'을(를) 인스턴스화 시도. 위치: {placementPose.position}, 회전: {placementPose.rotation}");
                GameObject instantiatedTamagotchi = Instantiate(tamagotchiToPlace, placementPose.position, placementPose.rotation);
                Debug.Log($"[ARPlacementManager] PlaceTamagotchiOnPlane: 다마고치 '{instantiatedTamagotchi.name}'이(가) AR 공간에 성공적으로 배치되었습니다.");
                Debug.Log($"[ARPlacementManager] PlaceTamagotchiOnPlane: 배치된 오브젝트 최종 위치: {instantiatedTamagotchi.transform.position}, 스케일: {instantiatedTamagotchi.transform.localScale}");
            }
            else
            {
                Debug.LogError("[ARPlacementManager] PlaceTamagotchiOnPlane: 포획된 다마고치 리스트에 유효하지 않은 (NULL) 프리팹이 있습니다.");
            }
        }
        else
        {
            Debug.LogWarning("[ARPlacementManager] PlaceTamagotchiOnPlane: 포획된 다마고치가 없습니다. AR에 배치할 수 없습니다.");
        }
    }
}