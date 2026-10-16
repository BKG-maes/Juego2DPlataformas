# Clase 2 — Juego de plataformas

Abre `Assets/Scenes/SampleScene.unity` en Unity **6000.4.12f1** y pulsa Play. El paquete `com.unity.pipeline` permite editar y comprobar el proyecto con Unity CLI; la lógica del juego está en los dos scripts `Personaje.cs` y `Partida.cs`.

## Alcance de la planificación

La planificación de plataformas pide para Clase 2: animaciones del personaje y un script que las controle; movimiento; colisiones con objetos recogibles y con objetos dañinos; interacción con el HUD; y fin de juego. Indica además que los sonidos son obligatorios, pero se hacen fuera de clase. Como trabajo previo de Clase 1 figuran el tilemap y fondo, las animaciones automáticas de objetos y el HUD de puntuación y vidas. Fuente: `../../Asignaturas/(3861)FUNDAMENTOSCOMPUTACIONALESDELOSVIDEOJUEGOS[26_27]/Teor_a/PlanificacionPlataformas.pdf`, p. 1.

## Controles y reglas actuales

- **A/D o flechas izquierda/derecha:** moverse. **Espacio:** saltar. El proyecto usa la opción de entrada `Both`; estos controles leen Legacy Input mediante `Input.GetAxisRaw`.
- **R:** reiniciar la escena después de terminar, tanto por derrota como por victoria.
- El personaje es Biker y empieza con **3 vidas**. Al llegar a cero aparece «FIN DEL JUEGO». Caer bajo el escenario no quita vidas ni provoca reaparición.
- Los siete martillos tienen Collider2D animados por el clip `MartilloAnimacion`; solo hacen daño en los tramos `Hammer_2` a `Hammer_6`. Los tramos `Hammer_7`, `Hammer_0` y `Hammer_1` son de seguridad. La animación conserva la velocidad **0,5**. No hay una clase dedicada a martillos ni código generador.
- Dinero suma **10**, cartas **25** y cofres **50** puntos. Cada objeto recogido se elimina. Tras recoger las **tres tarjetas**, `Partida` actualiza el contador del HUD y activa el trigger de salida. Tocar la salida entonces muestra «HAS ESCAPADO» con la puntuación; se puede reiniciar con R. El requisito de tres tarjetas es una decisión de diseño confirmada por el usuario, no una condición especificada por el profesor.
- El personaje aparece junto a la pantalla roja, en **(-9, -2.45)**. La salida reutiliza la pantalla `Screen2` del paquete, guardada como `Assets/Prefab/Pantalla.prefab`, colocada al final del nivel en **(10.45, -3.45, 0)**, con tag `Salida`, `BoxCollider2D` como Trigger y cartel «SALIDA» en posición local **(-0.3, 0.85, -0.1)**.
- Las puntuaciones, las tres vidas, la condición de tres tarjetas, los controles y la tecla R son decisiones concretas de esta implementación; la planificación no fija esos valores ni esos controles. Los sonidos siguen pendientes y no están implementados.

## Estructura preparada

```text
SampleScene
├─ Personaje — Personaje.cs
│  ├─ Rigidbody2D y CapsuleCollider2D
│  ├─ SpriteRenderer
│  └─ Animator — Personaje.controller
│     ├─ Speed (float)
│     ├─ Grounded (bool)
│     └─ Jump, Hurt, Death (triggers)
├─ Martillos — Collider2D activado por MartilloAnimacion
├─ Salida — tag Salida, BoxCollider2D Trigger, cartel SALIDA
├─ Fondo_Tilemap — tag Suelo y collider 2D
├─ Partida — Partida.cs; cuenta tarjetas y gestiona salida y fin
├─ Main Camera — fija; Orthographic, centro (0, -0.5, -10), tamaño 6.9
└─ Canvas — puntuación, vidas, tarjetas, barra y panel de fin
```

La escena contiene 18 objetos de recogida y daño: 7 de dinero, 3 tarjetas, 1 cofre y 7 martillos, además de la salida. Se eliminó `/Cofres` en la zona inferior (antes en **(10.466, -3.50)**) y se conserva `/Chest_0` en la parte superior. `/Card_0 (1)` se movió de **(10.46, 1.36)** a **(10.1, 1.36)** para que quede dentro del encuadre. El HUD incluye el contador de tarjetas. La cámara permanece fija y encuadra el nivel, las tarjetas y la salida sin recortes. Se conservan los dibujos existentes; no se añadieron scripts de cámara ni código generador.

## Cambios recientes — 16 de octubre de 2026

La variante de trabajo queda documentada en la rama `clase2-experimental`, que sustituye la variante anterior y conserva su historial. Se mantiene el daño de martillos solo en los tramos `Hammer_2`–`Hammer_6`, con seguridad en `Hammer_7`, `Hammer_0` y `Hammer_1`, y velocidad de animación 0,5. También se conserva la aparición junto a la pantalla roja y la salida que se habilita al recoger tres tarjetas. En esta actualización se retiró el cofre inferior, se colocó la salida al final del nivel, se ajustó la tarjeta al encuadre y se fijó la cámara para mostrar el nivel completo.

## Relación con los apuntes

- **Alcance y salida:** `PlanificacionPlataformas.pdf`, p. 1, incluye literalmente «Fin de juego», sin especificar la condición concreta. `Teor_a/FCV_5_Clase.pdf`, p. 10 (número impreso de diapositiva 8), recomienda acabar mediante un objeto final, por ejemplo una puerta, que puede activarse al cumplirse una condición; una llave es solo el ejemplo de la diapositiva. La salida condicionada a recoger tres tarjetas aplica esa pauta, pero la cantidad de tarjetas la eligió el usuario.
- **Entrada y movimiento:** `Apuntes/FCV_2.pdf`, p. 6, describe Legacy Input; p. 22 muestra el movimiento horizontal con `Rigidbody2D.velocity` conservando la velocidad vertical. El script usa `Rigidbody2D.linearVelocity`, la propiedad equivalente de la API actual.
- **Triggers 2D:** `Apuntes/FCV_3.pdf`, p. 13, pone monedas, checkpoints, meta y zona de daño como ejemplos de Collider2D Trigger; p. 14 muestra `OnTriggerEnter2D(Collider2D col)`.
- **HUD:** `Apuntes/FCV_1.pdf`, pp. 45–47, presenta texto o iconos, Slider y actualización de salud en el HUD.
- **Animación y fin de juego:** `Apuntes/Apuntes_programacion.pdf`, pp. 20 y 23–24, incluye `SceneManager`, el estado `GameOver` y el control de parámetros del Animator.
- **Sonido pendiente:** `Apuntes/Apuntes_programacion.pdf`, pp. 21–22, describe `AudioSource`, `AudioClip` y `PlayOneShot`.

## Verificación

Compilación comprobada sin errores de C#. Se comprobaron en Play los ocho fotogramas de los siete martillos (56 comprobaciones): los fotogramas 7/0/1 desactivan el collider de daño y los fotogramas 2–6 lo activan. El contacto con un martillo abierto no resta vidas, incluso después de acabar la inmunidad; al cerrarse vuelve a hacer daño.

Se comprobó la aparición junto a la pantalla roja; la salida bloqueada con cero, una o dos tarjetas; su apertura al recoger la tercera; el HUD 3/3 y 75 puntos por las tres tarjetas; la victoria al tocar la pantalla; el personaje detenido en Idle y el reinicio con tres vidas, cero tarjetas y la salida bloqueada. Las comprobaciones previas de recogida, derrota, fricción de paredes y caída sin daño se mantienen. Unity quedó fuera de Play.

Tras los últimos cambios se repitieron en Play las comprobaciones de recogida de las tres tarjetas, bloqueo y apertura de la salida, victoria y reinicio. También se verificó que las tres tarjetas y la pantalla de salida quedan completas dentro de un encuadre 16:9 y que solo permanece el cofre superior. Estas pruebas de interacción desplazan al personaje hasta los objetos; no sustituyen una partida manual completa para valorar la dificultad del recorrido.

![Nivel con la salida al final y las tarjetas dentro del encuadre](Documentacion/clase2.png)
