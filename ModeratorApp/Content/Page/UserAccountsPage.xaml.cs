using ModeratorApp.Cards;
using ModeratorApp.Content;
using TEST_APP.Services;

namespace ModeratorApp;

public partial class UserAccountsPage : ContentPage
{
	public UserAccountsPage()
	{
		InitializeComponent();
        InitUserAccountCards();
    }

	public async void InitUserAccountCards() {
        try {
            // add loading screen
            Loading loading_page = new Loading();
            ContentGrid.Children.Add(loading_page);

            // clear all children first
            UserAccountStack.Children.Clear();

            var response = await DatabaseConnector.Client
              .From<Models.Volunteer>()
              .Get();

            foreach (Models.Volunteer row in response.Models) {
                // add to stack
                var card = new UserAccountCard(row);
                UserAccountStack.Children.Add(card);
            }

            ContentGrid.Children.Remove(loading_page);
        }
        catch (Exception ex) {
            await DisplayAlert("Erro detectado", ex.Message, "Continuar");
        }
    }

    public void ShowAccountDetailView(Models.Volunteer volunteer_data) {
        Content.View.AccountDetailsView view = new Content.View.AccountDetailsView(volunteer_data);
        ContentGrid.Children.Add(view);
    }

}