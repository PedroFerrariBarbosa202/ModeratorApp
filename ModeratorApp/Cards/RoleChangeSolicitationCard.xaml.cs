using ModeratorApp.Content.View;
using ModeratorApp.Models;
using ModeratorApp.Services;

namespace ModeratorApp.Cards;

public partial class RoleChangeSolicitationCard : ContentView
{
    List<Models.VolunteerSector> solicitations;

    public RoleChangeSolicitationCard(List<Models.VolunteerSector> _solicitations)
	{
        solicitations = _solicitations;
		InitializeComponent();
		InitName(solicitations);
		InitTags(solicitations);

    }
    private async Task InitName(List<Models.VolunteerSector> solicitations) {
        if (solicitations == null || solicitations.Count == 0)
            return;

        int volunteerId = solicitations[0].volunteer_ID; 

        var response = await DatabaseConnector.Client
            .From<Models.Volunteer>()
            .Where(v => v.volunteer_ID == volunteerId)
            .Single();

        Name.Text = response?.name ?? "NONE";
    }

    private async void InitTags(List<Models.VolunteerSector> solicitations) {
		foreach(var sector in solicitations) {
            var response = await DatabaseConnector.Client
              .From<Models.Sector>()
			  .Where(v => v.sector_ID == sector.sector_ID)
              .Single();

			if (response == null)
				continue;

			Tag tag = new Tag(response);
            TagStack.Children.Add(tag);
		}
    }

    private async void ValidateRequest(object sender, EventArgs e) {
        if (solicitations == null || solicitations.Count == 0)
            return;

        int volunteerId = solicitations[0].volunteer_ID;

        foreach (var sector in solicitations) {
            var response = await DatabaseConnector.Client
              .From<Models.VolunteerSector>()
              .Where(v => v.volunteer_ID == volunteerId)
              .Where(v => v.sector_ID == sector.sector_ID)
              .Set(v => v.is_validated, true)
              .Update();
        }
    }
}