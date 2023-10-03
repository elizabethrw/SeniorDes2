using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using InTheHand.Net;
using System.Text;
using System;
using System.Security.Cryptography;

namespace MobileTrashBin;

public partial class MainPage : ContentPage
{

	BluetoothClient client = new BluetoothClient();
	BluetoothDeviceInfo rasp = null;
	// Bluetooth Address: DCA6322171C1 (Byte array is inveresed "C1", "71", "21", "32", "A6", "DC")
	static byte[] addr = { 193, 113, 33, 50, 166, 220 };
	BluetoothAddress raspAdr = new BluetoothAddress(addr);

	public MainPage()
	{
		InitializeComponent();
		RequestAccess();

		BluetoothDeviceInfo[] devices = client.PairedDevices.ToArray();

		foreach (BluetoothDeviceInfo device in devices)
		{
			System.Diagnostics.Debug.WriteLine("Target : " + device.DeviceAddress);
			System.Diagnostics.Debug.WriteLine("Comp : " + raspAdr);
			if (device.DeviceAddress.Equals(raspAdr))
			{
				rasp = device;
				break;
			}
		}
		freeSp.Text = (rasp != null) ? botInfo() : "Please pair the device";

	}

	private string botInfo()
	{
		initContact();
		return rasp.ClassOfDevice.Value + " " + rasp.DeviceName + " " + rasp.DeviceAddress;
	}

	private void initContact()
	{
		string result = contactBT("{\"init\":" + DeviceInfo.Current.Model + "}");

		if (result.Substring(0, 1) == "0")
			freeSp2.Text = "Connecting...";
		else freeSp2.Text = string.Concat("Could not connect\n", result.AsSpan(1));

        freeSp2.IsEnabled = true;
    }

	private string contactBT(string msg)
	{
        try
        {
            client.Connect(raspAdr, BluetoothService.SerialPort);

            Stream stream = client.GetStream();

            byte[] buffer = Encoding.ASCII.GetBytes(msg);

            stream.Write(buffer, 0, buffer.Length);

			return "0";
        }
        catch (Exception e)
        {
			return "1" + e.Message;
        }
    }

    private async void OnCounterClicked(object sender, EventArgs e)
	{
		CallBot.Text = "Sent";
        CallBot.IsEnabled = false;
		await Task.Delay(1000);
		CallBot.IsEnabled = true;
		CallBot.Text = "Summon";
    }

	private async void BTListen(object sender, EventArgs e)
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
	}

    async Task RequestAccess()
    {
		var locStatus = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
		if (locStatus != PermissionStatus.Granted)
			await Permissions.RequestAsync<Permissions.LocationAlways>();
		


    }

	
	

}

	