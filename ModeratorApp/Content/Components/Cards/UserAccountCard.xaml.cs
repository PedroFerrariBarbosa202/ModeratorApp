using ModeratorApp.Services;
using System.ComponentModel.DataAnnotations;

namespace ModeratorApp.Cards;

public partial class UserAccountCard : ContentView
{
    Models.Volunteer volunteer_data = new Models.Volunteer();

    public UserAccountCard(Models.Volunteer _volunteer_data)
	{
        volunteer_data = _volunteer_data;
        InitializeComponent();
		InitData(_volunteer_data);
	}

	private void InitData(Models.Volunteer volunteer_data) {
		VolunteerName.Text = volunteer_data.name;

        if(volunteer_data.user_img != string.Empty)
            UserImage.Source = ImageService.BytesToImageSource(Convert.FromBase64String(volunteer_data.user_img));

		if (volunteer_data.is_validated) {
			StatusLabel.Text = "Validada";
			StatusLabel.TextColor = Colors.Green;
		} else if (!volunteer_data.is_validated) {
            StatusLabel.Text = "Não Validada";
            StatusLabel.TextColor = Colors.Yellow;
        }
    }

	private void OnViewAccountClicked(object sender, EventArgs e) {
		var page = GetParentPage() as UserAccountsPage;
        page.ShowAccountDetailView(volunteer_data);

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