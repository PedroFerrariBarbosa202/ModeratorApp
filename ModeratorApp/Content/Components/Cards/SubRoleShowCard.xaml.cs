using Microsoft.Data.SqlClient;
using ModeratorApp.Services;
using System.Data;
using System.Diagnostics;

namespace ModeratorApp.Cards;

public partial class SubRoleShowCard : ContentView {
    private Models.Roles role_data;
    private Models.Events event_data;
    private Models.Volunteer client_data;
    public string RoleName { get; private set; }

    public SubRoleShowCard(Models.Roles _role_data, Models.Events _event_data, Models.Volunteer _client_data) {
        InitializeComponent();
        role_data = _role_data;
        event_data = _event_data;
        client_data = _client_data;
        BindingContext = role_data;

        SetLabels();
    }

    private async void SetLabels() {
        var response = await DatabaseConnector.Client
           .From<Models.VolunteerEvent>()
           .Where(v => v.role_ID == role_data.role_ID)
           .Where(v => v.event_ID == event_data.event_ID)
           .Where(v => v.volunteer_ID == client_data.volunteer_ID)
           .Single();

        if (response == null) return;

        DateLabel.Text = $"Data: {response.date.ToString()}";
        TimeBeginLabel.Text = $"Início: {response.time_begin.ToString()}";
        TimeEndLabel.Text = $"Fim: {response.time_end.ToString()}";
        RoleName = role_data.name;
    }

    private void OnCloseClicked(object sender, EventArgs e) {
        if (this.Parent is Layout parentLayout) {
            parentLayout.Children.Remove(this);
        }
    }
}
