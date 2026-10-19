using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Victoria : MonoBehaviour
{
    public TMP_Text textoPuntuacion;

    private void Start()
    {
        if (textoPuntuacion != null)
        {
            textoPuntuacion.text = Partida.puntuacionFinal.ToString("0000");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            VolverAJugar();
        }
    }

    public void VolverAJugar()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
