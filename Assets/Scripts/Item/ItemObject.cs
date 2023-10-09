using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData item;

    public string GetInteractPrompt()
    {
        return string.Format("Pickup {0}", item.displayName);
    }

    // 아이템 줍기
    public void OnInteract()
    {
        //Inventory.instance.AddItem(item);
        Destroy(gameObject);
    }
}
