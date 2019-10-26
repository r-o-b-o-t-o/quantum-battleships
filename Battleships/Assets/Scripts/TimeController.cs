using UnityEngine;

public class TimeController : MonoBehaviour
{
    [SerializeField] private Transform directionalLight;
    [SerializeField] private float timeScale = 0.5f;

    private void Update()
    {
        float rotation = this.timeScale;
        this.directionalLight.transform.Rotate(rotation * Vector3.right * Time.deltaTime);
    }
}
