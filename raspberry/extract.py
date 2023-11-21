import bluetooth._bluetooth as bluez
import struct

def receive_ble_messages(device_name):
    try:
        dev_id = bluez.hci_get_route(device_name)
        sock = bluez.hci_open_dev(dev_id)

        # Set the filter to listen to all HCI packets (0xFFFFFFF)
        flt = struct.pack("<L", 0xFFFFFFFF)
        sock.setsockopt(bluez.SOL_HCI, bluez.HCI_FILTER, flt)

        while True:
            pkt = sock.recv(255)
            print(pkt)
            # You can process and decode the packets here as needed

    except KeyboardInterrupt:
        pass
    except Exception as e:
        print(f"Error: {str(e)}")

if __name__ == "__main__":
    receive_ble_messages("hci0")

import pygatt
# import time
# import binascii
#
# adapter = pygatt.GATTToolBackend('hci0')
#
#
# # def handler(handle, data):
# #     print("\n data input:")
# #     print(data)
# #     print(data.decode("ascii"))
#
#
# try:
#     adapter.start()
#
#     device = adapter.connect("64:B5:F2:D1:2B:B0")
#     print("Connected!")
#
#     for uuid in device.discover_characteristics:
#         print(f"READING UUID: {uuid}\nData: {binascii.hexlify(device.char_read(uuid))}")
#
#
#     # print(device.char_read("00002b29-0000-1000-8000-00805f9b34fb"))
#     # print(device.)
#     #
#     # device.subscribe("00002b29-0000-1000-8000-00805f9b34fb", callback=handler)
#     # input("Ctrl-C to exit")
#
# finally:
#     print("stopped")
#     adapter.stop()
#
#





























# import asyncio
#
# from bleak import BleakClient
# from bleak import BleakScanner
#
# ardAddress = ''
# found = ''
# exit_flag = False
#
# temperaturedata = []
# timedata = []
# calibrationdata=[]
# quaterniondata=[]
#
# # loop: asyncio.AbstractEventLoop
#
# tempServiceUUID = '0000290c-0000-1000-8000-00805f9b34fb'  # Temperature Service UUID on Arduino 33 BLE
#
# stringUUID = '00002a56-0000-1000-8000-00805f9b34fb'  # Characteristic of type String [Write to Arduino]
# inttempUUID = '00002a1c-0000-1000-8000-00805f9b34fb'  # Characteristic of type Int [Temperature]
# longdateUUID = '00002a08-0000-1000-8000-00805f9b34fb'  # Characteristic of type Long [datetime millis]
#
# strCalibrationUUID = '00002a57-0000-1000-8000-00805f9b34fb'  # Characteristic of type String [BNO055 Calibration]
# strQuaternionUUID = '9e6c967a-5a87-49a1-a13f-5a0f96188552'  # Characteristic of type Long [BNO055 Quaternion]
#
#
# async def scanfordevices():
#     devices = await BleakScanner.discover()
#     for d in devices:
#         print(d)
#         if (d.name == 'TemperatureMonitor'):
#             global found, ardAddress
#             found = True
#             print(f'{d.name=}')
#             print(f'{d.address=}')
#             ardAddress = d.address
#             print(f'{d.rssi=}')
#             return d.address
#
#
# async def readtemperaturecharacteristic(client, uuid: str):
#     val = await client.read_gatt_char(uuid)
#     intval = int.from_bytes(val, byteorder='little')
#     print(f'readtemperaturecharacteristic:  Value read from: {uuid} is:  {val} | as int={intval}')
#
#
# async def readdatetimecharacteristic(client, uuid: str):
#     val = await client.read_gatt_char(uuid)
#     intval = int.from_bytes(val, byteorder='little')
#     print(f'readdatetimecharacteristic:  Value read from: {uuid} is:  {val} | as int={intval}')
#
#
# async def readcalibrationcharacteristic(client, uuid: str):
#     # Calibration characteristic is a string
#     val = await client.read_gatt_char(uuid)
#     strval = val.decode('UTF-8')
#     print(f'readcalibrationcharacteristic:  Value read from: {uuid} is:  {val} | as string={strval}')
#
#
# async def getservices(client):
#     svcs = await client.get_services()
#     print("Services:")
#     for service in svcs:
#         print(service)
#
#         ch = service.characteristics
#         for c in ch:
#             print(f'\tCharacteristic Desc:{c.description} | UUID:{c.uuid}')
#
#
# def notification_temperature_handler(sender, data):
#     """Simple notification handler which prints the data received."""
#     intval = int.from_bytes(data, byteorder='little')
#     # TODO:  review speed of append vs extend.  Extend using iterable but is faster
#     temperaturedata.append(intval)
#     #print(f'Temperature:  Sender: {sender}, and byte data= {data} as an Int={intval}')
#
#
# def notification_datetime_handler(sender, data):
#     """Simple notification handler which prints the data received."""
#     intval = int.from_bytes(data, byteorder='little')
#     timedata.append(intval)
#     #print(f'Datetime: Sender: {sender}, and byte data= {data} as an Int={intval}')
#
#
# def notification_calibration_handler(sender, data):
#     """Simple notification handler which prints the data received."""
#     strval = data.decode('UTF-8')
#     numlist=extractvaluesaslist(strval,':')
#     #Save to list for processing later
#     calibrationdata.append(numlist)
#
#     print(f'Calibration Data: {sender}, and byte data= {data} as a List={numlist}')
#
#
# def notification_quaternion_handler(sender, data):
#     """Simple notification handler which prints the data received."""
#     strval = data.decode('UTF-8')
#     numlist=extractvaluesaslist(strval,':')
#
#     #Save to list for processing later
#     quaterniondata.append(numlist)
#
#     print(f'Quaternion Data: {sender}, and byte data= {data} as a List={numlist}')
#
#
# def extractvaluesaslist(raw, separator=':'):
#     # Get everything after separator
#     s1 = raw.split(sep=separator)[1]
#     s2 = s1.split(sep=',')
#     return list(map(float, s2))
#
#
# async def runmain():
#     # Based on code from: https://github.com/hbldh/bleak/issues/254
#     global exit_flag
#
#     print('runmain: Starting Main Device Scan')
#
#     await scanfordevices()
#
#     print('runmain: Scan is done, checking if found Arduino')
#
#     if found:
#         async with BleakClient(ardAddress) as client:
#
#             print('runmain: Getting Service Info')
#             await getservices(client)
#
#             # print('runmain: Reading from Characteristics Arduino')
#             # await readdatetimecharacteristic(client, uuid=inttempUUID)
#             # await readcalibrationcharacteristic(client, uuid=strCalibrationUUID)
#
#             print('runmain: Assign notification callbacks')
#             await client.start_notify(inttempUUID, notification_temperature_handler)
#             await client.start_notify(longdateUUID, notification_datetime_handler)
#             await client.start_notify(strCalibrationUUID, notification_calibration_handler)
#             await client.start_notify(strQuaternionUUID, notification_quaternion_handler)
#
#             while not exit_flag:
#                 await asyncio.sleep(1)
#             # TODO:  This does nothing.  Understand why?
#             print('runmain: Stopping notifications.')
#             await client.stop_notify(inttempUUID)
#             print('runmain: Write to characteristic to let it know we plan to quit.')
#             await client.write_gatt_char(stringUUID, 'Stopping'.encode('ascii'))
#     else:
#         print('runmain: Arduino not found.  Check that its on')
#
#     print('runmain: Done.')
#
#
# def main():
#     # get main event loop
#     loop = asyncio.get_event_loop()
#
#     try:
#         loop.run_until_complete(runmain())
#     except KeyboardInterrupt:
#         global exit_flag
#         print('\tmain: Caught keyboard interrupt in main')
#         exit_flag = True
#     finally:
#         pass
#
#     print('main: Getting all pending tasks')
#
#     # From book Pg 26.
#     pending = asyncio.all_tasks(loop=loop)
#     print(f'\tmain: number of tasks={len(pending)}')
#     for task in pending:
#         task.cancel()
#     group = asyncio.gather(*pending, return_exceptions=True)
#     print('main: Waiting for tasks to complete')
#     loop.run_until_complete(group)
#     loop.close()
#
#     # Display data recorded in Dataframe
#     if len(temperaturedata)==len(timedata):
#         print(f'Temperature data len={len(temperaturedata)}, and len of timedata={len(timedata)}')
#
#         df = pd.DataFrame({'datetime': timedata,
#                            'temperature': temperaturedata})
#         #print(f'dataframe shape={df.shape}')
#         #print(df)
#         df.to_csv('temperaturedata.csv')
#     else:
#         print(f'No data or lengths different: temp={len(temperaturedata)}, time={len(timedata)}')
#
#     if len(quaterniondata)==len(calibrationdata):
#         print('Processing Quaternion and Calibration Data')
#         #Load quaternion data
#         dfq=pd.DataFrame(quaterniondata,columns=['time','qw','qx','qy','qz'])
#         print(f'Quaternion dataframe shape={dfq.shape}')
#         #Add datetime millis data
#         #dfq.insert(0,'Time',timedata)
#         #Load calibration data
#         dfcal=pd.DataFrame(calibrationdata,columns=['time','syscal','gyrocal','accelcal','magcal'])
#         print(f'Calibration dataframe shape={dfcal.shape}')
#         #Merge two dataframes together
#         dffinal=pd.concat([dfq,dfcal],axis=1)
#         dffinal.to_csv('quaternion_and_cal_data.csv')
#     else:
#         print(f'No data or lengths different. Quat={len(quaterniondata)}, Cal={len(calibrationdata)}')
#         if len(quaterniondata)>0:
#             dfq = pd.DataFrame(quaterniondata, columns=['time', 'qw', 'qx', 'qy', 'qz'])
#             dfq.to_csv('quaterniononly.csv')
#         if len(calibrationdata)>0:
#             dfcal = pd.DataFrame(calibrationdata, columns=['time','syscal', 'gyrocal', 'accelcal', 'magcal'])
#             dfcal.to_csv('calibrationonly.csv')
#
#     print("main: Done.")
#
#
# if __name__ == "__main__":
#     '''Starting Point of Program'''
#     main()










# from bluepy.btle import Peripheral, DefaultDelegate
#
# # Replace with your Raspberry Pi's device address and characteristic UUID
# device_address = "D8:3A:DD:3C:F1:3C"
# char_uuid = "00002b29-0000-1000-8000-00805f9b34fb"
#
# class CharacteristicDelegate(DefaultDelegate):
#     def handleNotification(self, cHandle, data):
#         print(f"Received data: {data.hex()}")
#
# peripheral = Peripheral(device_address)
# peripheral.setDelegate(CharacteristicDelegate())
#
# # Get the characteristic
# characteristic = peripheral.getCharacteristics(uuid=char_uuid)[0]
#
# # Enable notifications
# peripheral.writeCharacteristic(characteristic.valHandle + 1, b"\x01\x00")
#
# try:
#     while True:
#         if peripheral.waitForNotifications(1.0):
#             continue
#         print("Waiting for notifications...")
# except KeyboardInterrupt:
#     peripheral.disconnect()
#
#
#
#
#
#
#
#
#
#
# #
# # import asyncio
# #
# # from bleak import BleakClient
# # from bleak import BleakScanner
# #
# #
# #
# # async def scanfordevices():
# #     devices = await BleakScanner.discover()
# #     for d in devices:
# #         print(d)
# #         print(d.details['props']["Paired"])
# #         if d.details['props']["Paired"] is True:
# #             print(d)
# #             print(f'{d.name=}')
# #             print(f'{d.address=}')
# #             print(f'{d.details=}')
# #             myine = BleakClient(d.address)
# #             await myine.connect()
# #             # svcs = await myine.get_services()
# #             print("Services:")
# #             for service in myine.services:
# #                 print(service)
# #
# #
# # asyncio.run(scanfordevices())
