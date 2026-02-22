using Microsoft.Data.SqlClient;
using ModeratorApp.Services;
using System.Diagnostics;
using TEST_APP.Services;

namespace ModeratorApp.Cards;

public partial class RoleLimitCard : ContentView {
    private Models.Roles role_data;
    private Models.Events event_data;
    public string RoleName { get; private set; }

    public RoleLimitCard(Models.Roles _role_data, Models.Events _event_data) {
        InitializeComponent();
        role_data = _role_data;
        event_data = _event_data;
        BindingContext = role_data;

        SetRoleLimit();
    }

    private async void SetRoleLimit() {
        // get role limit
        var role = await DatabaseConnector.Client
          .From<Models.EventRole>()
          .Where(v => v.role_ID == role_data.role_ID)
          .Where(v => v.event_ID == event_data.event_ID)
          .Single();

        if (role == null)
            return;

        NumLimitLabel.Text = $"Vagas disponíveis: {role.number_limit}";
        RoleName = role_data.name;
    }

    private async void OnCloseClicked(object sender, EventArgs e) {
        if (this.Parent is Layout parentLayout) {
            parentLayout.Children.Remove(this);
            await DatabaseConnector.Client
              .From<Models.EventRole>()
              .Where(v => v.role_ID == role_data.role_ID)
              .Where(v => v.event_ID == event_data.event_ID)
              .Delete();
        }
    }
}
