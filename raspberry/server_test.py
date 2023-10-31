from bluedot.btcomm import BluetoothServer
from signal import pause


def data_received(data):
    print(data)
    s.send(data)


s = BluetoothServer(data_received)
pause()
# import pyshark
#
# # Create a Bluetooth LE display filter
# display_filter = 'btl2cap.cid == 0x0040 || btl2cap.cid == 0x0044'
#
# # Capture Bluetooth LE packets
# capture = pyshark.LiveCapture(interface='hci0', display_filter=display_filter)
#
# # Iterate through captured packets and print relevant information
# for packet in capture.sniff_continuously():
#     if 'L2CAP' in packet:
#         cid = packet['btl2cap.cid']
#         data = packet['l2cap'].replace(':', ' ')
#         print(f'CID: {cid}, Data: {data}')
#
#
#
#









# import asyncio
# import dbus
#
# adapter = dbus.SystemBus().get_object('org.bluez', '/')
# print(adapter)
# app = adapter.get_object('org.bluez.GattManager1', '/org/bluez/GattManager1')
#
# app.RegisterApplication(0, '', False)
#
# app.startDiscovery()
#
# def read_value(deviceId, serviceUuid, charUuid, val):
#     print(f'Device: {deviceId}, Service: {serviceUuid}, Characteristic: {charUuid}, Value: {val}')
#
# asyncio.run(app.RegisterValueChanged(None, None, None, read_value))
#
# app.StopDiscovery()


# import bluetooth
#
#
# def receiveMessages():
#     server_sock = bluetooth.BluetoothSocket(bluetooth.RFCOMM)
#
#     port = 1
#     server_sock.bind(("", port))
#     server_sock.listen(1)
#
#     client_sock, address = server_sock.accept()
#     print
#     "Accepted connection from " + str(address)
#
#     data = client_sock.recv(1024)
#     print
#     "received [%s]" % data
#
#     client_sock.close()
#     server_sock.close()
#
# receiveMessages()










# import bluetooth
# import pygatt
#
# adapter = pygatt.GATTToolBackend()
# device_mac = '64:B5:F2:D1:2B:B1'
#
#
# clue = bluetooth.find_service(uuid="00002b29-0000-1000-8000-00805f9b34fb")
# print(clue)
#
# def handle_data(handle, val):
#     print(f'Received data: {val.decode("ascii")}')
#
#
# try:
#     adapter.start()
#     device = adapter.connect(device_mac)
#
#     device.subscribe('00002b29-0000-1000-8000-00805f9b34fb', callback=handle_data)
#
#     input("Press Enter to stop")
#
# except KeyboardInterrupt:
#     pass
# finally:
#     adapter.disconnect()


#
# server_sock = bluetooth.BluetoothSocket(bluetooth.RFCOMM)
#
# server_sock.bind((bluetooth.read_local_bdaddr()[0], bluetooth.PORT_ANY))
#
# server_sock.listen(1)
#
# print(f"Waiting for connection on RFCOMM channel 1\n{bluetooth.read_local_bdaddr()[0]}")
#
# client_sock, client_info = server_sock.accept()
#
# print(f"Accepted connection from {client_info}")
#
# try:
#     while True:
#         data = client_sock.recv(1024)
#         if len(data) == 0:
#             break
#         print(f"Recieved: {data.decode('ascii')}")
#
# except KeyboardInterrupt:
#     pass
#
# print("Closing connection")
# client_sock.close()
# server_sock.close()
#
