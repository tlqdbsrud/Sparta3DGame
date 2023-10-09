using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float time;
    public float fullDayLenght; // 하루 시간
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
    public AnimationCurve lightingIntensityMutiplier; // 환경광
    public AnimationCurve reflectionIntensityMultiplier; // 반사광

    private void Start()
    {
        timeRate = 1.0f / fullDayLenght;
        time = startTime;
    }
    private void Update()
    {
        time = (time + timeRate * Time.deltaTime) % 1.0f; // 전체 시간 나누기

        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);

        RenderSettings.ambientIntensity = lightingIntensityMutiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);
    }
    void UpdateLighting(Light lightSource, Gradient colorGradiant, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time);
        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4.0f;
        // noon * 4.0f: 낮 2번, 밤 2번하여 4등분으로 나누어야 함. 90도 * 4 = 360도
        // lightSource == sun ? 0.25f : 0.75f): 빛(해)이 4분의 1 지점에 뜰 때 0.35f, 달이 4분의 1 지점에 뜰 때 해는 4분의 3 지점인 0.75f에 뜬다.
        lightSource.color = colorGradiant.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if (lightSource.intensity == 0 && go.activeInHierarchy)
            go.SetActive(false);
        else if (lightSource.intensity > 0 && !go.activeInHierarchy)
            go.SetActive(true);
    }
}
