using UnityEngine;

namespace UI
{
    public class MenuCamera : MonoBehaviour
    {
        [SerializeField] private Transform lookAtTarget;
        [SerializeField] private Transform pivot;
        [SerializeField] private Camera cam;
        [SerializeField] private float speed = 15.0f;
        [SerializeField] private float radius = 20.0f;

        private float angle = 3.5f * Mathf.PI;

        private void Update()
        {
            this.angle += Time.deltaTime * this.speed * Mathf.PI / 180.0f;

            var x = this.pivot.position.x + this.radius * Mathf.Cos(this.angle);
            var z = this.pivot.position.z + this.radius * Mathf.Sin(this.angle);
            this.cam.transform.position = new Vector3(x, this.pivot.position.y, z);

            this.cam.transform.LookAt(this.lookAtTarget);
        }
    }
}
