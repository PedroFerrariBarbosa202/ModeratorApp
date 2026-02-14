using ModeratorApp.Services;
using ModeratorApp.Content.View;
using TEST_APP.Services;

namespace ModeratorApp.Cards;

public partial class SolicitationCard : ContentView {
    // client data and the data needed for solicitations are similar,
    // so the same data structure is used
    CardManager.ClientData data;
    public SolicitationCard(CardManager.ClientData _data) {
        data = _data;
        InitializeComponent();
        InitializeFields();
        _ = InitSectors();

    }

    void InitializeFields() {
        Name.Text = $"Nome: {data.name}";
        Age.Text = $"Idade: {data.age.ToString()}";
        Email.Text = $"Email: {data.email}";
        Password.Text = $"Senha: {data.password}";
    }

    private async Task InitSectors() {
        var response = await DatabaseConnector.Client
              .From<Models.VolunteerSector>()
              .Where(v => v.volunteer_ID == data.client_id)
              .Get();

        foreach (var connection in response.Models) {
            // get sector by id
            var sector_response = await DatabaseConnector.Client
              .From<Models.Sector>()
              .Where(v => v.sector_ID == connection.sector_ID)
              .Single();

            var sec_data = new CardManager.TagData {
                tag_id = sector_response.sector_ID,
                name = sector_response.name,
                color = Color.FromArgb(sector_response.color),
            };

            Tag tag = new Tag(sec_data);
            TagStack.Children.Add(tag);
        }
    }

    private async void OnVerifyButtonClicked(object sender, EventArgs e) {
        // verify the account
        await DatabaseConnector.Client
              .From<Models.Volunteer>()
              .Where(v => v.volunteer_ID == data.client_id)
              .Set(x => x.is_validated, true)
              .Update();

        if (Shell.Current.CurrentPage is SolicitationPage solicitationPage) {
            solicitationPage.InitializeSolicitations();
        }
    }
}