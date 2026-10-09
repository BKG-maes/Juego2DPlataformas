# Clase 2: plataformas

Abre `Assets/Scenes/SampleScene.unity` en Unity 6000.4.12f1 y pulsa Play.

## Controles y objetivo

- A/D o flechas izquierda/derecha: moverse con el eje `Horizontal` del Input Manager clásico.
- Espacio, W o flecha arriba: saltar.
- Botón Reiniciar o R: volver a empezar después de ganar o perder. No hay controles de mando.
- Recoge la tarjeta (250 puntos) y el dinero (100 puntos); después llega al cofre para ganar 500 puntos adicionales.
- Empiezas con 100 de salud y 3 vidas. El martillo hace 25 de daño; después de cada golpe hay 1,1 segundos de invulnerabilidad. Caer fuera del nivel o perder toda la salud consume una vida.

## Scripts

- `PlayerController`: movimiento horizontal, salto, comprobaciones de suelo/techo, gravedad manual, daño y reaparición.
- `PlayerAnimationController`: elige entre quieto, correr, salto, caída, daño y muerte.
- `GameManager`: vidas, puntos, salud, objetivo, HUD, victoria, derrota y reinicio.
- `Collectible`: suma los puntos una vez y destruye el objeto recogido.
- `DamageZone`: comunica el daño del martillo al jugador.
- `ExitGoal`: abre el cofre cuando ya se recogieron los dos objetos.

## Relación con FCV4 y FCV5

`PlayerController` sigue FCV4: mueve el `Rigidbody2D` con un `Vector2` en `FixedUpdate`, comprueba el suelo y el techo con `BoxCast` y aplica gravedad desde el código con `gravityScale = 0`. El salto es básico, sin coyote time, memoria de salto ni atravesar plataformas.

Las interacciones siguen FCV5: los objetos usan `OnTriggerEnter2D` y `Collectible` llama a `Destroy(gameObject)`. El respawn se agenda con `Invoke`. El collider del martillo se activa solo durante los fotogramas de golpe configurados en su animación y cubre la parte baja: así el daño coincide con el impacto, no con todo el recorrido. La protección de 1,1 segundos evita perder salud en cada frame si el jugador permanece dentro. El ciclo del martillo está a 4 fps (2 segundos por ciclo); su velocidad se ajusta en el editor, no desde el script.

## Configuración guardada en Unity

- **Active Input Handling** está en **Both**. El script lee `Input.GetAxisRaw("Horizontal")` y `Input.GetKeyDown`.
- El jugador usa la capa **Suelo** para `groundLayers`. Su `Rigidbody2D` tiene **Gravity Scale = 0**, tanto en la escena como en el prefab; la gravedad se calcula en el controlador.
- Las referencias de `GameManager` al jugador, punto de aparición, HUD, panel final y botón están asignadas.
- El botón Reiniciar tiene un evento persistente `On Click()` conectado a `Clase2 > GameManager.Restart()` desde el Inspector.
- `Martillo.anim` está a **4 fps**, dura **2 segundos** y se repite. `BoxCollider2D > Enabled` está desactivado en el fotograma 0, activado en el 3 (0,75 s) y desactivado en el 5 (1,25 s): hay **1,5 segundos sin daño por ciclo**.
- El collider del martillo es un trigger de tamaño `(0.85, 0.45)` y desplazamiento `(0, -0.75)`, ajustado a su parte baja.
- Main Camera es fija, con tamaño ortográfico **5,35**. Se retiraron el componente y el script `LevelCameraFit`.
- El HUD está anclado arriba a la derecha y el pie muestra los controles actuales.
- Los ajustes de la escena, el prefab y la animación se hicieron en el editor. Se eliminó `Class2SceneSetup`; no hay un generador de escena por código.

La escena está guardada, Unity ha terminado de compilar sin errores y no contiene componentes con scripts ausentes. La revisión final fue de configuración; queda pendiente una partida completa para valorar el recorrido y la dificultad.

No se incluyen sonidos en esta clase.
