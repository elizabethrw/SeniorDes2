using MQTTnet.Client;
using MQTTnet;
using System.Linq.Expressions;
using System.Text;
using System.Net.WebSockets;
using Newtonsoft.Json;
using Android.App;

namespace MobileTrashBin;

public class RequestInfo
{
    public string sender = DeviceInfo.Current.Name;
    public string type { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
    public Dictionary<string, int> data { get; set; }
}

public partial class MainPage : ContentPage
{


    static private MqttFactory mqttFactory = new MqttFactory();
    static private IMqttClient _mqtt = mqttFactory.CreateMqttClient();

    ContentPage ControlsPage = new Controls();

    public MainPage()
    {
        InitializeComponent();
        RequestAccess();

        string default_addr = "192.168.0.104";
        _addr.Text = default_addr;
        ConnectMQTT();


        _mqtt.ConnectedAsync += ConnectionEstablished;
        _mqtt.DisconnectedAsync += DisconnectedClient;
        _mqtt.ApplicationMessageReceivedAsync += NotificationReceived;

    }


    int _count = 0;
    string _recent = null;
    private async void SummonRequest(object sender, EventArgs e)
    {
        try
        {
            CallBot.Text = "Sending...";
            CallBot.IsEnabled = false;
            Location location = await Geolocation.GetLocationAsync(new GeolocationRequest()
            {
                DesiredAccuracy = GeolocationAccuracy.Best,
                Timeout = TimeSpan.FromSeconds(30)
            });
            if (location != null)
                _recent = $"{location.Latitude}, {location.Longitude}";

            _count++;

            _tx_box.Text += $"\nLocation {_count}:\n{_recent}\n";

            RequestInfo unformatted = new RequestInfo();
            unformatted.type = "summon";
            unformatted.latitude = location.Latitude;
            unformatted.longitude = location.Longitude;
            
            await SendBot(JsonConvert.SerializeObject(unformatted));

            _error.Text = "";
            CallBot.Text = "Sent";
            await Task.Delay(1000);
            CallBot.IsEnabled = true;
            CallBot.Text = "Summon";
        }
        catch (Exception ex)
        {
            CallBot.Text = "Failed";
            _error.Text = "Error: " + ex.Message;
            _status.Text = "Disconnected";
            await Task.Delay(1000);
            CallBot.IsEnabled = true;
            CallBot.Text = "Retry";
        }
    }

    private void viewControls(object sender, EventArgs e)
    {
        Navigation.PushAsync(ControlsPage);
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

    public async static Task SendBot(string msg)
    {
        try
        {
            var appMsg = new MqttApplicationMessageBuilder().WithTopic("thread/pi").WithPayload(msg).Build();
            await _mqtt.PublishAsync(appMsg);
        } catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }
        
    private Task ConnectionEstablished(MqttClientConnectedEventArgs arg)
    {
        _status.Text = "Connected";
        return Task.CompletedTask;
    }

    private Task DisconnectedClient(MqttClientDisconnectedEventArgs arg)
    {
        _status.Text = "Disconnected";
        return Task.CompletedTask;
    }

    private Task NotificationReceived(MqttApplicationMessageReceivedEventArgs arg)
    {
        _rcv_box.Text += $"\n{arg.ClientId}: \n{Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment)}";
        return Task.CompletedTask;
    }

    private async void Reconnect(object sender, EventArgs e)
    {
        await ConnectMQTT();
    }

    async Task ConnectMQTT()
    {
        try
        {
            _error.Text = "";
            _recon.IsEnabled = false;

            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(_addr.Text).Build();
            await _mqtt.ConnectAsync(mqttClientOptions, tokenSource.Token);

            var mqttSubOptions = new MqttClientSubscribeOptionsBuilder().WithTopicFilter($"thread/{DeviceInfo.Current.Name}").Build();
            await _mqtt.SubscribeAsync(mqttSubOptions);
        }
        catch (Exception ex)
        {
            _error.Text = ex.Message;
        }
            
    }

    private void enableRecon()
    {
        if (_recon.IsEnabled) return;
        _recon.IsEnabled = true;
    }

    private async void _turn_off(object sender, EventArgs e) {
        var confirmCall = DisplayAlert(
          "Shutdown Raspberry Pi",
          "You would have to manually turn the raspberry pi back on to reinstate communication\nWould you like to continue?",
          "Yes",
          "No");
        if (!(await confirmCall))
            return;

        RequestInfo unformatted = new RequestInfo();
        unformatted.type = "shutdown";
        await SendBot(JsonConvert.SerializeObject(unformatted));
        enableRecon();
        await _mqtt.DisconnectAsync();
    }

    private void _addr_TextChanged(object sender, TextChangedEventArgs e)
    {
        enableRecon();
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

