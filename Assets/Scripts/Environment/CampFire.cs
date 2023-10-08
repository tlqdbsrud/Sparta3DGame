using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    public int damage;
    public float damageRate; // ������ �޴� �ӵ�

    private List<IDamagable> thingsToDamage = new List<IDamagable>(); // ������ ���� ������Ʈ ����Ʈ

    private void Start()
    {
        InvokeRepeating("DealDamage", 0, damageRate); // DealDamage�� �ֱ������� ����, damageRate�� �������� ������ �ֱ�
    }

    // ������ �ֱ�
    void DealDamage()
    {
        for (int i = 0; i < thingsToDamage.Count; i++)
        {
            thingsToDamage[i].TakePhysicalDamage(damage); // ����Ʈ�� �ִ� ������Ʈ�� ������ �ֱ�
        }
    }

    // �Ұ� �ٸ� ������Ʈ�� �浹O
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out IDamagable damagable))
        {
            thingsToDamage.Add(damagable); // �浹�� ������Ʈ �߰�
        }
    }

    // �Ұ� �ٸ� ������Ʈ�� �浹X
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamagable damagable))
        {
            thingsToDamage.Remove(damagable); // �浹���� ������ ������Ʈ ����
        }
    }
}
