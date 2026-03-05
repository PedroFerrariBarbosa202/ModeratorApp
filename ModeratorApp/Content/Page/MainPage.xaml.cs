
using ModeratorApp.Cards;
using ModeratorApp.Content;
using ModeratorApp.Services;


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
        Loading loading_page = new Loading();

        try {
            // add loading screen
            ContentGrid.Children.Add(loading_page);

            await DatabaseConnector.InitializeAsync();
            var response = await DatabaseConnector.Client
              .From<Models.Events>()
              .Get();

            foreach (Models.Events row in response.Models) {
                EventCard event_card = new EventCard(row);
                EventStackLayout.Children.Add(event_card);
            }

            // remove loading
            ContentGrid.Children.Remove(loading_page);
        }
        catch(Exception ex){
            // remove loading
            ContentGrid.Children.Remove(loading_page);

            await DisplayAlert("Erro detectado", ex.Message, "Continuar");
        }
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