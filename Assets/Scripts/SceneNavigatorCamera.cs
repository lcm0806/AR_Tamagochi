using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigatorCamera : MonoBehaviour
{
    // �ν����Ϳ��� � ������ �̵����� �̸��� �����մϴ�.
    public string arSceneName; // << ���⿡ ���� ī�޶� �� �̸��� ��������!

    // �� �޼���� UI ��ư�� OnClick �̺�Ʈ�� ����˴ϴ�.
    public void GoToARCameraScene()
    {
        arSceneName = "CameraScene";
        if (string.IsNullOrEmpty(arSceneName))
        {
            Debug.LogError("[SceneNavigator] AR �� �̸��� �������� �ʾҽ��ϴ�. �ν����Ϳ��� 'AR Scene Name'�� Ȯ�����ּ���.");
            return;
        }

        Debug.Log($"[SceneNavigator] '{arSceneName}' ������ �̵��մϴ�...");
        SceneManager.LoadScene(arSceneName);
    }

    public void GoToGPSScene()
    {
        arSceneName = "GPSScene";
        if (string.IsNullOrEmpty(arSceneName))
        {
            Debug.LogError("[SceneNavigator] AR �� �̸��� �������� �ʾҽ��ϴ�. �ν����Ϳ��� 'AR Scene Name'�� Ȯ�����ּ���.");
            return;
        }

        Debug.Log($"[SceneNavigator] '{arSceneName}' ������ �̵��մϴ�...");
        SceneManager.LoadScene(arSceneName);
    }
}
