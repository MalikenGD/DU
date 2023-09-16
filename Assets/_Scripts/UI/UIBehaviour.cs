using UnityEngine;

public class UIBehaviour : MonoBehaviour
{
    protected object parentObjectWithDataToDisplay;

    public void SetParentObjectWithDataToDisplay(object parentObject)
    {
        parentObjectWithDataToDisplay = parentObject;
    }
    
    
}