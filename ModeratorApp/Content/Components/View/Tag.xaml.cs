using ModeratorApp.Models;
using ModeratorApp.Services;

namespace ModeratorApp.Content.View;

public partial class Tag : ContentView
{
	Models.Sector sector;
    Models.Volunteer volunteer;

    public Tag(Models.Sector _sector, Models.Volunteer? _volunteer, bool isInputTransparent)
	{
        sector = _sector;
        volunteer = _volunteer;
		InitializeComponent();

        // init tag
        NameTag.Text = _sector.name;
        Border.Stroke = Color.FromArgb(_sector.color);
        ThisTag.InputTransparent = isInputTransparent;
    }

    private async void OnTagClicked(object sender, TappedEventArgs e) {
        // check if user is in specific sector
        VolunteerSector? table = await DatabaseConnector.Client
               .From<Models.VolunteerSector>()
               .Where(x => x.volunteer_ID == volunteer.volunteer_ID)
               .Where(x => x.sector_ID == sector.sector_ID)
               .Single();

        if (table != null) {

            // if volunteer is in sector, remove it
            await DatabaseConnector.Client
               .From<Models.VolunteerSector>()
               .Where(x => x.volunteer_ID == volunteer.volunteer_ID)
               .Where(x => x.sector_ID == sector.sector_ID)
               .Delete();
        }
        else {
            // instantiate new table entry to add to database
            VolunteerSector newVolunteerSector = new VolunteerSector {
                volunteer_ID = volunteer.volunteer_ID,
                sector_ID = sector.sector_ID,
                is_validated = true
            };

            // if volunteer not in sector, add it
            await DatabaseConnector.Client
               .From<Models.VolunteerSector>()
               .Insert(newVolunteerSector);
        }

        // reset ui in AccountDetailsView
        AccountDetailsView? acc = GetAccountDetailsView();
        if (acc != null)
            acc.RefreshUI();
    }

    private AccountDetailsView? GetAccountDetailsView() {
        Element parent = this;

        while (parent != null) {
            if (parent is AccountDetailsView page)
                return page;

            parent = parent.Parent;
        }

        return null;
    }
}