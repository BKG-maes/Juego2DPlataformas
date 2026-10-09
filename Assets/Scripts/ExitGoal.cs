using UnityEngine;

namespace JuegoPlataformas
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class ExitGoal : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D other) => TryFinish(other);
        void OnTriggerStay2D(Collider2D other) => TryFinish(other);
        void TryFinish(Collider2D other)
        {
            if (other.GetComponent<PlayerController>() == null) return;
            var game = FindAnyObjectByType<GameManager>();
            if (game != null) game.Win();
        }
    }
}
