using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigatorCamera : MonoBehaviour
{
    // �ν����Ϳ��� � ������ �̵����� �̸��� �����մϴ�.
    public string arSceneName = "CameraScene"; // << ���⿡ ���� ī�޶� �� �̸��� ��������!

    // �� �޼���� UI ��ư�� OnClick �̺�Ʈ�� ����˴ϴ�.
    public void GoToARCameraScene()
    {
        if (string.IsNullOrEmpty(arSceneName))
        {
            Debug.LogError("[SceneNavigator] AR �� �̸��� �������� �ʾҽ��ϴ�. �ν����Ϳ��� 'AR Scene Name'�� Ȯ�����ּ���.");
            return;
        }

        Debug.Log($"[SceneNavigator] '{arSceneName}' ������ �̵��մϴ�...");
        SceneManager.LoadScene(arSceneName);
    }
}
