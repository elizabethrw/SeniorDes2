namespace MobileTrashBin;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		CallBot.Text = "Not Ready";
	}
}

