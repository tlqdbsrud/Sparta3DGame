using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float time;
    public float fullDayLenght; // �Ϸ� �ð�
    public float startTime = 0.4f;
    private float timeRate;
    public Vector3 noon;

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMutiplier; // ȯ�汤
    public AnimationCurve reflectionIntensityMultiplier; // �ݻ籤

    private void Start()
    {
        timeRate = 1.0f / fullDayLenght;
        time = startTime;
    }
    private void Update()
    {
        time = (time + timeRate * Time.deltaTime) % 1.0f; // ��ü �ð� ������

        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);

        RenderSettings.ambientIntensity = lightingIntensityMutiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
    }
    void UpdateLighting(Light lightSource, Gradient colorGradiant, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time);
        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4.0f;
        // noon * 4.0f: �� 2��, �� 2���Ͽ� 4������� ������� ��. 90�� * 4 = 360��
        // lightSource == sun ? 0.25f : 0.75f): ��(��)�� 4���� 1 ������ �� �� 0.35f, ���� 4���� 1 ������ �� �� �ش� 4���� 3 ������ 0.75f�� ���.
        lightSource.color = colorGradiant.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if (lightSource.intensity == 0 && go.activeInHierarchy)
            go.SetActive(false);
        else if (lightSource.intensity > 0 && !go.activeInHierarchy)
            go.SetActive(true);
    }
}
