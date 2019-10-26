using UnityEngine;
using UnityEngine.EventSystems;

public class BombPlacement : MonoBehaviourSingleton<BombPlacement>
{
    public System.Action<Vector2Int> onBombPlaced;

    [SerializeField] private Camera cam;
    [SerializeField] private MeshFilter ocean;
    [SerializeField] private GameObject bombPrefab;
    private GameObject spawnedBomb;
    private Vector2Int mousePos;
    private bool active;

    private void Start()
    {
        this.spawnedBomb = GameObject.Instantiate(this.bombPrefab);
        this.Disable();
    }

    private void Update()
    {
        if (!this.active)
        {
            return;
        }

        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = this.cam.ScreenPointToRay(Input.mousePosition);
        Vector3 mouseWorldPos;
        float dist;
        if (plane.Raycast(ray, out dist))
        {
            mouseWorldPos = ray.GetPoint(dist);
            Vector2Int point = Ship.WorldPosToGridCoords(mouseWorldPos);
            point.x = Mathf.Clamp(point.x, 0, 9);
            point.y = Mathf.Clamp(point.y, 0, 9);
            if (point != this.mousePos)
            {
                this.mousePos = point;
                Vector3 newPos = this.spawnedBomb.transform.position;
                Vector3 snappedPos = Ship.GridCoordsToWorldPos(point);
                newPos.x = snappedPos.x;
                newPos.z = snappedPos.z;
                this.spawnedBomb.transform.position = newPos;
            }

            if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
            {
                this.onBombPlaced?.Invoke(point);
            }
        }
    }

    public void Enable()
    {
        this.spawnedBomb.gameObject.SetActive(true);
        this.active = true;
    }

    public void Disable()
    {
        this.spawnedBomb.gameObject.SetActive(false);
        this.active = false;
    }

    public void SetPos(Vector3 pos)
    {
        this.spawnedBomb.transform.position = pos;
    }

    public void SetCoords(Vector2Int coords)
    {
        this.SetPos(Ship.GridCoordsToWorldPos(coords));
    }
}
