using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public Image image;
    public float flashSpeed; // 플래시 나타나고 사라지는 속도

    private Coroutine coroutine;

    // 빨간 화면 On
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

    // 서서히 Off
    private IEnumerator FadeAway()
    {
        float startAlpha = 0.3f;
        float a = startAlpha;

        while(a > 0.0f)
        {
            a -= (startAlpha / flashSpeed) * Time.deltaTime;
            // (startAlpha / flashSpeed): 초기 알파값(startAlpha)을 플래시 속도로 나누어 초당 알파값이 얼마나 감소해야 하는지 나타냄.
            // (startAlpha / flashSpeed) * Time.deltaTime: 현재 프레임에서의 시간 간격 곱하기
            // a -= (startAlpha / flashSpeed) * Time.deltaTime: 현재 알파값(a)에서 계산된 감소량을 빼서 새로운 알파값을 얻어 서서히 감소
            image.color = new Color(1.0f, 0.0f, 0.0f, a);
            yield return null;
        }
        image.enabled = false;
    }
    
}
