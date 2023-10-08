using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public Image image;
    public float flashSpeed; // �÷��� ��Ÿ���� ������� �ӵ�

    private Coroutine coroutine;

    // ���� ȭ�� On
    public void Flash()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        image.enabled = true;
        image.color = Color.red;
        coroutine = StartCoroutine(FadeAway());
    }

    // ������ Off
    private IEnumerator FadeAway()
    {
        float startAlpha = 0.3f;
        float a = startAlpha;

        while(a > 0.0f)
        {
            a -= (startAlpha / flashSpeed) * Time.deltaTime;
            // (startAlpha / flashSpeed): �ʱ� ���İ�(startAlpha)�� �÷��� �ӵ��� ������ �ʴ� ���İ��� �󸶳� �����ؾ� �ϴ��� ��Ÿ��.
            // (startAlpha / flashSpeed) * Time.deltaTime: ���� �����ӿ����� �ð� ���� ���ϱ�
            // a -= (startAlpha / flashSpeed) * Time.deltaTime: ���� ���İ�(a)���� ���� ���ҷ��� ���� ���ο� ���İ��� ��� ������ ����
            image.color = new Color(1.0f, 0.0f, 0.0f, a);
            yield return null;
        }
        image.enabled = false;
    }
    
}
