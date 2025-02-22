using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlatformColors : MonoBehaviour
{
    [SerializeField] Material verticalArrowsMaterial;
    [SerializeField] Material horizontalArrowsMaterial;
    [SerializeField] Material verticalOctagonMaterial;
    [SerializeField] Material horizontalOctagonMaterial;

    [SerializeField] WorldData worldDataSO;
    [SerializeField] IntSO currentWorldSO;
    private void Awake()
    {
        currentWorldSO.value = PlayerPrefs.GetInt("WorldIndex", 0);
        UpdateColors(); 
    }

    private void UpdateColors()
    {

        UpdateColorForMaterial(verticalArrowsMaterial, worldDataSO.worlds[currentWorldSO.value].verticalOutPlatformColor);
        UpdateColorForMaterial(horizontalArrowsMaterial, worldDataSO.worlds[currentWorldSO.value].horizontalOutPlatformColor);
        UpdateColorForMaterial(verticalOctagonMaterial, worldDataSO.worlds[currentWorldSO.value].verticalInPlatformColor);
        UpdateColorForMaterial(horizontalOctagonMaterial, worldDataSO.worlds[currentWorldSO.value].horizontalInPlatformColor);

        UpdateAlphaValue(verticalOctagonMaterial, worldDataSO.worlds[currentWorldSO.value].aplhaValue);
        UpdateAlphaValue(horizontalOctagonMaterial, worldDataSO.worlds[currentWorldSO.value].aplhaValue);
    }

    void UpdateColorForMaterial(Material material, Color color)
    {
        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }
        if (material.HasProperty("_Base_color"))
        {
            material.SetColor("_Base_color", color);
        }
    }

    private void UpdateAlphaValue(Material material, float alphaValue)
    {
        if (material.HasProperty("_Alpha_value"))
        {
            material.SetFloat("_Alpha_value", alphaValue);
        }
        else
        {
            Debug.LogWarning($"Material {material.name} does not have an '_Alpha_value' property.");
        }
    }
}
