using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class GameController : Singleton<GameController>
{
    [System.Serializable]
    private class Player
    {
        public CrossController crossController;
        [SerializeField] private GameObject shipsContainer;

        private List<Ship> ships;

        public Player()
        {
            this.ships = new List<Ship>();
        }

        public Ship[] GetShips()
        {
            return this.ships.ToArray();
        }

        public bool HasShip(Ship ship)
        {
            return this.ships.Contains(ship);
        }

        public void AddShip(Ship ship)
        {
            if (!this.HasShip(ship))
            {
                this.ships.Add(ship);
                ship.transform.parent = this.shipsContainer.transform;
            }
        }

        public void RemoveShip(Ship ship)
        {
            this.ships.Remove(ship);
        }

        public void SetShipsState(bool shown)
        {
            this.shipsContainer.SetActive(shown);
        }

        public void SetCrossesState(bool shown)
        {
            this.crossController.gameObject.SetActive(shown);
        }

        public bool HasShipAtCoords(Vector2Int coords)
        {
            return this.ships.Any(ship => ship.OverlapsWith(coords));
        }
    }

    private int activePlayer;

    [SerializeField] private UI.TemplatedText placeShipPrompt;
    [SerializeField] private UI.TemplatedText playerTurnText;
    [SerializeField] private UI.TemplatedText winText;
    [SerializeField] private UI.ToggleButton toggleGridViewButton;
    [SerializeField] private TextMeshProUGUI hitText;
    [SerializeField] private Button startRoundButton;
    [SerializeField] private Button backToMainMenuButton;
    [SerializeField] private Player[] players;
    private bool gameStarted;

    private void Start()
    {
        ShipPlacement.Instance.onShipAdded += this.OnShipAdded;
        BombPlacement.Instance.onBombPlaced += this.OnBombAdded;
        Engine.EngineManager.Instance.onWin += this.OnWin;
        Engine.EngineManager.Instance.onShipSunk += this.OnShipSunk;
        this.startRoundButton.onClick.AddListener(this.OnStartRoundClicked);
        this.backToMainMenuButton.onClick.AddListener(this.OnBackToMainMenuClicked);
        this.toggleGridViewButton.onStateChanged += this.OnGridViewClicked;
        this.gameStarted = false;
        this.SetActivePlayer(0);
    }

    public Ship[] GetActivePlayerShips()
    {
        return this.players[this.activePlayer].GetShips();
    }

    public void RemoveShipFromActivePlayer(Ship ship)
    {
        this.players[this.activePlayer].RemoveShip(ship);
    }

    private void OnShipAdded(Ship ship)
    {
        Player p = this.players[this.activePlayer];
        if (p.HasShip(ship))
        {
            return;
        }

        p.AddShip(ship);

        Engine.Ship engineShip = new Engine.Ship();
        foreach (Vector2Int coords in ship.GetBlockCoords())
        {
            engineShip.blocks.Add(new Engine.Ship.Block(coords));
        }
        engineShip.health = ship.health;
        Engine.EngineManager.Instance.gameState.players[this.activePlayer].ships.Add(engineShip);
    }

    public void NextPlayer()
    {
        this.SetActivePlayer((this.activePlayer + 1) % 2);
        if (!this.gameStarted && this.activePlayer == 0 && this.GetActivePlayerShips().Length > 0)
        {
            ShipPlacement.Instance.gameObject.SetActive(false);
            this.PrepareForRoundStart();
            this.gameStarted = true;
        }
    }

    public int GetActivePlayer()
    {
        return this.activePlayer;
    }

    private void SetActivePlayer(int idx)
    {
        this.activePlayer = idx;
        this.UpdatePlaceShipPrompt();
        ShipPlacement.Instance.ResetAvailableShips();
        foreach (Player p in this.players)
        {
            p.SetShipsState(false);
        }
        this.players[this.activePlayer].SetShipsState(true);
    }

    private void UpdatePlaceShipPrompt()
    {
        this.placeShipPrompt.SetArguments(this.activePlayer + 1);
    }

    private void PrepareForRoundStart()
    {
        this.startRoundButton.gameObject.SetActive(true);
        this.playerTurnText.gameObject.SetActive(true);
        this.playerTurnText.SetArguments(this.activePlayer + 1);
        this.players[this.activePlayer].SetShipsState(false);
    }

    private void OnStartRoundClicked()
    {
        this.startRoundButton.gameObject.SetActive(false);
        this.toggleGridViewButton.gameObject.SetActive(true);
        BombPlacement.Instance.Enable();
        this.players[this.activePlayer].SetCrossesState(true);
    }

    private void OnGridViewClicked(bool state)
    {
        this.players[this.activePlayer].SetShipsState(state);
        this.players[this.activePlayer].SetCrossesState(!state);
        this.players[(this.activePlayer + 1) % 2].SetCrossesState(state);

        if (!state)
        {
            BombPlacement.Instance.Enable();
        }
        else
        {
            BombPlacement.Instance.Disable();
        }
    }

    private void OnBombAdded(Vector2Int coords)
    {
        Engine.EngineManager.Instance.gameState.players[this.activePlayer].bombs.Add((Engine.Coords)coords);
        BombPlacement.Instance.Disable();
        this.toggleGridViewButton.gameObject.SetActive(false);
        this.StartCoroutine(this.BombPlacedAnimation(coords));
    }

    private IEnumerator BombPlacedAnimation(Vector2Int coords)
    {
        if (this.players[(this.activePlayer + 1) % 2].HasShipAtCoords(coords))
        {
            SoundController.Instance.PlaySound(SoundController.Instance.explosionSound);
            this.players[this.activePlayer].crossController.Hit(coords);
            this.hitText.text = "Hit!";
        }
        else
        {
            SoundController.Instance.PlayRandomSound(SoundController.Instance.splashSounds);
            this.players[this.activePlayer].crossController.Miss(coords);
            this.hitText.text = "Missed!";
        }
        this.hitText.gameObject.SetActive(true);

        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        yield return Engine.EngineManager.Instance.UpdateGameStateCoroutine();
        watch.Stop();
        float timeToWait = 1.0f - (float)watch.Elapsed.TotalSeconds;
        if (timeToWait > 0.0f)
        {
            yield return new WaitForSecondsRealtime(timeToWait);
        }

        this.hitText.gameObject.SetActive(false);
        this.players[this.activePlayer].SetCrossesState(false);
        if (Engine.EngineManager.Instance.gameState.winner == 0)
        {
            this.NextPlayer();
            this.PrepareForRoundStart();
        }
    }

    private void OnWin(int winner)
    {
        this.winText.gameObject.SetActive(true);
        this.winText.SetArguments(winner + 1);
        this.backToMainMenuButton.gameObject.SetActive(true);
        this.startRoundButton.gameObject.SetActive(false);
    }

    private void OnBackToMainMenuClicked()
    {
        SceneManager.LoadSceneAsync("MenuScene", LoadSceneMode.Single);
    }

    private void OnShipSunk()
    {
        this.hitText.text += "\nSunk!";
    }
}
