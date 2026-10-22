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
    public static int puntuacionFinal;
    public int tarjetasRecogidas;
    public bool terminada;

    private void Start()
    {
        vidasActuales = vidasMaximas;
        puntuacion = 0;
        puntuacionFinal = 0;
        tarjetasRecogidas = 0;
        terminada = false;

        barraVida.minValue = 0f;
        barraVida.maxValue = vidasMaximas;
        textoFin.gameObject.SetActive(false);
        panelFin.SetActive(false);
        salidaTrigger.enabled = false;
        ActualizarHUD();
    }

    private void Update()
    {
        if (terminada && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void SumarPuntos(int cantidad)
    {
        if (terminada) return;
        puntuacion += cantidad;
        ActualizarHUD();
    }

    public void RecogerTarjeta()
    {
        if (terminada) return;

        tarjetasRecogidas++;
        if (tarjetasRecogidas >= tarjetasNecesarias) salidaTrigger.enabled = true;
        ActualizarHUD();
    }

    public void GanarPartida()
    {
        if (terminada || tarjetasRecogidas < tarjetasNecesarias) return;

        terminada = true;
        puntuacionFinal = puntuacion;
        SceneManager.LoadScene("Victoria");
    }

    public void RecibirDanio()
    {
        if (terminada) return;

        vidasActuales--;
        ActualizarHUD();
        if (vidasActuales == 0)
        {
            terminada = true;
            if (AudioManager.Instance != null) AudioManager.Instance.ReproducirDerrota();
            textoFin.text = puntuacion.ToString("0000");
            textoFin.gameObject.SetActive(true);
            panelFin.SetActive(true);
        }
    }

    private void ActualizarHUD()
    {
        textoPuntuacion.text = puntuacion.ToString("0000");
        textoVidas.text = $"VIDAS: {vidasActuales}/{vidasMaximas}";
        textoTarjetas.text = $"{tarjetasRecogidas}/{tarjetasNecesarias}";
        barraVida.value = vidasActuales;
    }
}
