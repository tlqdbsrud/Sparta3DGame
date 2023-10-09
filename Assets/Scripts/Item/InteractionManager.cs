using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    string GetInteractPrompt();
    void OnInteract();
}

public class InteractionManager : MonoBehaviour
{
    public float checkRate = 0.05f;// 확인하는 주기
    private float lastCheckTime; // 마지막 확인 시간
    public float maxCheckDistance; // 확인할 수 있는 최대 거리
    public LayerMask layerMask;

    private GameObject curInteractGameobject; // 현재 상호작용 가능한 게임 오브젝트 저장
    private IInteractable curInteractable; //현재 상호작용 가능한 게임 오브젝트의 인터페이스 저장

    public TextMeshProUGUI promptText; 
    private Camera camera;


    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // checkRate 간격마다 레이캐스트 실행하기
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time; // 현재 시간 저장

            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2)); // 화면 중앙에 레이 생성
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if (hit.collider.gameObject != curInteractGameobject) // 저장한 오브젝트와 다르면 충돌 처리
                {
                    curInteractGameobject = hit.collider.gameObject; // 새로운 충돌한 오브젝트 저장
                    curInteractable = hit.collider.GetComponent<IInteractable>(); // 플레이어가 상호작용할 수 있는 오브젝트 찾는 역할
                    SetPromptText();
                }
            }
            else
            {
                // 초기화
                curInteractGameobject = null;
                curInteractable = null;
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptText()
    {
        promptText.gameObject.SetActive(true);
        promptText.text = string.Format("<b>[E]</b> {0}", curInteractable.GetInteractPrompt());
    }

    // E키 눌러 줍기(이벤트)
    public void OnInteractInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase == InputActionPhase.Started && curInteractable != null) // E가 눌러졌고 바로보는 오브젝트가 있다면
        {
            curInteractable.OnInteract(); // 아이템 줍기
            
            // 초기화
            curInteractGameobject = null; 
            curInteractable = null; 
            promptText.gameObject.SetActive(false); 
        }
    }
}