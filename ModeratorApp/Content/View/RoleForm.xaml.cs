using Microsoft.Data.SqlClient;
using Microsoft.Maui.Storage;
using ModeratorApp.Services;
using System.Data;
using TEST_APP.Services;
using static ModeratorApp.Services.CardManager;

namespace ModeratorApp;

public partial class RoleForm : ContentView
{
	public RoleForm()
	{
		InitializeComponent();
		ShowRoles();
	}

	async void ShowRoles() {
        // remove all current elements
        RoleStack.Children.Clear();
        await DatabaseConnector.InitializeAsync();

        // get all roles
        var roles = await DatabaseConnector.Client
            .From<Models.Roles>()
            .Get();

        foreach (Models.Roles row in roles.Models) {
            var role_data = new CardManager.RoleData {
                role_id = row.role_ID,
                name = row.name,
                color = GetRandomColor().ToHex()
            };

            CardManager.add_role_manage(role_data, RoleStack);
        }
	}

    async void AddRole(object sender, EventArgs e) {
        if (RoleEntry.Text == null)
            return;

        var role = new Models.Roles {
            name = RoleEntry.Text
        };

        await DatabaseConnector.Client
                 .From<Models.Roles>()
                 .Insert(role);

        ShowRoles();
    }
    private void OnCloseClicked(object sender, EventArgs e) {
        if (this.Parent is Layout parentLayout) {
            parentLayout.Children.Remove(this);
        }
    }

    private Color GetRandomColor() {
        var random = new Random();
        return Color.FromRgb(random.Next(100, 256), random.Next(100, 256), random.Next(100, 256));
    }
}