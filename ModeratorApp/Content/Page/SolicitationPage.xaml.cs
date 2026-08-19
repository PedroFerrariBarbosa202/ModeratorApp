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
            OverlayManager.SetLoadingOverlay(ContentGrid);

            await DatabaseConnector.InitializeAsync();

            // clear all children first
            SolicitationStack.Children.Clear();

            if (FilterPicker.SelectedIndex == 0) {
                AddAccountValidationCard();
            }
            else if (FilterPicker.SelectedIndex == 1) {
                AddAccountValidationCard();
            }

            OverlayManager.RemoveLoadingOverlay(ContentGrid);
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

    private void OnPickerChanged(object sender, EventArgs e) {
        InitializeSolicitations();
    }
}