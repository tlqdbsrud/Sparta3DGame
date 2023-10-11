using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ItemData itemToGive;
    public int quantityPerHit = 1;
    public int capacity;

    public void Gather(Vector3 hitPoint, Vector3 hitNormal)
    {
        for (int i = 0; i < quantityPerHit; i++)
        {
            if (capacity <= 0) { break; }
            capacity -= 1; 

            
            Instantiate(itemToGive.dropPrefab, hitPoint + Vector3.up, Quaternion.LookRotation(hitNormal, Vector3.up));
            // 충돌이 일어난 나를 바라보는 방향서 생성
        }

        if (capacity <= 0) // capacity가 0보다 작아지면 오브젝트 제거
            Destroy(gameObject);
    }
}