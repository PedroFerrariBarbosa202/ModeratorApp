using ModeratorApp.Services;

namespace ModeratorApp;

public partial class EntrancePage : ContentPage
{
	public EntrancePage()
	{
		InitializeComponent();
        CheckSystemActivity();
    }

    private async void CheckSystemActivity() {
        try {
            await DatabaseConnector.InitializeAsync();
            InitUI();
        }
        catch (Exception ex) {
            SystemStatusLabel.Text = ex.Message;
            return;
        }
    }

	async void InitUI() {
        // registered events counter
        var events = await DatabaseConnector.Client
               .From<Models.Events>()
               .Get();

        RegisteredEvents.Text = $"{events.Models.Count}";

        // solicitations counter
        var solicitations = await DatabaseConnector.Client
               .From<Models.Volunteer>()
               .Where(v => v.solicitation_seen == false)
               .Get();

        Solicitations.Text = $"{solicitations.Models.Count}";

        // verified accounts counter
        var verified = await DatabaseConnector.Client
               .From<Models.Volunteer>()
               .Where(v => v.is_validated == true)
               .Get();

        VerifiedAccounts.Text = $"{verified.Models.Count}";

        // sectors counter
        var sectors = await DatabaseConnector.Client
               .From<Models.Sector>()
               .Get();

        Sectors.Text = $"{sectors.Models.Count}";

        // news counter
        var news = await DatabaseConnector.Client
               .From<Models.News>()
               .Get();

        News.Text = $"{sectors.Models.Count}";
    }
}