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
        Inventory.instance.AddItem(item); // E키 누를 때 아이템 줍기
        Destroy(gameObject); // 아이템 주우면 게임상에 제거
    }
}
