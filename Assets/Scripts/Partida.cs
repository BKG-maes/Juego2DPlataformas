using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Partida : MonoBehaviour
{
    public Slider barraVida;
    public TMP_Text textoPuntuacion;
    public TMP_Text textoVidas;
    public TMP_Text textoFin;
    public TMP_Text textoTarjetas;
    public GameObject panelFin;
    public Collider2D salidaTrigger;

    public int vidasMaximas = 3;
    public int puntosDinero = 10;
    public int puntosCartas = 25;
    public int puntosCofres = 50;
    public int tarjetasNecesarias = 3;

    public int vidasActuales;
    public int puntuacion;
    public int tarjetasRecogidas;
    public bool terminada;

    private void Start()
    {
        // Inicializa la partida y prepara la interfaz existente.
        vidasMaximas = Mathf.Max(1, vidasMaximas);
        tarjetasNecesarias = Mathf.Max(1, tarjetasNecesarias);
        vidasActuales = vidasMaximas;
        puntuacion = 0;
        tarjetasRecogidas = 0;
        terminada = false;

        if (barraVida != null)
        {
            barraVida.minValue = 0f;
            barraVida.maxValue = vidasMaximas;
            barraVida.value = vidasActuales;
        }

        if (textoFin != null)
        {
            textoFin.gameObject.SetActive(false);
        }

        if (panelFin != null)
        {
            panelFin.SetActive(false);
        }

        if (salidaTrigger != null)
        {
            salidaTrigger.enabled = false;
        }

        ActualizarHUD();
    }

    private void Update()
    {
        // La tecla R solo reinicia después de terminar la partida.
        if (terminada && Input.GetKeyDown(KeyCode.R))
        {
            Reiniciar();
        }
    }

    public void SumarPuntos(int cantidad)
    {
        if (terminada)
        {
            return;
        }

        puntuacion += cantidad;
        ActualizarHUD();
    }

    public void RecogerTarjeta()
    {
        if (terminada)
        {
            return;
        }

        tarjetasRecogidas = Mathf.Min(tarjetasRecogidas + 1, tarjetasNecesarias);
        if (tarjetasRecogidas >= tarjetasNecesarias && salidaTrigger != null)
        {
            salidaTrigger.enabled = true;
        }

        ActualizarHUD();
    }

    public bool GanarPartida()
    {
        if (terminada || tarjetasRecogidas < tarjetasNecesarias)
        {
            return false;
        }

        terminada = true;
        if (AudioManager.Instance != null) AudioManager.Instance.ReproducirVictoria();
        if (textoFin != null)
        {
            textoFin.text = $"HAS ESCAPADO\nSCORE:{puntuacion:0000}\nPulsa R para reiniciar";
            textoFin.gameObject.SetActive(true);
        }

        if (panelFin != null)
        {
            panelFin.SetActive(true);
        }

        return true;
    }

    public void RecibirDanio()
    {
        if (terminada)
        {
            return;
        }

        vidasActuales = Mathf.Max(0, vidasActuales - 1);
        ActualizarHUD();

        if (vidasActuales == 0)
        {
            TerminarPartida();
        }
    }

    public void Reiniciar()
    {
        // Carga de nuevo la escena actual.
        if (!terminada)
        {
            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ActualizarHUD()
    {
        if (textoPuntuacion != null)
        {
            textoPuntuacion.text = $"SCORE:{puntuacion:0000}";
        }

        if (textoVidas != null)
        {
            textoVidas.text = $"VIDAS: {vidasActuales}/{vidasMaximas}";
        }

        if (textoTarjetas != null)
        {
            textoTarjetas.text = $"TARJETAS: {tarjetasRecogidas}/{tarjetasNecesarias}";
        }

        if (barraVida != null)
        {
            barraVida.value = vidasActuales;
        }
    }

    private void TerminarPartida()
    {
        // Muestra el fin de juego; Personaje detiene su física al recibir la última vida.
        terminada = true;
        if (AudioManager.Instance != null) AudioManager.Instance.ReproducirDerrota();

        if (textoFin != null)
        {
            textoFin.text = $"FIN DEL JUEGO\nSCORE:{puntuacion:0000}\nPulsa R para reiniciar";
            textoFin.gameObject.SetActive(true);
        }

        if (panelFin != null)
        {
            panelFin.SetActive(true);
        }
    }
}
