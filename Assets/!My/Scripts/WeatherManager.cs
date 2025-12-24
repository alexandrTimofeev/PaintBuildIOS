using System;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeatherManager : MonoBehaviour
{
    [SerializeField] private Light sunLight;
    [SerializeField] private Vector2 minMaxLight = new Vector2(0, 5f);
    [SerializeField] private Vector2 minMaxLightAngle = new Vector2(0, 5f);
    [SerializeField] private Vector2 minMaxExpSkybox = new Vector2(0.5f, 3f);
    [SerializeField] private float angleOffset = 90f;

    [Space(1), Header("WatherSate")]
    [SerializeField] private GameObject[] weathers;
    [SerializeField] private TextMeshProUGUI textWeather;

    [Space(1), Header("UI")]
    [SerializeField] private Slider sliderRotation;
    [SerializeField] private Slider sliderLight;

    private int state;

    public float RotationValue => sliderRotation.value;
    public float LightValue => sliderLight.value;
    public int State => state;

    public static WeatherManager Instance;

    void Start()
    {
        Instance = this;

        sliderRotation.onValueChanged.AddListener(SetSliderRotation);
        sliderLight.onValueChanged.AddListener(SetSliderLight);
    }

    public void SetSliderLight(float arg0)
    {
        Material material = RenderSettings.skybox;

        float exp = Mathf.Lerp(minMaxExpSkybox.x, minMaxExpSkybox.y, arg0);
        material.SetFloat("_Exposure", exp);

        Vector3 euler = sunLight.transform.rotation.eulerAngles;
        float xRot = Mathf.Lerp(minMaxLightAngle.x, minMaxLightAngle.y, arg0);
        sunLight.transform.rotation = Quaternion.Euler(xRot, euler.y , euler.z);

        float light = Mathf.Lerp(minMaxLight.x, minMaxLight.y, arg0);
        sunLight.intensity = light;

        RenderSettings.skybox = material;

        SoundManager.PlaySFXNotOverlap(GameEntryGameplayCCh.DataContainer.LightPowerSFX, 0.09f);
    }

    public void SetSliderRotation(float arg0)
    {
        Vector3 euler = sunLight.transform.rotation.eulerAngles;
        float yRot = Mathf.Lerp(0f, 360f, arg0);
        yRot += angleOffset;
        if (yRot > 360)
            yRot -= 360;
        sunLight.transform.rotation = Quaternion.Euler(euler.x, yRot, euler.z);

        Material material = RenderSettings.skybox;

        yRot = Mathf.Lerp(360f, 0f, arg0);
        material.SetFloat("_Rotation", yRot);

        RenderSettings.skybox = material;

        SoundManager.PlaySFXNotOverlap(GameEntryGameplayCCh.DataContainer.LightAngleSFX, 0.09f);
    }

    public void NextWeather()
    {
        int nextState = state + 1;
        if(nextState >= weathers.Length)
            nextState = 0;

        SetWatherState(nextState);
    }

    public void SetSlidersValue (float light, float rotation)
    {
        sliderRotation.SetValueWithoutNotify(rotation);
        sliderLight.SetValueWithoutNotify(light);
    }

    public void SetWatherState(int weatherState)
    {
        state = weatherState;

        for (int i = 0; i < weathers.Length; i++)
            weathers[i].SetActive(i == weatherState);

        textWeather.text = $"{weathers[weatherState].gameObject.name}";
    }

    public void Clear()
    {
        SetWatherState(0);
        SetSliderRotation(0.5f);
        SetSliderLight(0.5f);
        SetSlidersValue(0.5f, 0.5f);
    }
}
