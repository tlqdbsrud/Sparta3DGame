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
    public int quantity; // ����
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

    // �̱���
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

        // UI ���� �ʱ�ȭ
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
        if (inventoryWindow.activeInHierarchy) // ���̾��Ű�� ���� �ִ°�
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

    // �κ��丮 ����
    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    // ������ �߰�
    public void AddItem(ItemData item)
    {
        if (item.canStack) // �ױ� ����
        {
            ItemSlot slotToStackTo = GetItemStack(item);
            if (slotToStackTo != null)
            {
                slotToStackTo.quantity++; // ���� �߰�
                UpdateUI(); // UI ������Ʈ
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot(); // �� ����

        if (emptySlot != null) // �� ����X
        {
            emptySlot.item = item; // ������ �߰�
            emptySlot.quantity = 1; // ���� 1 �߰�
            UpdateUI(); // UI ������Ʈ
            return;
        }

        ThrowItem(item); // ������ ������ ����
    }

    // ������ ������
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
            if (slots[i].item == item && slots[i].quantity < item.maxStackAmount) // ���� ã���� �ϴ� �����۰� ������ �ױ�
                return slots[i];
        }

        return null;
    }

    // �� ���� ã��
    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
                return slots[i]; // ����ٸ� ������ ä���
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
            // �̸��� �ɷ�ġ 
            selectedItemStatNames.text += selectedItem.item.consumables[i].type.ToString() + "\n"; // ���� �ٿ� �Է�
            selectedItemStatValues.text += selectedItem.item.consumables[i].value.ToString() + "\n";
        }

        // ��ư Ȱ��ȭ
        useButton.SetActive(selectedItem.item.type == ItemType.Consumable); // ��� �������� ��� ��ư Ȱ��ȭ
        equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !uiSlots[index].equipped); // ���� �������� ���� ��ư Ȱ��ȭ, ���� �ϰ� �ִ����� Ȯ��(�ߺ� ����)
        unEquipButton.SetActive(selectedItem.item.type == ItemType.Equipable && uiSlots[index].equipped);
        dropButton.SetActive(true);
    }

    // ������ ����
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

    // ������ ���(���� ��ư)
    public void OnUseButton()
    {
        if (selectedItem.item.type == ItemType.Consumable) // ������ ������ = �Ҹ� ������
        {
            for (int i = 0; i < selectedItem.item.consumables.Length; i++)
            {
                switch (selectedItem.item.consumables[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.item.consumables[i].value); break; // Health ä���
                    case ConsumableType.Hunger:
                        condition.Eat(selectedItem.item.consumables[i].value); break; // Hunger ä���
                }
            }
        }
        RemoveSelectedItem(); // ������ ��� �� ����
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

    // ���� ������ ����
    private void RemoveSelectedItem()
    {
        selectedItem.quantity--;

        if (selectedItem.quantity <= 0)
        {
            if (uiSlots[selectedItemIndex].equipped) // ������ ������ �� ���� ���̸� 
            {
                UnEquip(selectedItemIndex); // ���� ����
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