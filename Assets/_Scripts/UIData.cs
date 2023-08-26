using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIData", menuName = "ScriptableObjects/UI/UIData")]
public class UIData : ScriptableObject
{
    //TODO: Convert to Methods? Or Getters?
    public GameObject uiManagerPrefab;
    public Texture2D image;
    public Canvas shopUICanvas;
    public Canvas cardCanvas;
    public Canvas gridCanvas;
}
