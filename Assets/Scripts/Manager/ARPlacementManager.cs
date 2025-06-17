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

    [Header("Tamagotchi Customization")] // 새로 추가된 부분
    public Vector3 defaultTamagotchiScale = new Vector3(0.2f, 0.2f, 0.2f); // 기본 배치 스케일
    public Vector3 additionalTamagotchiRotation = new Vector3(0, 180, 0); // 배치 후 추가 회전 (오일러 각도)

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

    void PlaceTamagotchiOnPlane()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        List<GameObject> capturedTamagotchis = GameManager.Instance.CapturedTamagotchiPrefabs;
        Debug.Log($"[ARPlacementManager] PlaceTamagotchiOnPlane: GameManager에서 가져온 포획된 다마고치 수: {capturedTamagotchis.Count}");

        if (capturedTamagotchis.Count > 0)
        {
            //여기서 어떤 오브젝트를 배치할지 설정
            GameObject tamagotchiToPlace = capturedTamagotchis[0];

            if (tamagotchiToPlace != null)
            {
                GameObject instantiatedTamagotchi = Instantiate(tamagotchiToPlace, placementPose.position, placementPose.rotation);
                instantiatedTamagotchi.transform.localScale = defaultTamagotchiScale; // Inspector에서 설정한 크기로 설정
                // 기존 placementPose.rotation에 추가적인 회전을 더함
                instantiatedTamagotchi.transform.rotation *= Quaternion.Euler(additionalTamagotchiRotation);

                DontDestroyOnLoad(instantiatedTamagotchi);
                GameManager.Instance.AddPlacedTamagotchiInstance(instantiatedTamagotchi);
                Debug.Log($"[ARPlacementManager] PlaceTamagotchiOnPlane: 다마고치 '{tamagotchiToPlace.name}' 배치 완료 및 DontDestroyOnLoad 설정됨.");

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