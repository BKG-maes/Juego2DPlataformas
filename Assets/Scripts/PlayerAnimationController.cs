using UnityEngine;

namespace JuegoPlataformas
{
    [RequireComponent(typeof(Animator), typeof(PlayerController))]
    public sealed class PlayerAnimationController : MonoBehaviour
    {
        static readonly int State = Animator.StringToHash("Estado");
        Animator animator;
        PlayerController player;

        void Awake()
        {
            animator = GetComponent<Animator>();
            player = GetComponent<PlayerController>();
        }

        void Update()
        {
            // Estado: 0 quieto, 1 correr, 2 saltar, 3 caer, 4 daño, 5 muerte.
            int state;
            if (player.IsDead)
                state = 5;
            else if (player.IsHurt)
                state = 4;
            else if (!player.IsGrounded && player.VerticalSpeed > 0.15f)
                state = 2;
            else if (!player.IsGrounded)
                state = 3;
            else if (Mathf.Abs(player.HorizontalSpeed) > 0.1f)
                state = 1;
            else
                state = 0;

            animator.SetInteger(State, state);
        }
    }
}
