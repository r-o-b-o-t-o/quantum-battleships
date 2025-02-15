﻿using System.IO;
using System.Diagnostics;
using System.Collections;
using UnityEngine;

namespace Engine
{
    public class EngineManager : Singleton<EngineManager>
    {
        public uint shots = 2048;
        public GameState gameState;
        public System.Action<int> onWin;
        public System.Action onShipSunk;

        private Process process;
        private bool runningUpdate;

        private void Awake()
        {
            ProcessStartInfo info = new ProcessStartInfo(Path.Combine(Directory.GetCurrentDirectory(), "battleships_engine.exe"));
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            this.process = Process.Start(info);
            this.process.OutputDataReceived += this.OnData;
            this.process.BeginOutputReadLine();
            this.process.ErrorDataReceived += this.OnError;
            this.process.BeginErrorReadLine();
        }

        private void Start()
        {
            this.GetBlankGameState();
            this.gameState.players = new Player[] { new Player(), new Player() };
        }

        private void OnData(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
            {
                return;
            }

            try
            {
                int sunkShipsBeforeUpdate = this.gameState.players[(GameController.Instance.GetActivePlayer() + 1) % 2].CountSunkShips();
                JsonUtility.FromJsonOverwrite(e.Data, this.gameState);
                if (this.gameState.players[(GameController.Instance.GetActivePlayer() + 1) % 2].CountSunkShips() > sunkShipsBeforeUpdate)
                {
                    MainThreadDispatcher.Instance.RunOnMainThread(() =>
                    {
                        this.onShipSunk?.Invoke();
                    });
                }
                this.runningUpdate = false;
                if (this.gameState.winner > 0)
                {
                    MainThreadDispatcher.Instance.RunOnMainThread(() =>
                    {
                        this.onWin?.Invoke(this.gameState.winner - 1);
                    });
                }
            }
            catch (System.Exception) { }
        }

        private void OnError(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
            {
                return;
            }
            UnityEngine.Debug.LogError(e.Data);
        }

        private void GetBlankGameState()
        {
            this.SendCommand("blankGameState");
        }

        public void UpdateGameState()
        {
            this.StartCoroutine(this.UpdateGameStateCoroutine());
        }

        public IEnumerator UpdateGameStateCoroutine()
        {
            this.runningUpdate = true;
            this.SendCommand("updateGameState");
            var query = new Engine.Query(this.gameState, this.shots);
            this.SendCommand(JsonUtility.ToJson(query, false));

            yield return new WaitWhile(() => this.runningUpdate);
        }

        private void OnApplicationQuit()
        {
            this.SendCommand("quit");
        }

        private void SendCommand(string command)
        {
            this.process.StandardInput.WriteLine(command);
        }

        private void OnDestroy()
        {
            if (this.process != null && !this.process.HasExited)
            {
                this.process.Kill();
            }
        }
    }
}
