using ModeratorApp.Cards;
using ModeratorApp.Services;
using TEST_APP.Services;

namespace ModeratorApp;

public partial class SolicitationPage : ContentPage
{
    public SolicitationPage() {
        InitializeComponent();
        InitializeSolicitations();
    }

    public async void InitializeSolicitations() {
        try {
            await DatabaseConnector.InitializeAsync();

            // clear all children first
            SolicitationStack.Children.Clear();

            var response = await DatabaseConnector.Client
              .From<Models.Volunteer>()
              .Where(v => v.is_validated == false)
              .Get();

            foreach (Models.Volunteer row in response.Models) {
                var card = new SolicitationCard(row);
                SolicitationStack.Children.Add(card);

                // update solicitation so it appears as seen to the user
                await DatabaseConnector.Client
                  .From<Models.Volunteer>()
                  .Where(v => v.volunteer_ID == row.volunteer_ID)
                  .Set(x => x.solicitation_seen, true)
                  .Update();
            }
        }
        catch (Exception ex) {
            await DisplayAlert("Erro detectado", ex.Message, "Continuar");
        }
    }
}