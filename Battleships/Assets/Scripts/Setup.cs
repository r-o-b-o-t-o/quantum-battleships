using UnityEngine;

public class Setup : MonoBehaviour
{
    private void Start()
    {
#if !UNITY_EDITOR
        QualitySettings.vSyncCount = 0;
#else
        QualitySettings.vSyncCount = 1;
#endif
    }
}
