using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        GameObject tamagotchiToPlace = GameManager.Instance.GetNextTamagotchiToPlace();

        if (tamagotchiToPlace != null)
        {
            GameObject instantiatedTamagotchi = Instantiate(tamagotchiToPlace, placementPose.position, placementPose.rotation);

            instantiatedTamagotchi.transform.localScale = defaultTamagotchiScale;
            instantiatedTamagotchi.transform.rotation *= Quaternion.Euler(additionalTamagotchiRotation);

            DontDestroyOnLoad(instantiatedTamagotchi);
            GameManager.Instance.AddPlacedTamagotchiInstance(instantiatedTamagotchi);
            Debug.Log($"[ARPlacementManager] PlaceTamagotchiOnPlane: 다마고치 '{tamagotchiToPlace.name}' 배치 완료 및 DontDestroyOnLoad/GameManager 등록됨.");

            // 배치 후 다음 다마고치를 위해 인덱스 증가
            GameManager.Instance.AdvancePlacementIndex();

            // 모든 다마고치를 다 배치했는지 확인
            if (GameManager.Instance.GetCurrentPlacementIndex() >= GameManager.Instance.GetTotalCapturedTamagotchisCount())
            {
                Debug.Log("[ARPlacementManager] 모든 포획된 다마고치를 배치했습니다. GPS 씬으로 이동합니다.");
                // 선택 사항: 배치 모드 종료 메시지 표시 등
                // placementIndicator.SetActive(false); // 지표 비활성화
                //SceneManager.LoadScene("GPSScene"); // 모든 배치 완료 후 GPS 씬으로 이동
            }
        }
        else
        {
            // GetNextTamagotchiToPlace()가 null을 반환했으므로, 더 이상 배치할 다마고치가 없다는 의미
            Debug.LogWarning("[ARPlacementManager] 더 이상 배치할 포획된 다마고치가 없습니다. GPS 씬으로 이동합니다.");
            // 이미 모든 다마고치를 배치했거나, 포획된 다마고치가 없는 경우
            SceneManager.LoadScene("GPSScene");
        }
    }
}