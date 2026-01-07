using Microsoft.Data.SqlClient;
using ModeratorApp.Services;
using System.Diagnostics;
using TEST_APP.Services;

namespace ModeratorApp.Cards;

public partial class RoleLimitCard : ContentView {
    private CardManager.RoleData role_data;
    private CardManager.EventData event_data;
    public string RoleName { get; private set; }

    public RoleLimitCard(CardManager.RoleData _role_data, CardManager.EventData _event_data) {
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
          .Where(v => v.role_ID == role_data.role_id)
          .Where(v => v.event_ID == event_data.event_id)
          .Single();

        if (role == null)
            return;

        NumLimitLabel.Text = $"Vagas disponíveis: {role.role_ID}";
        RoleName = role_data.name;
    }

    private void OnCloseClicked(object sender, EventArgs e) {
        if (this.Parent is Layout parentLayout) {
            parentLayout.Children.Remove(this);
        }
    }
}
