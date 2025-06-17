
using UnityEngine.XR.ARFoundation;
using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;

public class ARSceneManager : MonoBehaviour
{
    [SerializeField] private ARSession arSession; // AR Session 컴포넌트 할당
    [SerializeField] private XROrigin arSessionOrigin; // AR Session Origin 컴포넌트 할당

    void OnEnable()
    {
        // AR 세션이 추적 상태를 변경할 때 이벤트 구독
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
            Debug.Log($"[ARSceneManager] AR 세션 상태 변경: {args.state}");
            // 세션이 트래킹 중일 때 또는 초기화 중일 때 배치된 다마고치들을 처리
            CheckAndRelocatePlacedTamagotchis();
        }
    }

    private void CheckAndRelocatePlacedTamagotchis()
    {
        if (GameManager.Instance == null || GameManager.Instance.PlacedTamagotchiInstances == null)
        {
            Debug.LogWarning("[ARSceneManager] GameManager 또는 배치된 다마고치 인스턴스 리스트를 찾을 수 없습니다.");
            return;
        }

        Debug.Log($"[ARSceneManager] 배치된 다마고치 재배치 확인 중. 총 {GameManager.Instance.PlacedTamagotchiInstances.Count}개.");

        foreach (GameObject tamagotchiInstance in GameManager.Instance.PlacedTamagotchiInstances)
        {
            if (tamagotchiInstance != null)
            {
                // 오브젝트 활성화 확인
                if (!tamagotchiInstance.activeInHierarchy)
                {
                    tamagotchiInstance.SetActive(true);
                    Debug.Log($"[ARSceneManager] 다마고치 '{tamagotchiInstance.name}' 활성화됨.");
                }

                // 선택 사항: AR 세션 기준점에 따라 위치 재조정 (복잡한 고급 기능)
                // 현재는 DontDestroyOnLoad로 독립적이면 필요 없을 수 있지만,
                // 카메라가 리셋될 때 이전에 배치된 곳이 카메라 시야 밖에 있다면 보이지 않으므로
                // 대략적인 카메라 앞/주변으로 이동시키는 로직을 추가할 수 있음.
                // 예를 들어:
                // if (arSessionOrigin != null && arSessionOrigin.camera != null)
                // {
                //     // 현재 AR 카메라의 바로 앞 특정 위치로 옮기기 (테스트용)
                //     // tamagotchiInstance.transform.position = arSessionOrigin.camera.transform.position + arSessionOrigin.camera.transform.forward * 1.5f;
                //     // tamagotchiInstance.transform.LookAt(arSessionOrigin.camera.transform.position); // 카메라를 바라보게
                //     // Debug.Log($"[ARSceneManager] 다마고치 '{tamagotchiInstance.name}'를 카메라 앞으로 재배치 (테스트).");
                // }
            }
        }
        // 배치 지표는 필요 없다면 비활성화
        if (arSession != null && arSession.gameObject.GetComponent<ARPlacementManager>() != null && arSession.gameObject.GetComponent<ARPlacementManager>().placementIndicator != null)
        {
            // arPlacementManager.placementIndicator.SetActive(false); // 이미 업데이트에서 처리될 수도 있음
        }
    }
}