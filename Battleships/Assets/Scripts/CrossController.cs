using UnityEngine;
using System.Collections.Generic;

public class CrossController : MonoBehaviour
{
    [SerializeField] private Projector hitPrefab;
    [SerializeField] private Projector missPrefab;

    private List<Projector> hits;
    private List<Projector> misses;

    private void Start()
    {
        this.hits = new List<Projector>();
        this.misses = new List<Projector>();
    }

    public void Hit(Vector2Int coords)
    {
        this.Instantiate(this.hitPrefab, this.hits, coords);
    }

    public void Miss(Vector2Int coords)
    {
        this.Instantiate(this.missPrefab, this.misses, coords);
    }

    private void Instantiate(Projector prefab, List<Projector> list, Vector2Int coords)
    {
        Projector instance = GameObject.Instantiate(prefab);
        instance.name = prefab.name;
        instance.transform.parent = this.transform;

        Vector3 pos = Ship.GridCoordsToWorldPos(coords);
        pos.y = instance.transform.position.y;
        instance.transform.position = pos;

        list.Add(instance);
    }
}
