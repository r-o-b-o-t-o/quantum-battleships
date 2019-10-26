using System.Collections.Generic;
using UnityEngine;

public class CellStateController : MonoBehaviourSingleton<CellStateController>
{
    [SerializeField] private List<Projector> projectors;
    [SerializeField] private Material mat;

    private void Start()
    {
        this.projectors.ForEach(p => p.material = new Material(this.mat));
    }

    private void OnDestroy()
    {
        this.projectors.ForEach(p => Destroy(p.material));
    }

    public void DisableAll()
    {
        this.projectors.ForEach(p => p.gameObject.SetActive(false));
    }

    public Projector GetProjector()
    {
        foreach (Projector p in this.projectors)
        {
            if (!p.gameObject.activeInHierarchy)
            {
                p.gameObject.SetActive(true);
                return p;
            }
        }
        return null;
    }
}
