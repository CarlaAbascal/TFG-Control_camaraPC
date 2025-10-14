"""
detectar_mano.py
----------------
Script para detectar gestos de la mano usando MediaPipe y enviar el resultado
a la aplicación C# (Form1.cs) mediante un socket TCP.

GESTOS:
  Puño → Aterrizar
  Un dedo → Avanzar
  Dos dedos → Girar derecha
  Tres dedos → Girar izquierda
  Palma → Despegar
"""

import cv2
import mediapipe as mp
import socket
import time

# ---------------------------- CONFIGURACIÓN DEL SOCKET ----------------------------
# El script Python actúa como cliente TCP.
# En C# deberás iniciar un servidor (TcpListener) que escuche en el mismo puerto.
TCP_IP = '127.0.0.1'   # Dirección local (localhost)
TCP_PORT = 5005        # Puerto de comunicación (debe coincidir con el de C#)

sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
print("[INFO] Conectando con el servidor C#...")
sock.connect((TCP_IP, TCP_PORT))
print("[OK] Conectado con la aplicación C#")

# ---------------------------- CONFIGURACIÓN DE MEDIAPIPE ----------------------------
mp_hands = mp.solutions.hands
mp_drawing = mp.solutions.drawing_utils

hands = mp_hands.Hands(
    static_image_mode=False,      # Para video en tiempo real
    max_num_hands=1,              # Solo analizamos una mano
    min_detection_confidence=0.7,
    min_tracking_confidence=0.5
)

# ---------------------------- INICIAR CÁMARA ----------------------------
cap = cv2.VideoCapture(0)
cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)

if not cap.isOpened():
    print("[ERROR] No se puede acceder a la cámara.")
    exit()

# ---------------------------- FUNCIÓN DE DETECCIÓN DE GESTOS ----------------------------

def detectar_gesto(hand_landmarks):
    """
    Determina el gesto según qué dedos están extendidos.
    Devuelve un string con el nombre del gesto.
    """
    tips = [4, 8, 12, 16, 20]  # Índices de las puntas de los dedos
    dedos = []

    # --- Pulgar ---
    # Comparamos la coordenada X con su articulación anterior.
    if hand_landmarks.landmark[tips[0]].x < hand_landmarks.landmark[tips[0] - 1].x:
        dedos.append(1)
    else:
        dedos.append(0)

    # --- Otros 4 dedos ---
    # Si la punta del dedo (tip) está por encima (menor Y) que la articulación intermedia (PIP)
    for id in range(1, 5):
        if hand_landmarks.landmark[tips[id]].y < hand_landmarks.landmark[tips[id] - 2].y:
            dedos.append(1)
        else:
            dedos.append(0)

    total_dedos = dedos.count(1)

    # --- Mapeo del número de dedos a gestos ---
    if total_dedos == 0:
        return "puño"      # ✊ → aterrizar
    elif total_dedos == 1:
        return "uno"       # ☝️ → avanzar
    elif total_dedos == 2:
        return "dos"       # ✌️ → derecha
    elif total_dedos == 3:
        return "tres"      # 🤟 → izquierda
    elif total_dedos >= 4:
        return "palm"      # 🖐️ → despegar
    else:
        return None


#Contorl del envío de gestos
ultimo_gesto = None
ultimo_tiempo = 0
DELAY_GESTO = 0.8  # segundos de estabilidad antes de enviar

sock.setblocking(False)  # evita bloqueo del envío

# ---------------------------- BUCLE PRINCIPAL ----------------------------
while True:
    success, frame = cap.read()
    if not success:
        print("[ERROR] No se pudo leer frame de la cámara.")
        break

    frame_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    results = hands.process(frame_rgb)

    gesto_detectado = None

    if results.multi_hand_landmarks:
        for hand_landmarks in results.multi_hand_landmarks:
            # Dibujamos los 21 puntos clave
            mp_drawing.draw_landmarks(frame, hand_landmarks, mp_hands.HAND_CONNECTIONS)

            # Detectar gesto actual
            gesto_detectado = detectar_gesto(hand_landmarks)
            
            # Mostrar en pantalla el gesto actual
            cv2.putText(frame, gesto_detectado if gesto_detectado else "",
                        (10, 60), cv2.FONT_HERSHEY_SIMPLEX, 1.2, (0, 255, 0), 3)

            #Envío de gesto solo si se mantiene el tiempo
            ahora = time.time()
            if gesto_detectado:
                if gesto_detectado == ultimo_gesto:
                    if ahora - ultimo_tiempo > DELAY_GESTO:
                        try:
                            sock.sendall(gesto_detectado.encode('utf-8'))
                            print(f"[GESTO] Enviado: {gesto_detectado}")
                        except BlockingIOError:
                            pass # ignora si el socket está ocupado
                        except:
                            print("[ERROR] No se pudo enviar el gesto.")
                        ultimo_tiempo = ahora  # Reinicia el temporizador
                else:
                    # Nuevo gesto detectado → reinicia el contador de tiempo
                    ultimo_gesto = gesto_detectado
                    ultimo_tiempo = ahora

            cv2.imshow("Gestos - MediaPipe", frame)

    # Salir con tecla ESC
    if cv2.waitKey(1) & 0xFF == 27:
        break
# ---------------------------- FINALIZAR ----------------------------
cap.release()
cv2.destroyAllWindows()
sock.close()
print("[INFO] Conexión cerrada correctamente.")
