using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using InTheHand.Net;
using System.Text;
using System;
using System.Security.Cryptography;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Extensions;
using Microsoft.Maui.Devices.Sensors;
using MQTTnet.Server;
using MQTTnet.Client;
using MQTTnet;

namespace MobileTrashBin;

public partial class MainPage : ContentPage
{

    IBluetoothLE ble = CrossBluetoothLE.Current;
    IAdapter adapter = CrossBluetoothLE.Current.Adapter;

    ICharacteristic _characteristic = null;
    string uuid = null;

    BluetoothClient client = new BluetoothClient();
    BluetoothDeviceInfo rasp = null;
    // Bluetooth Address: DCA6322171C1 (Byte array is inveresed "C1", "71", "21", "32", "A6", "DC")
    //static byte[] addr = { 193, 113, 33, 50, 166, 220 };
    BluetoothAddress raspAdr;// = new BluetoothAddress(addr);


    public MainPage()
    {
        InitializeComponent();
        RequestAccess();

        string default_addr = "192.168.0.104";
        _addr.Text = default_addr;
        ConnectMQTT();
    }


    int _count = 1;
    string _recent = null;
    private async void SummonRequest(object sender, EventArgs e)
    {
        try
        {
            CallBot.Text = "Sending...";
            CallBot.IsEnabled = false;
            ConnectMQTT();
            Location location = await Geolocation.GetLocationAsync(new GeolocationRequest()
            {
                DesiredAccuracy = GeolocationAccuracy.Best,
                Timeout = TimeSpan.FromSeconds(30)
            });
            if (location != null)
                _recent = $"{location.Latitude}, {location.Longitude}";

            _tx_box.Text += $"\nLocation {_count}:\n{_recent}\n";
            _count++;
            // await _characteristic.WriteAsync(Encoding.ASCII.GetBytes(data));
            _error.Text = "";
            CallBot.Text = "Sent";
            await Task.Delay(1000);
            CallBot.IsEnabled = true;
            CallBot.Text = "Summon";
        }
        catch (Exception ex)
        {
            CallBot.Text = "Failed";
            _error.Text = "Confirm bluetooth is enabled and device is paired\nError: " + ex.Message;
            _status.Text = (_status.Text == "Ready") ? "Disconnected" : _status.Text;
            uuid = null;
            _characteristic = null;
            await Task.Delay(1000);
            CallBot.IsEnabled = true;
            CallBot.Text = "Retry";
        }
    }

    

    async Task RequestAccess()
    {
        var locStatus = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
        if (locStatus != PermissionStatus.Granted)
            await Permissions.RequestAsync<Permissions.LocationAlways>();

    }
    private async void CopyToClip(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(_recent);
    }

    async Task ConnectMQTT()
    {
        var mqttFactory = new MqttFactory();

        using (var mqttClient = mqttFactory.CreateMqttClient())
        {
            // Use builder classes where possible in this project.
            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(_addr.Text).Build();

            // This will throw an exception if the server is not available.
            // The result from this message returns additional data which was sent 
            // from the server. Please refer to the MQTT protocol specification for details.
            var response = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
            _status.Text = "The MQTT client is connected.";

            // Send a clean disconnect to the server by calling _DisconnectAsync_. Without this the TCP connection
            // gets dropped and the server will handle this as a non clean disconnect (see MQTT spec for details).
            var mqttClientDisconnectOptions = mqttFactory.CreateClientDisconnectOptionsBuilder().Build();

            await mqttClient.DisconnectAsync(mqttClientDisconnectOptions, CancellationToken.None);
        }
    }

}


/*private string botInfo()
{
    initContact();
    return rasp.ClassOfDevice.Value + " " + rasp.DeviceName + " " + rasp.DeviceAddress;
}

private void initContact()
{
    string result = contactBT("{\"init\":" + DeviceInfo.Current.Model + "}");

    //if (result.Substring(0, 1) == "0")
    //	freeSp2.Text = "Connecting...";
    //else freeSp2.Text = string.Concat("Could not connect\n", result.AsSpan(1));

    //freeSp2.Text = BluetoothService.SerialPort.ToByteArray();
    /*foreach (byte b in BluetoothService.SerialPort.ToByteArray())

    {
        freeSp2.Text += b;
    }
    //freeSp2.IsEnabled = true;
}

private string contactBT(string msg)
{
    try
    {
        //client.Connect(rasp.DeviceAddress, );

        Stream stream = client.GetStream();

        if (stream == null)
        {
            return "1Stream is null";
        }

        byte[] buffer = Encoding.ASCII.GetBytes(msg);

        stream.Write(buffer, 0, buffer.Length);

        return "0";
    }
    catch (Exception e)
    {
        return "1" + e.Message;
    }
}*/

/*private async void BTListen(object sender, EventArgs e)
	{
		await Task.Delay(1000);
		byte[] buffer = new byte[1024];
		try
		{
            client.Connect(raspAdr, BluetoothService.SerialPort);

            Stream stream = client.GetStream();
			int bytesread;

            while ((bytesread = stream.Read(buffer, 0, buffer.Length)) > 0)
			{
				string recieved = Encoding.UTF8.GetString(buffer, 0, bytesread);
				freeSp3.Text = recieved;
			}
		} 
		catch (Exception ex)
		{
			freeSp3.Text = ex.Message;
		}
	}*/
/*foreach (var d in adapter.BondedDevices)
{
    if (d.ToString().Equals("rasppi"))
    {
        freeSp.Text = "Found Rasppi";
        try
        {
            await adapter.ConnectToDeviceAsync(d);
            freeSp.Text += "\n" + d.BondState;
            freeSp2.Text = "Service Name\nCharacteristic Name : Can Write : Can Read";
            foreach (var s in await d.GetServicesAsync())
            {
                freeSp3.Text += s.Name + "\n";
                foreach (var c in await s.GetCharacteristicsAsync())
                {
                    freeSp3.Text += c.Uuid + " : " + c.CanWrite + " : " + c.CanRead + "\n";
                }
            }
        }catch (Exception ex)
        {
            freeSp3.Text = "Error: " + ex.Message;
        }
    }
}*/



/*private async Task FindUUID()
{
    foreach (var d in adapter.BondedDevices)
    {
        if (d.ToString().Equals("rasppi"))
        {
            try
            {
                extra.Text = "";
                await adapter.ConnectToDeviceAsync(d);
                foreach (var s in await d.GetServicesAsync())
                {
                    foreach (var c in await s.GetCharacteristicsAsync())
                    {
                        if (c.CanRead && c.CanWrite)
                        {
                            _status.Text = "Ready";
                            uuid = c.Uuid;
                            _characteristic = c;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}

    /*BluetoothDeviceInfo[] devices = client.PairedDevices.ToArray();

    foreach (BluetoothDeviceInfo device in devices)
    {
        freeSp3.Text += device.DeviceName + "\n";
        System.Diagnostics.Debug.WriteLine("Target : " + device.DeviceAddress);
        if (device.DeviceName.Equals("rasppi"))
        {

            freeSp2.Text = "Connecting";

            //await client.ConnectAsync(rasp.DeviceAddress, Guid.Parse("be6ffa65-4636-4f02-94db-0c7326f85a02"));

            try
            {
                await client.ConnectAsync(rasp.DeviceAddress, Guid.Parse("13e2afc2-6ad3-11ee-b962-0242ac120002"));

                Stream stream = client.GetStream();
                freeSp2.IsEnabled = true;
                if (stream == null)
                {
                    freeSp.Text = "Null stream value";
                }
                else
                {
                    freeSp.Text = "Might've worked";
                }

                byte[] buffer = Encoding.ASCII.GetBytes("Hello World");

                stream.Write(buffer, 0, buffer.Length);
                stream.Dispose();

                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                freeSp3.Text = "Error: " + ex.Message;
            }

            /*
            var service = await device.GetRfcommServicesAsync();
            foreach(Guid u in service)
            {
                freeSp.Text += u + "\n";
            }
        }
    }




}*/

