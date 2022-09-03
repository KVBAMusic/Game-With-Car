/* using UnityEngine;
using UnityEditor;
using System.Collections;

public class ChangeColour : MonoBehaviour
{
    public Material material;
    public Color colour;
    // Use this for initialization

    public void ChangeMaterialColour()
    {
        material.SetColor("_BaseColor", colour);
        AssetDatabase.CreateAsset(material, "Assets/Materials/Car.mat");
    }
} */