using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedButton : MonoBehaviour
{
    // 이 버튼이 상호작용할 IDamagotchi 인스턴스 참조
    private IDamagotchi targetTamagotchi;

    // 먹이를 줄 양
    public int feedAmount = 10;

    // 이 스크립트가 활성화될 때 (모델이 생성될 때) TamagotchiDisplayManager로부터 데이터를 받아올 준비를 합니다.
    // Public 함수로 만들어서 외부에서 이 다마고치 모델의 데이터를 설정할 수 있게 합니다.
    public void SetTargetTamagotchi(IDamagotchi tamaData)
    {
        targetTamagotchi = tamaData;
        Debug.Log($"FeedButton 스크립트에 다마고치 '{targetTamagotchi.Name}' 데이터가 설정되었습니다.");
    }

    // 실제로 먹이를 주는 로직. 이 함수는 UI 버튼 클릭 이벤트나 3D 모델 클릭 이벤트에 연결될 수 있습니다.
    public void OnFeedClicked()
    {
        if (targetTamagotchi == null)
        {
            Debug.LogError("FeedButton: 상호작용할 다마고치 데이터가 설정되지 않았습니다!");
            return;
        }

        Debug.Log($"FeedButton: {targetTamagotchi.Name}에게 먹이주기 버튼이 클릭되었습니다.");
        targetTamagotchi.Feed(feedAmount);

        // 먹이를 준 후 UI를 업데이트해야 하므로, TamagotchiDisplayManager에 알림
        // 직접 참조하거나, 이벤트를 발생시키거나, 아니면 Update()에서 주기적으로 갱신되도록 할 수 있습니다.
        // 여기서는 TamagotchiDisplayManager가 Update()에서 주기적으로 UI를 갱신한다고 가정합니다.
        // 또는 TamagotchiDisplayManager에 Public UpdateUI() 함수를 만들고 여기서 호출할 수도 있습니다.
        // 예: FindObjectOfType<TamagotchiDisplayManager>()?.UpdateUI(); (성능에 좋지 않음)
        // 더 나은 방법은 TamagotchiDisplayManager가 currentTamagotchiData의 변경을 감지하도록 하는 것입니다.
    }

    // (선택 사항) 3D 모델 자체를 클릭했을 때 먹이를 주려면 Collider와 OnMouseDown()을 사용합니다.
    // 현재는 UI 버튼 클릭 방식이므로 이 부분은 필요 없을 수 있습니다.
    /*
    void OnMouseDown()
    {
        OnFeedClicked(); // 3D 모델 클릭 시 먹이 주기 함수 호출
    }
    */
}
