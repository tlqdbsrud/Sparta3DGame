using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class ItemSlot
{
    public ItemData item;
    public int quantity; // 수량
}

public class Inventory : MonoBehaviour
{
    public ItemSlotUI[] uiSlots;
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform dropPosition;

    [Header("Selected Item")]
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatNames;
    public TextMeshProUGUI selectedItemStatValues;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    private int curEquipIndex;

    private PlayerController controller;
    private PlayerConditions condition;

    [Header("Events")]
    public UnityEvent onOpenInventory;
    public UnityEvent onCloseInventory;

    // 싱글톤
    public static Inventory instance;
    void Awake()
    {
        instance = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerConditions>();
    }
    private void Start()
    {
        inventoryWindow.SetActive(false);
        slots = new ItemSlot[uiSlots.Length];

        // UI 슬롯 초기화
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = new ItemSlot();
            uiSlots[i].index = i;
            uiSlots[i].Clear();
        }

        ClearSeletecItemWindow();
    }

    public void OnInventoryButton(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase == InputActionPhase.Started)
        {
            Toggle();
        }
    }


    public void Toggle()
    {
        if (inventoryWindow.activeInHierarchy) // 하이어라키에 켜져 있는가
        {
            inventoryWindow.SetActive(false);
            onCloseInventory?.Invoke();
            controller.ToggleCursor(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
            onOpenInventory?.Invoke();
            controller.ToggleCursor(true);
        }
    }

    // 인벤토리 열기
    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    // 아이템 추가
    public void AddItem(ItemData item)
    {
        if (item.canStack) // 쌓기 가능
        {
            ItemSlot slotToStackTo = GetItemStack(item);
            if (slotToStackTo != null)
            {
                slotToStackTo.quantity++; // 수량 추가
                UpdateUI(); // UI 업데이트
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot(); // 빈 슬롯

        if (emptySlot != null) // 빈 슬롯X
        {
            emptySlot.item = item; // 아이템 추가
            emptySlot.quantity = 1; // 수량 1 추가
            UpdateUI(); // UI 업데이트
            return;
        }

        ThrowItem(item); // 꽉차면 아이템 버림
    }

    // 아이템 버리기
    void ThrowItem(ItemData item)
    {
        Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360f));
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
                uiSlots[i].Set(slots[i]);
            else
                uiSlots[i].Clear();
        }
    }

    ItemSlot GetItemStack(ItemData item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item && slots[i].quantity < item.maxStackAmount) // 내가 찾고자 하는 아이템과 같으면 쌓기
                return slots[i];
        }

        return null;
    }

    // 빈 슬롯 찾기
    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
                return slots[i]; // 비었다면 아이템 채우기
        }

        return null;
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null)
            return;

        selectedItem = slots[index];
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.item.displayName;
        selectedItemDescription.text = selectedItem.item.description;

        selectedItemStatNames.text = string.Empty;
        selectedItemStatValues.text = string.Empty;

        for (int i = 0; i < selectedItem.item.consumables.Length; i++)
        {
            // 이름과 능력치 
            selectedItemStatNames.text += selectedItem.item.consumables[i].type.ToString() + "\n"; // 다음 줄에 입력
            selectedItemStatValues.text += selectedItem.item.consumables[i].value.ToString() + "\n";
        }

        // 버튼 활성화
        useButton.SetActive(selectedItem.item.type == ItemType.Consumable); // 사용 아이템은 사용 버튼 활성화
        equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !uiSlots[index].equipped); // 장착 아이템은 장착 버튼 활성화, 장착 하고 있는지도 확인(중복 예방)
        unEquipButton.SetActive(selectedItem.item.type == ItemType.Equipable && uiSlots[index].equipped);
        dropButton.SetActive(true);
    }

    // 아이템 제거
    private void ClearSeletecItemWindow()
    {
        selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;

        selectedItemStatNames.text = string.Empty;
        selectedItemStatValues.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    // 아이템 사용(실행 버튼)
    public void OnUseButton()
    {
        if (selectedItem.item.type == ItemType.Consumable) // 선택한 아이템 = 소모 아이템
        {
            for (int i = 0; i < selectedItem.item.consumables.Length; i++)
            {
                switch (selectedItem.item.consumables[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.item.consumables[i].value); break; // Health 채우기
                    case ConsumableType.Hunger:
                        condition.Eat(selectedItem.item.consumables[i].value); break; // Hunger 채우기
                }
            }
        }
        RemoveSelectedItem(); // 아이템 사용 후 제거
    }

    public void OnEquipButton()
    {

    }

    void UnEquip(int index)
    {

    }

    public void OnUnEquipButton()
    {

    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem.item);
        RemoveSelectedItem();
    }

    // 선택 아이템 제거
    private void RemoveSelectedItem()
    {
        selectedItem.quantity--;

        if (selectedItem.quantity <= 0)
        {
            if (uiSlots[selectedItemIndex].equipped) // 아이템 제거할 때 장착 중이면 
            {
                UnEquip(selectedItemIndex); // 장착 해제
            }

            selectedItem.item = null;
            ClearSeletecItemWindow();
        }

        UpdateUI(); 
    }

    public void RemoveItem(ItemData item)
    {

    }

    public bool HasItems(ItemData item, int quantity)
    {
        return false;
    }
}