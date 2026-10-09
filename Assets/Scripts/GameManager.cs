using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace JuegoPlataformas
{
    public sealed class GameManager : MonoBehaviour
    {
        public enum GameState { Playing, Respawning, Won, Lost }

        [SerializeField] PlayerController player;
        [SerializeField] Transform spawnPoint;
        [SerializeField] int startingLives = 3;
        [SerializeField] int requiredPickups = 2;
        [SerializeField] TMP_Text scoreText;
        [SerializeField] TMP_Text livesText;
        [SerializeField] TMP_Text healthText;
        [SerializeField] TMP_Text objectiveText;
        [SerializeField] Slider healthBar;
        [SerializeField] GameObject endPanel;
        [SerializeField] TMP_Text endTitle;
        [SerializeField] TMP_Text endDetails;
        [SerializeField] Button restartButton;

        float messageUntil;

        public GameState State { get; private set; } = GameState.Playing;
        public bool IsPlaying => State == GameState.Playing;
        public int Score { get; private set; }
        public int Lives { get; private set; }
        public int Pickups { get; private set; }
        public bool CanFinish => IsPlaying && Pickups >= requiredPickups;

        void Start()
        {
            Time.timeScale = 1f;
            Lives = startingLives;
            endPanel.SetActive(false);
            RefreshHud();
            ShowObjective();
        }

        void Update()
        {
            if (State == GameState.Won || State == GameState.Lost)
            {
                if (Input.GetKeyDown(KeyCode.R))
                    Restart();

                return;
            }

            if (IsPlaying && messageUntil > 0f && Time.time >= messageUntil)
            {
                messageUntil = 0f;
                ShowObjective();
            }
        }

        public void Collect(int points, string label)
        {
            if (!IsPlaying) return;

            Score += points;
            Pickups++;
            RefreshHud();
            ShowMessage(label + " · +" + points + " puntos");
        }

        public void ShowMessage(string message)
        {
            objectiveText.text = message;
            messageUntil = Time.time + 2.5f;
        }

        void ShowObjective()
        {
            if (Pickups >= requiredPickups)
                objectiveText.text = "Todo recogido. ¡Llega al cofre para ganar!";
            else
                objectiveText.text = "Recoge tarjeta y dinero · " + Pickups + "/" + requiredPickups;
        }

        public void RefreshHud()
        {
            scoreText.text = "PUNTOS " + Score.ToString("D4");
            livesText.text = "VIDAS " + Lives;
            healthText.text = "SALUD " + player.Health;
            healthBar.value = player.Health;
        }

        public void LoseLife()
        {
            if (!IsPlaying) return;

            Lives = Mathf.Max(0, Lives - 1);
            State = GameState.Respawning;
            RefreshHud();
            Invoke(nameof(RespawnOrEnd), 1.1f);
        }

        void RespawnOrEnd()
        {
            if (Lives == 0)
            {
                EndGame(false);
                return;
            }

            player.Respawn(spawnPoint.position);
            State = GameState.Playing;
            ShowMessage("Nueva oportunidad. Los objetos recogidos se conservan.");
        }

        public void Win()
        {
            if (!CanFinish)
            {
                if (IsPlaying)
                    ShowMessage("Recoge la tarjeta y el dinero antes de abrir el cofre.");

                return;
            }

            Score += 500;
            RefreshHud();
            EndGame(true);
        }

        void EndGame(bool won)
        {
            State = won ? GameState.Won : GameState.Lost;
            player.FinishLevel();
            endTitle.text = won ? "¡NIVEL COMPLETADO!" : "FIN DEL JUEGO";
            endTitle.color = won ? new Color(0.35f, 1f, 0.75f) : new Color(1f, 0.4f, 0.4f);
            endDetails.text = "Puntuación: " + Score + "\n" + (won
                ? "Has recuperado los objetos y abierto el cofre."
                : "Te has quedado sin vidas. Vuelve a intentarlo.");
            endPanel.SetActive(true);
            restartButton.Select();
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
