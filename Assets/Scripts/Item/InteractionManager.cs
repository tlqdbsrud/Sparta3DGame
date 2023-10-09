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
    public float checkRate = 0.05f;// Ȯ���ϴ� �ֱ�
    private float lastCheckTime; // ������ Ȯ�� �ð�
    public float maxCheckDistance; // Ȯ���� �� �ִ� �ִ� �Ÿ�
    public LayerMask layerMask;

    private GameObject curInteractGameobject; // ���� ��ȣ�ۿ� ������ ���� ������Ʈ ����
    private IInteractable curInteractable; //���� ��ȣ�ۿ� ������ ���� ������Ʈ�� �������̽� ����

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
        // checkRate ���ݸ��� ����ĳ��Ʈ �����ϱ�
        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time; // ���� �ð� ����

            Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2)); // ȭ�� �߾ӿ� ���� ����
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if (hit.collider.gameObject != curInteractGameobject) // ������ ������Ʈ�� �ٸ��� �浹 ó��
                {
                    curInteractGameobject = hit.collider.gameObject; // ���ο� �浹�� ������Ʈ ����
                    curInteractable = hit.collider.GetComponent<IInteractable>(); // �÷��̾ ��ȣ�ۿ��� �� �ִ� ������Ʈ ã�� ����
                    SetPromptText();
                }
            }
            else
            {
                // �ʱ�ȭ
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

    // EŰ ���� �ݱ�(�̺�Ʈ)
    public void OnInteractInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase == InputActionPhase.Started && curInteractable != null) // E�� �������� �ٷκ��� ������Ʈ�� �ִٸ�
        {
            curInteractable.OnInteract(); // ������ �ݱ�
            
            // �ʱ�ȭ
            curInteractGameobject = null; 
            curInteractable = null; 
            promptText.gameObject.SetActive(false); 
        }
    }
}