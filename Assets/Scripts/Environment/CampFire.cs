using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    public int damage;
    public float damageRate; // 데미지 받는 속도

    private List<IDamagable> thingsToDamage = new List<IDamagable>(); // 데미지 받을 오브젝트 리스트

    private void Start()
    {
        InvokeRepeating("DealDamage", 0, damageRate); // DealDamage를 주기적으로 실행, damageRate의 간격으로 데미지 주기
    }

    // 데미지 주기
    void DealDamage()
    {
        for (int i = 0; i < thingsToDamage.Count; i++)
        {
            thingsToDamage[i].TakePhysicalDamage(damage); // 리스트에 있는 오브젝트에 데미지 주기
        }
    }

    // 불과 다른 오브젝트가 충돌O
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out IDamagable damagable))
        {
            thingsToDamage.Add(damagable); // 충돌한 오브젝트 추가
        }
    }

    // 불과 다른 오브젝트가 충돌X
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamagable damagable))
        {
            thingsToDamage.Remove(damagable); // 충돌하지 않으면 오브젝트 제거
        }
    }
}
