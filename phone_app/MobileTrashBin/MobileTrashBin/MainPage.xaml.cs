using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using InTheHand.Net;

namespace MobileTrashBin;

public partial class MainPage : ContentPage
{

	BluetoothClient client = new BluetoothClient();
	BluetoothDeviceInfo rasp = null;
	// Bluetooth Address: DCA6322171C1 (Byte array is inveresed "C1", "71", "21", "32", "A6", "DC")
	static byte[] addr = {193, 113, 33, 50, 166, 220};
    BluetoothAddress raspAdr = new BluetoothAddress(addr);

	private static byte[] StringToByteArray(string str)
	{
		return Enumerable.Range(0, str.Length)
			.Where(x => x % 2 == 0).Select(x => Convert.ToByte(str.Substring(x, 2), 16)).ToArray();
	}

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
		if (rasp != null)
        {
            freeSp.Text = rasp.ClassOfDevice.Value + " " + rasp.DeviceName + " " + rasp.DeviceAddress;
		}
		else
		{
			freeSp.Text = "Could not find Device";
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

    async Task RequestAccess()
    {
		var locStatus = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
		if (locStatus != PermissionStatus.Granted)
			await Permissions.RequestAsync<Permissions.LocationAlways>();
		


    }

	
	

}

	