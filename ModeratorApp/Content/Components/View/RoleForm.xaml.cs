using ModeratorApp.Cards;
using ModeratorApp.Services;


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
            var role_manage = new RoleManageCard(row);
            RoleStack.Children.Add(role_manage);
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