using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShipPlacement : MonoBehaviourSingleton<ShipPlacement>
{
    public Action<Ship> onShipAdded;

    [SerializeField] private Camera cam;
    [SerializeField] private MeshFilter ocean;
    [SerializeField] private List<Button> availableShipsButtons;
    [SerializeField] private Button doneButton;
    [SerializeField] private Color colorRed;
    [SerializeField] private Color colorGreen;

    private Ship spawnedShip;
    private Vector2Int mousePos;

    private void Start()
    {
        this.doneButton.onClick.AddListener(this.OnFinish);
        this.DisableFinishUI();
    }

    public void SpawnShip(int type)
    {
        this.CancelShipPlacement();
        Ship prefab = Rules.Instance.shipTypes[type];
        this.spawnedShip = GameObject.Instantiate(prefab);
        this.spawnedShip.ocean = this.ocean;
        this.mousePos = Vector2Int.zero; // Set the mouse position to an arbitrary value so that a value change is detected and the new ship gets positioned
    }

    public void ResetAvailableShips()
    {
        this.availableShipsButtons.ForEach(btn => btn.interactable = true);
        this.DisableFinishUI();
    }

    public void PlaceRandomly()
    {
        foreach (Ship ship in GameController.Instance.GetActivePlayerShips())
        {
            GameController.Instance.RemoveShipFromActivePlayer(ship);
            GameObject.Destroy(ship.gameObject);
        }

        for (int i = 0; i < this.availableShipsButtons.Count; ++i)
        {
            while (true)
            {
                this.SpawnShip(i);
                this.spawnedShip.transform.position = new Vector3(UnityEngine.Random.Range(-90.0f, 90.0f), this.spawnedShip.transform.position.y, UnityEngine.Random.Range(-90.0f, 90.0f));
                this.spawnedShip.Rotate(UnityEngine.Random.Range(0, 4) * 90.0f);
                this.spawnedShip.SetupSnap();
                if (!this.CanPlaceShip(this.spawnedShip))
                {
                    GameObject.Destroy(this.spawnedShip.gameObject);
                    continue;
                }
                this.onShipAdded?.Invoke(this.spawnedShip);
                break;
            }
            this.spawnedShip = null;
            this.availableShipsButtons[i].interactable = false;
        }

        this.EnableFinishUI();
        SoundController.Instance.PlayRandomSound(SoundController.Instance.splashSounds);
    }

    private void Update()
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = this.cam.ScreenPointToRay(Input.mousePosition);
        Vector3 mouseWorldPos;
        float dist;
        if (plane.Raycast(ray, out dist))
        {
            mouseWorldPos = ray.GetPoint(dist);
            Vector2Int point = Ship.WorldPosToGridCoords(mouseWorldPos);
            if (point != this.mousePos)
            {
                this.mousePos = point;
                if (this.spawnedShip != null)
                {
                    this.spawnedShip.transform.position = mouseWorldPos;
                    this.spawnedShip.SetupSnap();
                    this.SetPlacementBlockProjectors();
                }
            }
        }

        if (this.spawnedShip != null)
        {
            this.HandleShipPlacementRotation();

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    this.TryPlaceShip();
                }
                if (Input.GetMouseButtonDown(1) || Input.GetKeyUp(KeyCode.Delete))
                {
                    this.CancelShipPlacement();
                }
            }
        }
        else
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    foreach (Ship ship in GameController.Instance.GetActivePlayerShips())
                    {
                        if (ship.OverlapsWith(this.mousePos))
                        {
                            this.spawnedShip = ship; // Grab the ship to move it
                            this.SetPlacementBlockProjectors();
                            break;
                        }
                    }
                }

                if (Input.GetMouseButtonDown(1) || Input.GetKeyUp(KeyCode.Delete))
                {
                    foreach (Ship ship in GameController.Instance.GetActivePlayerShips())
                    {
                        if (ship.OverlapsWith(this.mousePos))
                        {
                            this.UnregisterShip(ship);
                            break;
                        }
                    }
                }
            }
        }
    }

    private Nullable<Vector2Int> GetHoveredGridPos()
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = this.cam.ScreenPointToRay(Input.mousePosition);
        float dist;
        if (plane.Raycast(ray, out dist))
        {
            Vector3 worldPos = ray.GetPoint(dist);
            return Ship.WorldPosToGridCoords(worldPos);
        }
        return null;
    }

    private void CancelShipPlacement()
    {
        if (this.spawnedShip != null)
        {
            if (GameController.Instance.GetActivePlayerShips().Contains(this.spawnedShip))
            {
                this.UnregisterShip(this.spawnedShip);
            }
            else
            {
                GameObject.Destroy(this.spawnedShip.gameObject);
            }
            this.spawnedShip = null;
            this.DisablePlacementBlockProjectors();
        }
    }

    private void UnregisterShip(Ship ship)
    {
        GameController.Instance.RemoveShipFromActivePlayer(ship);
        GameObject.Destroy(ship.gameObject);
        int typeIdx = Rules.Instance.shipTypes.FindIndex(prefab => prefab.identifier == ship.identifier);
        this.availableShipsButtons[typeIdx].interactable = true;
        this.DisableFinishUI();
    }

    private void HandleShipPlacementRotation()
    {
        bool rotated = false;

        if (Input.GetKeyUp(KeyCode.R))
        {
            this.spawnedShip.Rotate((Input.GetKey(KeyCode.LeftShift) ? -1.0f : 1.0f) * 90.0f);
            rotated = true;
        }

        if (Mathf.Abs(Input.mouseScrollDelta.y) > 0.01f)
        {
            this.spawnedShip.Rotate((Input.mouseScrollDelta.y > 0.0f ? -1.0f : 1.0f) * 90.0f);
            rotated = true;
        }

        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            this.spawnedShip.SetRotation(0.0f);
            rotated = true;
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            this.spawnedShip.SetRotation(-90.0f);
            rotated = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            this.spawnedShip.SetRotation(180.0f);
            rotated = true;
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            this.spawnedShip.SetRotation(90.0f);
            rotated = true;
        }

        if (rotated)
        {
            this.SetPlacementBlockProjectors();
        }
    }

    private void TryPlaceShip()
    {
        if (!this.CanPlaceShip(this.spawnedShip))
        {
            SoundController.Instance.PlaySound(SoundController.Instance.errorSound);
            return;
        }

        SoundController.Instance.PlayRandomSound(SoundController.Instance.splashSounds);
        this.onShipAdded?.Invoke(this.spawnedShip);
        int typeIdx = Rules.Instance.shipTypes.FindIndex(ship => ship.identifier == this.spawnedShip.identifier);
        this.availableShipsButtons[typeIdx].interactable = false;
        this.spawnedShip = null;
        this.DisablePlacementBlockProjectors();

        // Check if all ships have been placed
        if (GameController.Instance.GetActivePlayerShips().Length >= Rules.Instance.shipTypes.Count)
        {
            this.EnableFinishUI();
        }
    }

    private void EnableFinishUI()
    {
        this.doneButton.gameObject.SetActive(true);
        this.availableShipsButtons.ForEach(btn => btn.gameObject.SetActive(false));
    }

    private void DisableFinishUI()
    {
        this.doneButton.gameObject.SetActive(false);
        this.availableShipsButtons.ForEach(btn => btn.gameObject.SetActive(true));
    }

    private bool CanPlaceShip(Ship ship)
    {
        Vector2Int[] blocks = ship.GetBlockCoords();
        return blocks.All(b => this.CanPlaceBlockAtCoords(b));
    }

    private bool CanPlaceBlockAtCoords(Vector2Int coords)
    {
        if (coords.x >= 10 || coords.x < 0 ||
            coords.y >= 10 || coords.y < 0)
        {
            return false;
        }

        Ship[] ships = GameController.Instance.GetActivePlayerShips();
        if (ships.Where(s => s != this.spawnedShip).Any(s => s.OverlapsWith(coords)))
        {
            return false;
        }

        return true;
    }

    private void SetPlacementBlockProjectors()
    {
        this.DisablePlacementBlockProjectors();

        Vector2Int[] blocks = this.spawnedShip.GetBlockCoords();
        foreach (Vector2Int block in blocks)
        {
            Projector p = CellStateController.Instance.GetProjector();
            Vector3 pos = Ship.GridCoordsToWorldPos(block);
            pos.y = p.transform.position.y;
            p.transform.position = pos;

            p.material.color = this.CanPlaceBlockAtCoords(block) ? this.colorGreen : this.colorRed;
        }
    }

    private void DisablePlacementBlockProjectors()
    {
        CellStateController.Instance.DisableAll();
    }

    private void OnFinish()
    {
        GameController.Instance.NextPlayer();
    }
}
