namespace ModeratorApp.Cards;

using Microsoft.Data.SqlClient;
using Microsoft.Maui.Controls;
using ModeratorApp.Services;
using System.Diagnostics;
using TEST_APP.Services;

public partial class ClientCard : ContentView {
    private CardManager.ClientData client_data;
    private CardManager.EventData event_data;

    private bool _isExpanded = false;

    public ClientCard(CardManager.ClientData c_data, CardManager.EventData e_data) {
        InitializeComponent();
        client_data = c_data;
        event_data = e_data;

        BindingContext = c_data;
        ClientDataLabel.Text = @$"Nome: {client_data.name}
Idade: {client_data.age}
Email: {client_data.email}";
        //UserImage.Source = ImageSource.FromStream(() => new MemoryStream(c_data.user_img));
    }

    public VerticalStackLayout RoleStackLayout => RoleStack;

    private void OnExpandClicked(object sender, EventArgs e) {
        if (!_isExpanded) {
            RootBorder.MaximumHeightRequest = 500;
            _isExpanded = !_isExpanded;
        }
        else {
            RootBorder.MaximumHeightRequest = 80;
            _isExpanded = !_isExpanded;
        }
    }

    private async void OnRemoveClicked(object sender, EventArgs e) {
        if (sender is Button btn) {
            if (!event_data.Equals(default(CardManager.EventData)) && btn.BackgroundColor == Colors.Red) {
                await DatabaseConnector.InitializeAsync();
                await DatabaseConnector.Client
                    .From<Models.VolunteerEvent>()
                    .Where(v => v.volunteer_ID == client_data.client_id)
                    .Where(v => v.event_ID == event_data.event_id)
                    .Delete();

                btn.BackgroundColor = Colors.Gray;
                btn.Text = "Removed";
            }
        }
    }
}
