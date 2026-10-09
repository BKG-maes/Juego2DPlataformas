using UnityEngine;

namespace JuegoPlataformas
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class DamageZone : MonoBehaviour
    {
        [SerializeField] int damage = 25;
        void OnTriggerEnter2D(Collider2D other) => Hurt(other);
        void OnTriggerStay2D(Collider2D other) => Hurt(other);
        void Hurt(Collider2D other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null) player.TakeDamage(damage, transform.position);
        }
    }
}
