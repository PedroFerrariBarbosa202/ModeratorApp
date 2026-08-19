using ModeratorApp.Models;
using ModeratorApp.Services;

namespace ModeratorApp.Content.View;

public partial class AccountDetailsView : ContentView
{
    Models.Volunteer volunteer_data = new Models.Volunteer();
    public List<Sector> TagsConnected = new();

    public AccountDetailsView(Models.Volunteer _volunteer_data) {
        volunteer_data = _volunteer_data;
        InitializeComponent();
        RefreshUI();
    }

    public void RefreshUI() {
        InitData();
        InitSectors();
    }

    private void InitData() {
        NameLabel.Text = $"Nome: {volunteer_data.name}";
        AgeLabel.Text = $"Idade: {volunteer_data.age.ToString()}";
        EmailLabel.Text = $"Email: {volunteer_data.email}";
        PhoneLabel.Text = $"Telefone: {volunteer_data.phone}";
        ProfessionLabel.Text = $"Emprego: {volunteer_data.profession}";
        CompanyLabel.Text = $"Empresa: {volunteer_data.company}";

        if (volunteer_data.user_img != string.Empty)
            UserImage.Source = ImageService.BytesToImageSource(Convert.FromBase64String(volunteer_data.user_img));
    }

    private async void InitSectors() {
        // loading overlay
        UserAccountsPage? parentPage = (UserAccountsPage?)GetParentPage();
        if(parentPage != null) OverlayManager.SetLoadingOverlay(parentPage.GetContentGrid());

        // delete all tags currently in the sector sections
        ConnectedSectors.Children.Clear();
        ToChooseSectors.Children.Clear();
        TagsConnected.Clear();


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

            TagsConnected.Add(sector);

            // create and integrate tag object
            Tag tag = new Tag(sector, volunteer_data, false);
            ConnectedSectors.Children.Add(tag);
        }

        // fill in all other tags that the user is not connected

        // get data from database
        await DatabaseConnector.InitializeAsync();
        var response = await DatabaseConnector.Client
                .From<Models.Sector>()
                .Get();

        foreach (var sector in response.Models) {
            Tag tag = new Tag(sector, volunteer_data, false);

            if (!TagsConnected.Any(t => t.sector_ID == sector.sector_ID)) {
                ToChooseSectors.Children.Add(tag);
            }
        }

        if (parentPage != null)
            OverlayManager.RemoveLoadingOverlay(parentPage.GetContentGrid());

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