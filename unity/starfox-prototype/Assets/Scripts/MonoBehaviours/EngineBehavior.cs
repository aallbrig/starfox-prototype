using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EngineBehavior : MonoBehaviour
{
    public Light enginePointLight;
    private readonly float _waitTimeMin = 0.01f;
    private readonly float _waitTimeMax = 0.1f;
    private readonly float _additionIntensityMin = 0.05f;
    private readonly float _additionIntensityMax = 0.1f;
    private IEnumerator _dynamicFireBehavior;
    private Color _originalColor;

    private void Start()
    {
        if (enginePointLight == null)
        {
            Debug.LogError("Engine Point Light not set!");
            return;
        }
        RememberTheLightColor();
    }

    private void OnEnable()
    {
        _dynamicFireBehavior = DynamicEngineFireBehavior();
        StartCoroutine(_dynamicFireBehavior);
    }

    private void OnDisable()
    {
        if (_dynamicFireBehavior != null) StopCoroutine(_dynamicFireBehavior);
    }

    private void RememberTheLightColor() => _originalColor = enginePointLight.color;
    private IEnumerator DynamicEngineFireBehavior()
    {
        while (true)
        {
            NewFireColor();
            yield return new WaitForSeconds(CalculateNextFireColorChange());
        }
    }

    private void NewFireColor()
    {
        // go brighter or darker?
        var colorToAdd = Random.value < .5 ? Color.black : Color.white;
        // by how much?
        var additionIntensity = Random.Range(_additionIntensityMin, _additionIntensityMax);

        enginePointLight.color = Color.Lerp(_originalColor, colorToAdd, additionIntensity);
    }

    private float CalculateNextFireColorChange() => Random.Range(_waitTimeMin, _waitTimeMax);
}
