using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickableObject : MonoBehaviour
{
    // Inspector���� ������, �̵��� Scene�� �̸��Դϴ�.
    // ��: "MainMenu", "GameLevel2" ��
    public string targetSceneName = "TestScene";

    // �� �Լ��� �� Gameobject�� �پ��ִ� Collider ������
    // ���콺 ���� ��ư Ŭ�� �Ǵ� ����� ��ġ�� �߻����� �� ȣ��˴ϴ�.
    void OnMouseDown()
    {
        Debug.Log($"'{gameObject.name}' ������Ʈ�� Ŭ��(��ġ)�Ǿ����ϴ�. Scene '{targetSceneName}'���� �̵� �õ�.");

        // targetSceneName�� ������ Scene���� �̵��մϴ�.
        // Scene �̸� ��� Scene�� ���� �ε����� ����� ���� �ֽ��ϴ�: SceneManager.LoadScene(1);
        SceneManager.LoadScene(targetSceneName);

        // (���� ����) Scene ��ȯ �� �� ������Ʈ�� �ı��ϰ� �ʹٸ�:
        Destroy(gameObject);
    }
}
