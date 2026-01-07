using Microsoft.Data.SqlClient;
using ModeratorApp.Services;
using System.Data;
using System.Diagnostics;
using TEST_APP.Services;

namespace ModeratorApp.Cards;

public partial class SubRoleShowCard : ContentView {
    private CardManager.RoleData role_data;
    private CardManager.EventData event_data;
    private CardManager.ClientData client_data;
    public string RoleName { get; private set; }

    public SubRoleShowCard(CardManager.RoleData _role_data, CardManager.EventData _event_data, CardManager.ClientData _client_data) {
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
           .Where(v => v.role_ID == role_data.role_id)
           .Where(v => v.event_ID == event_data.event_id)
           .Where(v => v.volunteer_ID == client_data.client_id)
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
