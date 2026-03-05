using ModeratorApp.Services;
using ModeratorApp.Content.View;

namespace ModeratorApp.Cards;

public partial class SolicitationCard : ContentView {
    // client data and the data needed for solicitations are similar,
    // so the same data structure is used
    Models.Volunteer data;
    public SolicitationCard(Models.Volunteer _data) {
        data = _data;
        InitializeComponent();
        InitializeFields();
        _ = InitSectors();
    }

    void InitializeFields() {
        Name.Text = $"Nome: {data.name}";
        Email.Text = $"Email: {data.email}";
        Password.Text = $"Senha: {data.password}";
    }

    private async Task InitSectors() {
        var response = await DatabaseConnector.Client
              .From<Models.VolunteerSector>()
              .Where(v => v.volunteer_ID == data.volunteer_ID)
              .Get();

        foreach (var connection in response.Models) {
            // get sector by id
            var sector_response = await DatabaseConnector.Client
              .From<Models.Sector>()
              .Where(v => v.sector_ID == connection.sector_ID)
              .Single();

            var sec_data = new Models.Sector {
                sector_ID = sector_response.sector_ID,
                name = sector_response.name,
                color = sector_response.color,
            };

            Tag tag = new Tag(sec_data);
            TagStack.Children.Add(tag);
        }
    }

    private async void OnVerifyButtonClicked(object sender, EventArgs e) {
        // verify the account
        await DatabaseConnector.Client
              .From<Models.Volunteer>()
              .Where(v => v.volunteer_ID == data.volunteer_ID)
              .Set(x => x.is_validated, true)
              .Update();

        if (Shell.Current.CurrentPage is SolicitationPage solicitationPage) {
            solicitationPage.InitializeSolicitations();
        }
    }
}