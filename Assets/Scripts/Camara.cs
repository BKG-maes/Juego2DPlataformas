using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Camara : MonoBehaviour
{
    public Transform personaje;
    public SpriteRenderer fondo;

    private Camera camara;

    private void Awake()
    {
        camara = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (personaje == null || fondo == null)
        {
            return;
        }

        // Mantiene los bordes horizontales de la vista dentro del fondo.
        float semiancho = camara.orthographicSize * camara.aspect;
        float limiteIzquierdo = fondo.bounds.min.x + semiancho;
        float limiteDerecho = fondo.bounds.max.x - semiancho;
        float posicionX;

        if (limiteIzquierdo > limiteDerecho)
        {
            posicionX = fondo.bounds.center.x;
        }
        else
        {
            posicionX = Mathf.Clamp(personaje.position.x, limiteIzquierdo, limiteDerecho);
        }

        Vector3 posicionCamara = transform.position;
        posicionCamara.x = posicionX;
        transform.position = posicionCamara;
    }
}
