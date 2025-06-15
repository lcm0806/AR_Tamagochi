using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickableObject : MonoBehaviour
{
    // Inspector에서 설정할, 이동할 Scene의 이름입니다.
    // 예: "MainMenu", "GameLevel2" 등
    public string targetSceneName = "TestScene";

    // 이 함수는 이 Gameobject에 붙어있는 Collider 위에서
    // 마우스 왼쪽 버튼 클릭 또는 모바일 터치가 발생했을 때 호출됩니다.
    void OnMouseDown()
    {
        Debug.Log($"'{gameObject.name}' 오브젝트가 클릭(터치)되었습니다. Scene '{targetSceneName}'으로 이동 시도.");

        // targetSceneName에 지정된 Scene으로 이동합니다.
        // Scene 이름 대신 Scene의 빌드 인덱스를 사용할 수도 있습니다: SceneManager.LoadScene(1);
        SceneManager.LoadScene(targetSceneName);

        // (선택 사항) Scene 전환 후 이 오브젝트를 파괴하고 싶다면:
        Destroy(gameObject);
    }
}
