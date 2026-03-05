using ModeratorApp.Cards;
using ModeratorApp.Content;
using ModeratorApp.Services;

namespace ModeratorApp;

public partial class SolicitationPage : ContentPage {
    public SolicitationPage() {
        InitializeComponent();
        InitializeSolicitations();
    }

    public async void InitializeSolicitations() {
        try {
            Loading loading_page = new Loading();
            ContentGrid.Children.Add(loading_page);

            await DatabaseConnector.InitializeAsync();

            // clear all children first
            SolicitationStack.Children.Clear();

            if (FilterPicker.SelectedIndex == 0) {
                AddAccountValidationCard();
                AddSectorChangeCard();
            }
            else if (FilterPicker.SelectedIndex == 1) {
                AddAccountValidationCard();
            }
            else if (FilterPicker.SelectedIndex == 2) {
                AddSectorChangeCard();
            }

            ContentGrid.Children.Remove(loading_page);
        }
        catch (Exception ex) {
            await DisplayAlert("Erro detectado", ex.Message, "Continuar");
        }
    }

    private async void AddAccountValidationCard() {
        // Account creation cards
        var account_change_solicitation = await DatabaseConnector.Client
            .From<Models.Volunteer>()
            .Where(v => v.is_validated == false)
            .Get();

        foreach (Models.Volunteer row in account_change_solicitation.Models) {
            var card = new SolicitationCard(row);
            SolicitationStack.Children.Add(card);
        }

        // make all solicitation 'seen'
        await DatabaseConnector.Client
            .From<Models.Volunteer>()
            .Where(v => v.is_validated == false)
            .Set(v => v.solicitation_seen, true)
            .Update();
    }

    private async void AddSectorChangeCard() {
        // Sector change cards
        var sector_change_solicitation = await DatabaseConnector.Client
            .From<Models.VolunteerSector>()
            .Where(v => v.is_validated == false)
            .Get();

        var groupedByUser = sector_change_solicitation.Models
            .GroupBy(s => s.volunteer_ID);

        foreach (var userGroup in groupedByUser) {
            var userSolicitations = userGroup.ToList();

            var card = new RoleChangeSolicitationCard(userSolicitations);
            SolicitationStack.Children.Add(card);
        }
    }

    private void OnPickerChanged(object sender, EventArgs e) {
        InitializeSolicitations();
    }
}