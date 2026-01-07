using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ModeratorApp.Services;
using System.Data;
using System.Diagnostics;
using TEST_APP.Services;

namespace ModeratorApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        ExecuteQuery();
    }

    private async void ExecuteQuery()
    {
        await DatabaseConnector.InitializeAsync();
        var response = await DatabaseConnector.Client
          .From<Models.Events>()
          .Get();

        foreach (Models.Events row in response.Models)
        {
            var event_data = new CardManager.EventData {
                event_id = row.event_ID,
                name = row.name,
                description = row.description,
                date = row.date.ToString(),
                time_begin = row.time_begin.ToString(),
                time_end = row.time_begin.ToString(),
                link = row.link,
                color = GetRandomColor().ToHex()
            };

            CardManager.add_event(event_data, EventStackLayout);
        }
    }

    private Color GetRandomColor()
    {
        var random = new Random();
        return Color.FromRgb(random.Next(100, 256), random.Next(100, 256), random.Next(100, 256));
    }
    private async void AddEvent(object sender, EventArgs e) {
        var button = (Button)sender;

        await button.ScaleTo(0.8, 60, Easing.Linear);
        await button.ScaleTo(1.0, 60, Easing.Linear);

        EventForm ev_form = new EventForm(EventStackLayout);
        MainGrid.Add(ev_form);
    }

    private async void ManageRole(object sender, EventArgs e) {
        var button = (Button)sender;

        await button.ScaleTo(0.8, 60, Easing.Linear);
        await button.ScaleTo(1.0, 60, Easing.Linear);

        RoleForm role_form = new RoleForm();
        MainGrid.Add(role_form);
    }
}