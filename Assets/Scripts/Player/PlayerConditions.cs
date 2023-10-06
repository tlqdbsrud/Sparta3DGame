using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public interface IDamagable
{
    void TakePhysicalDamage(int damageAmount); // 데미지 처리
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
        curValue = Mathf.Min(curValue + amount, maxValue); // 값을 얼마나 더 할 것인가(MaxValue까지만)
    }

    public void Subtract(float amount)
    {
        curValue = Mathf.Max(curValue - amount, 0.0f); // 값을 얼마나 뺄 것인가(0까지만)
    }

    public float GetPercentage() // UI 바
    {
        return curValue / maxValue;
    }

}


public class PlayerConditions : MonoBehaviour, IDamagable
{
    public Condition health;
    public Condition hunger;
    public Condition stamina;

    public float noHungerHealthDecay;//배고픔 감소, health 감소

    public UnityEvent onTakeDamage;

    void Start()
    {
        // 시작 값
        health.curValue = health.startValue;
        hunger.curValue = hunger.startValue;
        stamina.curValue = stamina.startValue;
    }

    // Update is called once per frame
    void Update()
    {
        hunger.Subtract(hunger.decayRate * Time.deltaTime); // hunger은 주기적으로 decayRate만큼 감소
        stamina.Add(stamina.regenRate * Time.deltaTime); // stamina 주기적으로 regenRate만큼 회복

        if (hunger.curValue == 0.0f) // hnger가 0이면
            health.Subtract(noHungerHealthDecay * Time.deltaTime); // health 감소

        if (health.curValue == 0.0f) //health가 0이면
            Die(); // 사망

        // UI 바
        health.uiBar.fillAmount = health.GetPercentage();
        hunger.uiBar.fillAmount = hunger.GetPercentage();
        stamina.uiBar.fillAmount = stamina.GetPercentage();
    }

    public void Heal(float amount)
    {
        health.Add(amount); // Heal에는 health 추가
    }

    public void Eat(float amount)
    {
        hunger.Add(amount); // 먹으면 hunger 추가
    }

    public bool UseStamina(float amount) 
    {
        if (stamina.curValue - amount < 0) // 현재 stamina를 사용한 값이 0보다 작으면
            return false; //  사용X

        stamina.Subtract(amount); // stamina 사용 후 감소
        return true;
    }

    // 사망
    public void Die() 
    {
        Debug.Log("플레이어가 죽었다.");
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        health.Subtract(damageAmount); // 피해를 입으면 health 감소
        onTakeDamage?.Invoke(); // 이벤트 실행
    }
}