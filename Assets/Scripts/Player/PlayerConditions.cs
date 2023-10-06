using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public interface IDamagable
{
    void TakePhysicalDamage(int damageAmount); // ������ ó��
}

[System.Serializable]
public class Condition
{
    [HideInInspector]
    public float curValue;
    public float maxValue;
    public float startValue;
    public float regenRate;
    public float decayRate;
    public Image uiBar;

    public void Add(float amount) 
    {
        curValue = Mathf.Min(curValue + amount, maxValue); // ���� �󸶳� �� �� ���ΰ�(MaxValue������)
    }

    public void Subtract(float amount)
    {
        curValue = Mathf.Max(curValue - amount, 0.0f); // ���� �󸶳� �� ���ΰ�(0������)
    }

    public float GetPercentage() // UI ��
    {
        return curValue / maxValue;
    }

}


public class PlayerConditions : MonoBehaviour, IDamagable
{
    public Condition health;
    public Condition hunger;
    public Condition stamina;

    public float noHungerHealthDecay;//����� ����, health ����

    public UnityEvent onTakeDamage;

    void Start()
    {
        // ���� ��
        health.curValue = health.startValue;
        hunger.curValue = hunger.startValue;
        stamina.curValue = stamina.startValue;
    }

    // Update is called once per frame
    void Update()
    {
        hunger.Subtract(hunger.decayRate * Time.deltaTime); // hunger�� �ֱ������� decayRate��ŭ ����
        stamina.Add(stamina.regenRate * Time.deltaTime); // stamina �ֱ������� regenRate��ŭ ȸ��

        if (hunger.curValue == 0.0f) // hnger�� 0�̸�
            health.Subtract(noHungerHealthDecay * Time.deltaTime); // health ����

        if (health.curValue == 0.0f) //health�� 0�̸�
            Die(); // ���

        // UI ��
        health.uiBar.fillAmount = health.GetPercentage();
        hunger.uiBar.fillAmount = hunger.GetPercentage();
        stamina.uiBar.fillAmount = stamina.GetPercentage();
    }

    public void Heal(float amount)
    {
        health.Add(amount); // Heal���� health �߰�
    }

    public void Eat(float amount)
    {
        hunger.Add(amount); // ������ hunger �߰�
    }

    public bool UseStamina(float amount) 
    {
        if (stamina.curValue - amount < 0) // ���� stamina�� ����� ���� 0���� ������
            return false; //  ���X

        stamina.Subtract(amount); // stamina ��� �� ����
        return true;
    }

    // ���
    public void Die() 
    {
        Debug.Log("�÷��̾ �׾���.");
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        health.Subtract(damageAmount); // ���ظ� ������ health ����
        onTakeDamage?.Invoke(); // �̺�Ʈ ����
    }
}