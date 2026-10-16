using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class Personaje : MonoBehaviour
{
    public Partida partida;
    public float velocidad = 5f;
    public float fuerzaSalto = 9f;
    public float duracionInvulnerabilidad = 1.2f;
    public float duracionBloqueoGolpe = 0.25f;
    public float duracionTinteGolpe = 0.25f;

    private Rigidbody2D cuerpo;
    private Animator animador;
    private SpriteRenderer sprite;
    private Color colorOriginal;
    private float movimientoHorizontal;
    private float invulnerabilidadRestante;
    private float bloqueoGolpeRestante;
    private float tinteGolpeRestante;
    private bool enSuelo;
    private bool saltoSolicitado;
    private bool muerto;

    private void Awake()
    {
        // Guarda los componentes necesarios para el movimiento y las animaciones.
        cuerpo = GetComponent<Rigidbody2D>();
        animador = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        colorOriginal = sprite.color;
        cuerpo.freezeRotation = true;
    }

    private void Update()
    {
        // Cuenta los temporizadores del golpe y restaura el color al terminar.
        ActualizarTemporizadores();

        if (muerto || (partida != null && partida.terminada))
        {
            movimientoHorizontal = 0f;
            saltoSolicitado = false;
            return;
        }

        // Lee el eje Horizontal y guarda el salto para aplicarlo en FixedUpdate.
        if (bloqueoGolpeRestante > 0f)
        {
            movimientoHorizontal = 0f;
            saltoSolicitado = false;
        }
        else
        {
            movimientoHorizontal = Input.GetAxisRaw("Horizontal");

            if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
            {
                saltoSolicitado = true;
            }
        }

        // Actualiza la animación de movimiento y mira hacia el sentido elegido.
        animador.SetFloat("Speed", Mathf.Abs(movimientoHorizontal));
        if (movimientoHorizontal > 0f)
        {
            sprite.flipX = false;
        }
        else if (movimientoHorizontal < 0f)
        {
            sprite.flipX = true;
        }

    }

    private void FixedUpdate()
    {
        if (muerto || (partida != null && partida.terminada))
        {
            cuerpo.linearVelocity = Vector2.zero;
            return;
        }

        if (bloqueoGolpeRestante > 0f)
        {
            movimientoHorizontal = 0f;
            saltoSolicitado = false;
        }

        // Cambia la velocidad horizontal sin alterar la velocidad vertical.
        Vector2 velocidadActual = cuerpo.linearVelocity;
        velocidadActual.x = movimientoHorizontal * velocidad;

        if (saltoSolicitado && enSuelo)
        {
            velocidadActual.y = fuerzaSalto;
            CambiarEstadoSuelo(false);
            animador.SetTrigger("Jump");
        }

        saltoSolicitado = false;
        cuerpo.linearVelocity = velocidadActual;

        // El siguiente callback físico vuelve a confirmar si sigue apoyado.
        enSuelo = false;
        animador.SetBool("Grounded", false);
    }

    private void OnCollisionEnter2D(Collision2D colision)
    {
        ComprobarSuelo(colision);
    }

    private void OnCollisionStay2D(Collision2D colision)
    {
        ComprobarSuelo(colision);
    }

    private void OnCollisionExit2D(Collision2D colision)
    {
        if (muerto || (partida != null && partida.terminada))
        {
            return;
        }

        if (colision.gameObject.CompareTag("Suelo"))
        {
            CambiarEstadoSuelo(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (muerto || (partida != null && partida.terminada))
        {
            return;
        }

        // Los objetos recogibles se identifican por su tag y se puntúan una vez.
        if (otro.CompareTag("Dinero"))
        {
            Recoger(otro, partida != null ? partida.puntosDinero : 0);
        }
        else if (otro.CompareTag("Cartas"))
        {
            Recoger(otro, partida != null ? partida.puntosCartas : 0, true);
        }
        else if (otro.CompareTag("Cofres"))
        {
            Recoger(otro, partida != null ? partida.puntosCofres : 0);
        }
        else if (otro.CompareTag("Prensas"))
        {
            RecibirGolpe();
        }
        else if (otro.CompareTag("Salida"))
        {
            IntentarVictoria();
        }
    }

    private void OnTriggerStay2D(Collider2D otro)
    {
        // Si permanece sobre una prensa, puede volver a recibir daño al acabar la inmunidad.
        if (otro.CompareTag("Prensas"))
        {
            RecibirGolpe();
        }
        else if (otro.CompareTag("Salida"))
        {
            IntentarVictoria();
        }
    }

    private void ComprobarSuelo(Collision2D colision)
    {
        if (!colision.gameObject.CompareTag("Suelo"))
        {
            return;
        }

        // Solo cuenta como suelo un contacto cuya normal apunta hacia arriba.
        foreach (ContactPoint2D contacto in colision.contacts)
        {
            if (contacto.normal.y > 0.5f && cuerpo.linearVelocity.y <= 0.1f)
            {
                CambiarEstadoSuelo(true);
                return;
            }
        }
    }

    private void CambiarEstadoSuelo(bool estado)
    {
        enSuelo = estado;
        animador.SetBool("Grounded", enSuelo);
    }

    private void Recoger(Collider2D objeto, int puntos, bool esTarjeta = false)
    {
        if (partida == null || partida.terminada || !objeto.enabled)
        {
            return;
        }

        // Desactiva el trigger antes de destruirlo para evitar puntuación duplicada.
        objeto.enabled = false;
        if (esTarjeta)
        {
            partida.RecogerTarjeta();
        }

        partida.SumarPuntos(puntos);
        Destroy(objeto.gameObject);
    }

    private void IntentarVictoria()
    {
        if (partida != null && partida.GanarPartida())
        {
            // Detiene la física y deja al personaje en reposo, sin activar Death.
            movimientoHorizontal = 0f;
            saltoSolicitado = false;
            cuerpo.linearVelocity = Vector2.zero;
            animador.ResetTrigger("Jump");
            animador.ResetTrigger("Hurt");
            animador.SetFloat("Speed", 0f);
            CambiarEstadoSuelo(true);
            cuerpo.simulated = false;
        }
    }

    private void RecibirGolpe()
    {
        if (partida == null || partida.terminada || muerto || invulnerabilidadRestante > 0f)
        {
            return;
        }

        partida.RecibirDanio();
        if (partida.terminada)
        {
            Morir();
            return;
        }

        AplicarEfectoGolpe();
    }

    private void AplicarEfectoGolpe()
    {
        // Limita el daño repetido, bloquea brevemente el movimiento y muestra el golpe.
        invulnerabilidadRestante = duracionInvulnerabilidad;
        bloqueoGolpeRestante = duracionBloqueoGolpe;
        tinteGolpeRestante = duracionTinteGolpe;
        sprite.color = Color.red;
        animador.SetTrigger("Hurt");
    }

    private void ActualizarTemporizadores()
    {
        if (invulnerabilidadRestante > 0f)
        {
            invulnerabilidadRestante -= Time.deltaTime;
        }

        if (bloqueoGolpeRestante > 0f)
        {
            bloqueoGolpeRestante -= Time.deltaTime;
        }

        if (tinteGolpeRestante > 0f)
        {
            tinteGolpeRestante -= Time.deltaTime;
            if (tinteGolpeRestante <= 0f)
            {
                sprite.color = colorOriginal;
            }
        }
    }

    private void Morir()
    {
        if (muerto)
        {
            return;
        }

        // Detiene el movimiento y deja que el Animator reproduzca la muerte.
        muerto = true;
        movimientoHorizontal = 0f;
        saltoSolicitado = false;
        cuerpo.linearVelocity = Vector2.zero;
        sprite.color = colorOriginal;
        tinteGolpeRestante = 0f;
        animador.ResetTrigger("Jump");
        animador.ResetTrigger("Hurt");
        animador.SetTrigger("Death");
        cuerpo.simulated = false;
    }
}
