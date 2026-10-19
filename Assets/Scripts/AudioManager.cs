using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicaFondo;
    public AudioSource efectosSource;

    public AudioClip sonidoVictoria;
    public AudioClip sonidoDerrota;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ReproducirVictoria()
    {
        if (musicaFondo != null) musicaFondo.Stop();
        if (sonidoVictoria != null) efectosSource.PlayOneShot(sonidoVictoria);
    }

    public void ReproducirDerrota()
    {
        if (musicaFondo != null) musicaFondo.Stop();
        if (sonidoDerrota != null) efectosSource.PlayOneShot(sonidoDerrota);
    }
}