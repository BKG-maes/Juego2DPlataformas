using UnityEngine;

namespace JuegoPlataformas
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class Collectible : MonoBehaviour
    {
        [SerializeField] int points = 100;
        [SerializeField] string label = "Objeto recogido";

        bool collected;

        void OnTriggerEnter2D(Collider2D other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (collected || player == null || player.IsDead) return;

            GameManager game = FindAnyObjectByType<GameManager>();
            if (game == null || !game.IsPlaying) return;

            collected = true;
            game.Collect(points, label);
            Destroy(gameObject);
        }
    }
}
