using Microsoft.Data.SqlClient;
using ModeratorApp.Services;
using TEST_APP.Services;
namespace ModeratorApp.Cards;

public partial class RoleManageCard : ContentView
{
    Models.Roles role_data;

    public RoleManageCard(Models.Roles _role_data)
	{
        InitializeComponent();
        role_data = _role_data;
        BindingContext = _role_data;
    }

    private async void OnCloseClicked(object sender, EventArgs e) {
        await DatabaseConnector.Client
           .From<Models.Roles>()
           .Where(v => v.name == role_data.name)
           .Delete();

        // destroy this object
        if (this.Parent is Layout parentLayout) {
            parentLayout.Children.Remove(this);
        }
    }
}