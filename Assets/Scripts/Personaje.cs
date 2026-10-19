using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class Personaje : MonoBehaviour
{
    public Partida partida;
    public LayerMask capaSuelo;
    public float velocidad = 5f;
    public float fuerzaSalto = 6f;

    private const float gravedad = 23.62f;
    private Rigidbody2D cuerpo;
    private Animator animador;
    private SpriteRenderer sprite;
    private CapsuleCollider2D capsula;
    private float movimientoHorizontal;
    private bool saltoSolicitado;

    private void Awake()
    {
        cuerpo = GetComponent<Rigidbody2D>();
        animador = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        capsula = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        if (partida.terminada) return;

        movimientoHorizontal = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space)) saltoSolicitado = true;

        animador.SetFloat("Speed", Mathf.Abs(movimientoHorizontal));
        if (movimientoHorizontal > 0f) sprite.flipX = false;
        else if (movimientoHorizontal < 0f) sprite.flipX = true;
    }

    private void FixedUpdate()
    {
        if (partida.terminada) return;

        Vector2 velocidadActual = cuerpo.linearVelocity;
        float tiempoFisico = Time.fixedDeltaTime;
        bool enSuelo = velocidadActual.y <= 0.1f && ComprobarRayos(true, 0.04f);
        float distanciaTecho = velocidadActual.y > 0f ? velocidadActual.y * tiempoFisico + 0.02f : 0.04f;
        bool techo = ComprobarRayos(false, distanciaTecho);
        animador.SetBool("Grounded", enSuelo);
        velocidadActual.x = movimientoHorizontal * velocidad;

        if (enSuelo)
        {
            velocidadActual.y = 0f;
            if (saltoSolicitado && !techo)
            {
                velocidadActual.y = fuerzaSalto;
                animador.SetBool("Grounded", false);
                animador.SetTrigger("Jump");
            }
        }
        else
        {
            if (velocidadActual.y > 0f && techo) velocidadActual.y = 0f;
            velocidadActual.y -= gravedad * tiempoFisico;
        }

        saltoSolicitado = false;
        cuerpo.linearVelocity = velocidadActual;
    }

    private bool ComprobarRayos(bool haciaAbajo, float distancia)
    {
        Bounds limites = capsula.bounds;
        Vector2 direccion = haciaAbajo ? Vector2.down : Vector2.up;
        float y = haciaAbajo ? limites.min.y + 0.01f : limites.max.y - 0.01f;
        float margen = 0.05f;
        Vector2 origenIzquierdo = new Vector2(limites.min.x + margen, y);
        Vector2 origenCentro = new Vector2(limites.center.x, y);
        Vector2 origenDerecho = new Vector2(limites.max.x - margen, y);
        Debug.DrawRay(origenIzquierdo, direccion * distancia, Color.yellow);
        Debug.DrawRay(origenCentro, direccion * distancia, Color.yellow);
        Debug.DrawRay(origenDerecho, direccion * distancia, Color.yellow);
        RaycastHit2D izquierda = Physics2D.Raycast(origenIzquierdo, direccion, distancia, capaSuelo);
        RaycastHit2D centro = Physics2D.Raycast(origenCentro, direccion, distancia, capaSuelo);
        RaycastHit2D derecha = Physics2D.Raycast(origenDerecho, direccion, distancia, capaSuelo);
        return izquierda.collider != null || centro.collider != null || derecha.collider != null;
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (partida.terminada) return;

        if (otro.CompareTag("Dinero"))
        {
            Recoger(otro, partida.puntosDinero, false);
        }
        else if (otro.CompareTag("Cartas"))
        {
            Recoger(otro, partida.puntosCartas, true);
        }
        else if (otro.CompareTag("Cofres"))
        {
            Recoger(otro, partida.puntosCofres, false);
        }
        else if (otro.CompareTag("Prensas"))
        {
            partida.RecibirDanio();
            if (partida.terminada)
            {
                movimientoHorizontal = 0f;
                saltoSolicitado = false;
                cuerpo.linearVelocity = Vector2.zero;
                animador.ResetTrigger("Jump");
                animador.ResetTrigger("Hurt");
                animador.SetTrigger("Death");
                cuerpo.simulated = false;
            }
            else
            {
                animador.SetTrigger("Hurt");
            }
        }
        else if (otro.CompareTag("Salida"))
        {
            partida.GanarPartida();
        }
    }

    private void Recoger(Collider2D objeto, int puntos, bool esTarjeta)
    {
        if (!objeto.enabled) return;
        objeto.enabled = false;
        if (esTarjeta) partida.RecogerTarjeta();
        partida.SumarPuntos(puntos);
        Destroy(objeto.gameObject);
    }
}
