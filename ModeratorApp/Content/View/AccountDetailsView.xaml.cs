using ModeratorApp.Content.View;
using ModeratorApp.Models;
using TEST_APP.Services;

namespace ModeratorApp.Content.View;

public partial class AccountDetailsView : ContentView
{
    Models.Volunteer volunteer_data = new Models.Volunteer();

    public AccountDetailsView(Models.Volunteer _volunteer_data) {
        volunteer_data = _volunteer_data;
        InitializeComponent();
        InitUI();
    }

    private void InitUI() {
        InitData();
        InitSectors();
    }

    private void InitData() {
        NameLabel.Text = $"Nome: {volunteer_data.name}";
        AgeLabel.Text = $"Idade: {volunteer_data.age.ToString()}";
        EmailLabel.Text = $"Email: {volunteer_data.email}";
        PasswordLabel.Text = $"Senha: {volunteer_data.password}";
    }

    private async void InitSectors() {
        // get all sectors a volunteer is connected to
        var volunteer_sectors = await DatabaseConnector.Client
                 .From<Models.VolunteerSector>()
                 .Where(v => v.volunteer_ID == volunteer_data.volunteer_ID)
                 .Get();

        foreach(var volunteer_sector in volunteer_sectors.Models) {
            // get sector data to send to tag object
            var sector = await DatabaseConnector.Client
                 .From<Models.Sector>()
                 .Where(v => v.sector_ID == volunteer_sector.sector_ID)
                 .Single();

            if (sector == null)
                continue;   

            // create and integrate tag object
            Tag tag = new Tag(sector);
            SectorStack.Children.Add(tag);
        }
    }

    private async void OnDeleteAccountClicked(object sender, EventArgs e) {
        // delete account from system
        await DatabaseConnector.Client
             .From<Models.Volunteer>()
             .Where(v => v.volunteer_ID == volunteer_data.volunteer_ID)
             .Delete();

        // update ui to show deleted account
        var page = GetParentPage() as UserAccountsPage; 
        if (page == null) return;

        page.InitUserAccountCards();
        OnCloseClicked(null, null);
    }

    private void OnCloseClicked(object? sender, EventArgs? e) {
        if (this.Parent is Layout parentLayout) {
            parentLayout.Children.Remove(this);
        }
    }

    private Page? GetParentPage() {
        Element parent = this;

        while (parent != null) {
            if (parent is Page page)
                return page;

            parent = parent.Parent;
        }

        return null;
    }
}