using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigatorCamera : MonoBehaviour
{
    // 인스펙터에서 어떤 씬으로 이동할지 이름을 지정합니다.
    public string arSceneName; // << 여기에 실제 카메라 씬 이름을 넣으세요!

    // 이 메서드는 UI 버튼의 OnClick 이벤트에 연결됩니다.
    public void GoToARCameraScene()
    {
        arSceneName = "CameraScene";
        if (string.IsNullOrEmpty(arSceneName))
        {
            Debug.LogError("[SceneNavigator] AR 씬 이름이 설정되지 않았습니다. 인스펙터에서 'AR Scene Name'을 확인해주세요.");
            return;
        }

        Debug.Log($"[SceneNavigator] '{arSceneName}' 씬으로 이동합니다...");
        SceneManager.LoadScene(arSceneName);
    }

    public void GoToGPSScene()
    {
        arSceneName = "GPSScene";
        if (string.IsNullOrEmpty(arSceneName))
        {
            Debug.LogError("[SceneNavigator] AR 씬 이름이 설정되지 않았습니다. 인스펙터에서 'AR Scene Name'을 확인해주세요.");
            return;
        }

        Debug.Log($"[SceneNavigator] '{arSceneName}' 씬으로 이동합니다...");
        SceneManager.LoadScene(arSceneName);
    }
}
