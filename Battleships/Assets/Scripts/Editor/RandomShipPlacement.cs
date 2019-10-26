using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShipPlacement))]
public class RandomShipPlacement : Editor
{
    public override void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            if (GUILayout.Button("Place ships randomly"))
            {
                ShipPlacement.Instance.PlaceRandomly();
            }
        }
    }
}
