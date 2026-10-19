# Revisión del juego con el temario

Revisión del 19 de octubre de 2026 sobre `clase2-experimental`.

Se revisaron los apuntes FCV 0–3, los apuntes de programación completos, las transparencias de las clases 0–6, las prácticas del bloque 1 y las planificaciones de Arkanoid y plataformas. También se comprobó que la guía de Blender corresponde a otro trabajo. Las copias repetidas del mismo PDF contienen el mismo material.

El criterio para este juego es la planificación de **plataformas 2D**. Los apuntes presentan alternativas y una progresión de ejemplos; no todos esos enfoques deben mezclarse en un mismo controlador.

## Correspondencia por temas

| Tema y referencia | Aplicación en el proyecto |
| --- | --- |
| PlanificacionPlataformas.pdf, p. 1 | Tilemap y fondo; animaciones de objetos y personaje; movimiento; recogidas y daño; HUD; fin de juego y sonidos. |
| FCV_1.pdf, pp. 10, 27–29; Apuntes_programacion.pdf, pp. 5 y 19 | Referencias en Awake, entrada y temporizadores en Update, movimiento físico en FixedUpdate y cámara en LateUpdate. |
| FCV_2.pdf, pp. 3–6 y 18–24 | Entrada Legacy y Rigidbody2D. El proyecto permite ambos sistemas de entrada; los scripts de juego usan Input.GetAxisRaw y GetKeyDown. |
| FCV_3.pdf, pp. 9–22; FCV_3_Clase.pdf, pp. 8–14 | Cápsula para el jugador, triggers para objetos, capas para filtrar rayos, material sin fricción y Debug.DrawRay. El Tilemap se fusiona con CompositeCollider2D y un Rigidbody2D Static. En Unity 6, Merge es el ajuste que sustituye a Used By Composite. |
| FCV_4_Clase.pdf, pp. 11–14 | Personaje conserva un cuerpo Dynamic sin gravedad automática, detecta suelo y techo con rayos y calcula la velocidad vertical mediante gravedad y Time.fixedDeltaTime. |
| FCV_5_Clase.pdf, pp. 8–11 | Recogidas y daño mediante triggers. Partida habilita la salida al reunir las tarjetas. Los martillos animan sprites y el collider; no transportan al jugador ni desplazan plataformas. |
| FCV_1.pdf, pp. 38–49; PracticasFCVBloque1.pdf, pp. 2–4 | Canvas, anclajes, escala de pantalla, TMP, botones y collider compuesto. La victoria tiene referencia 1920×1080, CanvasScaler, GraphicRaycaster y un EventSystem. Los gráficos decorativos no interceptan los clics. |
| FCV_1.pdf, pp. 43–44; Apuntes_programacion.pdf, pp. 20, 24 y 28 | Cambio de escena con SceneManager.LoadScene y botón conectado a un método público desde el Inspector. SampleScene y Victoria están activadas en Build Settings. |
| Apuntes_programacion.pdf, pp. 21–24 y 29–31; FCV_5_Clase.pdf, pp. 13–15 | AudioSource/AudioClip, parámetros del Animator y temporizadores. La música de victoria es el mismo clip anterior, asignado a un AudioSource de la escena final con Play On Awake. |
| FCV_6_Clase.pdf, pp. 3–14 | Se revisaron el ejemplo básico de movimiento y la separación Interaccion/Interactivo/control. Son alternativas posteriores; este juego conserva rayos y gravedad de la clase 4 e interacción por tags de la clase 5. |

Las páginas citadas son las del PDF, que pueden diferir del número impreso en la diapositiva. Los archivos están en las carpetas `Apuntes`, `Teor_a` y `Pr_cticas` del temario FCV.

## Adaptaciones que conviene saber explicar

- **Tres rayos por dirección:** el ejemplo enseña un Raycast. Aquí se comprueba también a ambos lados de la cápsula para detectar los bordes de plataformas. El filtrado se hace con la capa Suelo.
- **Gravedad constante:** se sigue el algoritmo básico de la clase 4. No se añadieron doble salto, plataformas móviles, escaleras ni un sistema de estados avanzado.
- **Score entre escenas:** Partida guarda un único entero estático antes de cargar Victoria. Los apuntes enseñan miembros estáticos en los gestores, pero no contienen este ejemplo exacto de paso de puntuación. Es una adaptación pequeña que evita añadir un gestor persistente. Al empezar otra partida, ese entero se reinicia.
- **Interacciones:** se suma puntuación antes de destruir el objeto, desactivando primero el trigger para evitar sumar dos veces. Se mantiene el enfoque de tags y triggers. No se afirma que reproduzca la arquitectura con herencia de la clase 6 ni que implemente todos los patrones SOLID, Observer o State explicados como ampliaciones.
- **Cámara:** seguimiento horizontal con límites al fondo mediante Mathf.Clamp. Aplica LateUpdate y Clamp del temario; no es una copia literal de un script del profesor.
- **Reglas del nivel:** tres tarjetas, tres vidas y los valores de puntuación son decisiones de este juego, no cantidades impuestas en la planificación.

## Última simplificación de Personaje y Partida

Tras contrastar de nuevo la planificación y las clases 4 y 5, `Personaje.cs` queda en 138 líneas y `Partida.cs` en 100, sin comentarios. Se retiran el tinte rojo, el bloqueo tras el golpe y los tres temporizadores; el estado de muerte duplicado; `OnTriggerStay2D`; el código de reposo al ganar que ya no necesita una escena que se descarga; y los métodos que solo envolvían el reinicio o el panel de derrota. Las referencias de componentes y HUD se comprobaron en el Inspector antes de quitar sus comprobaciones de nulos repetidas. Se mantiene el esquema de rayos, gravedad y FixedUpdate de la clase 4, y tags, OnTriggerEnter2D y salida condicionada de la clase 5. La gravedad actual `23.62` es el ajuste del usuario y se conserva.

Cada nuevo contacto con una prensa activa resta una vida. Su animación apaga y reactiva el collider en cada golpe, por lo que no hace falta procesar daño continuo desde Stay. Dos peligros distintos pueden restar dos vidas: se ha eliminado la invulnerabilidad adicional. Las tres tarjetas y la escena de victoria solicitada se mantienen como reglas ya existentes del juego.

La comprobación final compiló sin errores y verificó en Play derrota por golpes reales, reinicio con R, salida bloqueada sin tarjetas, cofre (50), dinero (10), tres tarjetas (75), salida habilitada, Victoria con `0135` y regreso con tres vidas, cero puntos y cero tarjetas. No se añadieron scripts de prueba ni se guardaron posiciones temporales. La programación de esta revisión se realizó con Luna Max.

## Escena de victoria

`Assets/Scenes/Victoria.unity` contiene cámara y AudioListener, Canvas, panel, textos, imagen del cofre, botón, EventSystem, AudioSource y el componente Victoria. El diseño queda guardado en objetos de la escena y se puede modificar desde la jerarquía y el Inspector. No se construye mediante código al ejecutar el juego.

El botón llama a `Victoria.VolverAJugar()`. R llama al mismo método. `Victoria.Start()` muestra el entero recibido con `ToString("0000")`. El controlador ocupa 29 líneas y no tiene comentarios.

## Comprobaciones realizadas

- Compilación de los scripts sin errores de C#.
- Apertura directa de Victoria: marcador 0000 y referencias de UI asignadas.
- Regreso al nivel mediante R.
- Con el collider compuesto, el personaje permanece sobre el suelo y sobre la plataforma de la primera tarjeta.
- Recogida de la primera y segunda tarjetas; contador 2 antes de la última.
- Tres tarjetas: 75 puntos. Tocar la salida carga Victoria y muestra 0075.
- El botón seleccionado, confirmado con Enter, vuelve a SampleScene.
- Después de volver: tres vidas, cero puntos, cero tarjetas y partida sin terminar.
- AudioSource de Victoria: el mismo clip de victoria anterior, Play On Awake activado, sin Loop y con AudioListener en la cámara.
- Revisión visual del diseño a 1920×1080 y captura en `Documentacion/victoria.png`.

Las comprobaciones de recogida colocaron temporalmente al personaje desde el Inspector/CLI durante Play. No sustituyen una partida manual completa para valorar la dificultad o recorrer todos los bordes del mapa. No se guardaron esas posiciones ni las recogidas de prueba en la escena.

## Entrega y ayuda de IA

FCV_0_Clase.pdf, p. 27, pide identificar la ayuda de IA y que el grupo entienda y pueda modificar el código. En esta sesión se usó Codex y Luna Max para la lógica de cambio de escena y puntuación, revisión del código y asistencia en la pantalla de victoria. El diseño visual recibió ajustes individuales con los controles del editor/CLI. La simplificación previa de Personaje también recibió ayuda de IA.

El mismo apartado exige que el juego contenga partes no generadas por IA. El grupo debe completar la autoría del trabajo restante con información real. La revisión técnica no certifica autoría humana ni sustituye la explicación del grupo.

La planificación de Arkanoid y las partes de Blender/3D describen otros ejercicios. Sus requisitos específicos de versión, menú inicial, exportaciones o modelado no se han añadido automáticamente a este juego de plataformas. El proyecto actual usa Unity 6000.4.12f1; antes de entregar conviene contrastar el enunciado concreto de plataformas y sus archivos de entrega.
