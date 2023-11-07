using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MobileTrashBin;

public partial class Controls : ContentPage
{
    public Controls()
    {
        InitializeComponent();
        //var mainpage = Application.Current.MainPage as MainPage;
        //mainpage?.SendBot("test");
    }

    int left_motor = 0;
    int right_motor = 0;
    List<Button> clicked = new List<Button>();

    private async void SpeedSet(object sender, EventArgs e)
    {
        /*
         * _l = left motor
         * _r = right motor
         * _m = move front/back
         * _t = tilt
         */
        var s = (Button)sender;

        RequestInfo requestInfo = new RequestInfo();

        requestInfo.type = s.ClassId;

        requestInfo.type = "controls";
        Dictionary<string, int> speed = new Dictionary<string, int>();

        switch (s.ClassId[s.ClassId.Length-1])
        {
            case 'l':
                Clear("r");
                left_motor = Int32.Parse(s.Text);
                break;

            case 'r':
                Clear("l");
                right_motor = Int32.Parse(s.Text);
                break;

            case 'm':
                Clear();
                right_motor = Int32.Parse(s.Text);
                left_motor = Int32.Parse(s.Text);
                break;

            case 't':
                Clear();
                right_motor = Int32.Parse(s.Text)*-1;
                left_motor = Int32.Parse(s.Text);
                break;
        }

        speed.Add("right", right_motor);
        speed.Add("left", left_motor);

        requestInfo.data = speed;

        s.BackgroundColor = Colors.ForestGreen;
        clicked.Add(s);
        s.IsEnabled = false;


        await MainPage.SendBot(JsonConvert.SerializeObject(requestInfo));
    }

    private async void StopMovement(object sender, EventArgs e)
    {
        Clear();
        RequestInfo requestInfo = new RequestInfo();
        requestInfo.type = "controls";
        Dictionary<string, int> speed = new Dictionary<string, int>();
        speed.Add("right", 0);
        speed.Add("left", 0);
        requestInfo.data = speed;
        await MainPage.SendBot(JsonConvert.SerializeObject(requestInfo));
    }

    private void Clear(string filter = "a")
    {
        foreach (Button b in clicked)
        {
            if(filter != "a" && !(b.ClassId.EndsWith(filter)))
            {
                b.BackgroundColor = Colors.White;
                b.IsEnabled = true;
            }
            if(filter == "a")
            {
                b.BackgroundColor = Colors.White;
                b.IsEnabled = true;
                right_motor = 0;
                left_motor = 0;
            }
        }
    }
}