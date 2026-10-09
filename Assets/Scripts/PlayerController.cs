using UnityEngine;

namespace JuegoPlataformas
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D), typeof(SpriteRenderer))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 4.8f;
        [SerializeField] float jumpSpeed = 11.5f;
        [SerializeField] LayerMask groundLayers;
        [SerializeField] float invulnerabilityTime = 1.1f;

        const float gravity = 29.4f;
        const float checkDistance = 0.08f;

        Rigidbody2D body;
        CapsuleCollider2D capsule;
        SpriteRenderer sprite;
        GameManager game;
        float horizontalInput;
        float invulnerableUntil;
        float hurtUntil;
        bool jumpPressed;
        bool controlsLocked;

        public bool IsGrounded { get; private set; }
        public bool IsDead { get; private set; }
        public bool IsHurt => !IsDead && Time.time < hurtUntil;
        public float HorizontalSpeed => body == null ? 0f : body.linearVelocity.x;
        public float VerticalSpeed => body == null ? 0f : body.linearVelocity.y;
        public int Health { get; private set; } = 100;

        void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            capsule = GetComponent<CapsuleCollider2D>();
            sprite = GetComponent<SpriteRenderer>();
            game = FindAnyObjectByType<GameManager>();

            // La gravedad se calcula aquí, en vez de delegarla al Rigidbody2D.
            body.gravityScale = 0f;
        }

        void Update()
        {
            if (controlsLocked || IsDead || game == null || !game.IsPlaying) return;

            horizontalInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetKeyDown(KeyCode.Space)
                || Input.GetKeyDown(KeyCode.W)
                || Input.GetKeyDown(KeyCode.UpArrow))
            {
                jumpPressed = true;
            }

            if (horizontalInput != 0f)
                sprite.flipX = horizontalInput < 0f;

            sprite.color = Time.time < invulnerableUntil
                ? new Color(1f, 0.65f, 0.65f, 0.4f)
                : Color.white;

            if (transform.position.y < -6.5f)
                Die();
        }

        void FixedUpdate()
        {
            if (controlsLocked || IsDead || game == null || !game.IsPlaying)
            {
                jumpPressed = false;
                return;
            }

            IsGrounded = CheckGround();
            bool hitCeiling = CheckCeiling();
            Vector2 velocity = body.linearVelocity;

            if (!IsHurt)
                velocity.x = horizontalInput * moveSpeed;

            if (IsGrounded)
            {
                if (velocity.y < 0f)
                    velocity.y = 0f;

                if (jumpPressed && !IsHurt)
                {
                    velocity.y = jumpSpeed;
                    IsGrounded = false;
                }
            }
            else
            {
                if (hitCeiling && velocity.y > 0f)
                    velocity.y = 0f;

                velocity.y -= gravity * Time.fixedDeltaTime;
            }

            body.linearVelocity = velocity;
            jumpPressed = false;
        }

        bool CheckGround()
        {
            if (body.linearVelocity.y > 0.1f) return false;

            Bounds bounds = capsule.bounds;
            Vector2 size = new Vector2(bounds.size.x * 0.9f, 0.05f);
            Vector2 origin = new Vector2(bounds.center.x, bounds.min.y + size.y * 0.5f);
            RaycastHit2D hit = Physics2D.BoxCast(
                origin, size, 0f, Vector2.down, checkDistance, groundLayers);

            Debug.DrawRay(origin, Vector2.down * checkDistance, Color.green);
            return hit.collider != null && hit.normal.y > 0.5f;
        }

        bool CheckCeiling()
        {
            Bounds bounds = capsule.bounds;
            Vector2 size = new Vector2(bounds.size.x * 0.9f, 0.05f);
            Vector2 origin = new Vector2(bounds.center.x, bounds.max.y - size.y * 0.5f);
            RaycastHit2D[] hits = Physics2D.BoxCastAll(
                origin, size, 0f, Vector2.up, checkDistance, groundLayers);

            Debug.DrawRay(origin, Vector2.up * checkDistance, Color.red);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider == null) continue;

                PlatformEffector2D effector = hit.collider.GetComponent<PlatformEffector2D>();
                if (hit.collider.usedByEffector && effector != null && effector.useOneWay)
                    continue;

                return true;
            }

            return false;
        }

        public void TakeDamage(int amount, Vector2 source)
        {
            if (IsDead || controlsLocked || game == null || !game.IsPlaying
                || Time.time < invulnerableUntil || amount <= 0)
            {
                return;
            }

            Health = Mathf.Max(0, Health - amount);
            game.RefreshHud();

            if (Health == 0)
            {
                Die();
                return;
            }

            invulnerableUntil = Time.time + invulnerabilityTime;
            hurtUntil = Time.time + 0.25f;

            float pushDirection = transform.position.x >= source.x ? 1f : -1f;
            body.linearVelocity = new Vector2(pushDirection * 4f, 5f);
            game.ShowMessage("¡Daño! Aléjate del martillo.");
        }

        public void Die()
        {
            if (IsDead || game == null || !game.IsPlaying) return;

            Health = 0;
            IsDead = true;
            controlsLocked = true;
            body.linearVelocity = Vector2.zero;
            sprite.color = Color.white;
            game.LoseLife();
        }

        public void Respawn(Vector3 position)
        {
            body.position = position;
            body.linearVelocity = Vector2.zero;
            body.gravityScale = 0f;

            controlsLocked = false;
            IsDead = false;
            IsGrounded = false;
            Health = 100;
            horizontalInput = 0f;
            jumpPressed = false;
            hurtUntil = 0f;
            invulnerableUntil = Time.time + invulnerabilityTime;
            sprite.color = Color.white;
            game.RefreshHud();
        }

        public void FinishLevel()
        {
            controlsLocked = true;
            horizontalInput = 0f;
            jumpPressed = false;
            body.linearVelocity = Vector2.zero;
            IsGrounded = true;
            sprite.color = Color.white;
        }
    }
}
