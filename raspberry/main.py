import paho.mqtt.client as mqtt
import time

def on_recieve(client, userdata, msg):
    print(f'Recieved: {msg.payload.decode("utf-8")}\n')
    # A function could be called from here to read in the data

client = mqtt.Client("rasppi")
client.message_callback_add("thread", on_recieve)

client.connect("localhost", 1883)
client.loop_start()
client.subscribe("thread")

print("Ctrl-C to break")
while True:
    msg = input("> ")
    if len(msg) > 0:
        print("Reserved to send a test message to the server")
    time.sleep(1)