using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;

namespace MobileTrashBin;

public partial class MainPage : ContentPage
{

    public MainPage()
	{
		InitializeComponent();
		RequestAccess();

		BluetoothClient client = new BluetoothClient();
		BluetoothDeviceInfo[] devices = client.PairedDevices.ToArray();

		foreach (BluetoothDeviceInfo device in devices)
		{
			freeSp2.Text += device.DeviceName + "\n";
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

	