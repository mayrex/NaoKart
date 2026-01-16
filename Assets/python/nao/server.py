import socket
import threading
import paho.mqtt.client as mqtt
#import ai

# Configurazione MQTT
MQTT_BROKER = "broker.emqx.io"  # Cambia se usi un altro broker
MQTT_PORT = 1883
MQTT_TOPIC = "checkpoint"

# Configurazione server socket
SERVER_HOST = "192.168.56.1"
SERVER_PORT = 6969
clients = []  # Lista dei client connessi

# Callback quando il server si connette al broker MQTT
def on_connect(client, userdata, flags, rc):
    if rc == 0:
        print("[MQTT] Connesso al broker!")
        client.subscribe(MQTT_TOPIC)
        client.subscribe("AI")
        client.subscribe("HandShake")
        client.subscribe("Uscita_pista")
    else:
        print(f"[MQTT] Errore di connessione, codice: {rc}")

# Callback quando arriva un messaggio MQTT
def on_message(client, userdata, msg):
    message = msg.payload.decode("utf-8")
    topic = msg.topic
    formatted_message = f"{topic}|{message}"
    print(f"[MQTT] Ricevuto '{message}' su '{topic}', inoltro ai client...")
    """
    if topic=="AI":
        print(f"[MQTT] Ricevuto '{message}' su '{topic}', inoltro ai client...")
        ai.run(message)
    """


    # Invia il messaggio a tutti i client socket
    for sock in clients:
        try:
            sock.sendall(formatted_message.encode("utf-8"))
        except:
            clients.remove(sock)

# Funzione per gestire i client socket
def handle_client(client_socket, client_address):
    print(f"[SOCKET] Nuovo client connesso: {client_address}")
    clients.append(client_socket)

    while True:
        try:
            data = client_socket.recv(1024).decode("utf-8")
            if not data:
                break
            print(f"[SOCKET] Ricevuto dal client {client_address}: {data}")
        except:
            break

    print(f"[SOCKET] Client {client_address} disconnesso")
    clients.remove(client_socket)
    client_socket.close()

# Avvio server socket
def start_socket_server():
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.bind((SERVER_HOST, SERVER_PORT))
    server.listen(5)
    print(f"[SOCKET] Server avviato su {SERVER_HOST}:{SERVER_PORT}")

    while True:
        client_socket, client_address = server.accept()
        threading.Thread(target=handle_client, args=(client_socket, client_address), daemon=True).start()

# Configura e avvia il client MQTT
mqtt_client = mqtt.Client()
mqtt_client.on_connect = on_connect
mqtt_client.on_message = on_message
mqtt_client.connect(MQTT_BROKER, MQTT_PORT, keepalive=60)

# Avvia il server socket in un thread separato
threading.Thread(target=start_socket_server, daemon=True).start()

# Avvia il loop MQTT
mqtt_client.loop_forever()
