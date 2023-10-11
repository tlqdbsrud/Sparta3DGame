using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipManager : MonoBehaviour
{
    public Equip curEquip;
    public Transform equipParent;

    private PlayerController controller;
    private PlayerConditions conditions;

    // ½Ì±ÛÅæ
    public static EquipManager instance;

    private void Awake()
    {
        instance = this;
        controller = GetComponent<PlayerController>();
        conditions = GetComponent<PlayerConditions>();
    }

    // °ø°Ý ÀÌº¥Æ®
    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && curEquip != null && controller.canLook)
        {
            curEquip.OnAttackInput(conditions);
        }
    }

    // »õ·Î ÀåÂø
    public void EquipNew(ItemData item)
    {
        UnEquip();
        curEquip = Instantiate(item.equipPrefab, equipParent).GetComponent<Equip>(); // ÀåÂø ¾ÆÀÌÅÛ ÇÁ¸®ÆÕ »ý¼º
    }


    // ÀåÂø ÇØÁ¦
    public void UnEquip()
    {
        if (curEquip != null)
        {
            Destroy(curEquip.gameObject); // ÇöÀç ÀåÂø ¿ÀºêÁ§Æ® Á¦°Å 
            curEquip = null;
        }
    }
}