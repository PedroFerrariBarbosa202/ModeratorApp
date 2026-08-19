using ModeratorApp.Services;
namespace ModeratorApp.Cards;

public partial class EventCard : ContentView
{
    private Models.Events event_data;

    public EventCard(Models.Events e_data) {
        InitializeComponent();
        event_data = e_data;

        BindingContext = e_data;

        // add color to border]
        RootBorder.Stroke = GetRandomColor();
    }

    public async void ViewEvent(object sender, EventArgs e) {
        var button = (Button)sender;

        await button.ScaleTo(0.8, 60, Easing.Linear);
        await button.ScaleTo(1.0, 60, Easing.Linear);

        // goto next page
        await Navigation.PushAsync(new EventPage(event_data));

    }
    public async void RemoveEvent(object sender, EventArgs e) {
        if (sender is Button btn) {
            if (!event_data.Equals(default(Models.Events)) && btn.BackgroundColor == Colors.Red) {
                try {
                    await DatabaseConnector.InitializeAsync();

                    // remove event connection from volunteer_event
                    await DatabaseConnector.Client
                        .From<Models.VolunteerEvent>()
                        .Where(v => v.event_ID == event_data.event_ID)
                        .Delete();

                    // remove event connection to event_role
                    await DatabaseConnector.Client
                        .From<Models.EventRole>()
                        .Where(v => v.event_ID == event_data.event_ID)
                        .Delete();

                    // remove event
                    await DatabaseConnector.Client
                        .From<Models.Events>()
                        .Where(v => v.event_ID == event_data.event_ID)
                        .Delete();

                    btn.BackgroundColor = Colors.Gray;
                    btn.Text = "Removido";
                }
                catch (Exception) {
                    Application.Current.MainPage.DisplayAlert("Erro", "Não foi possível remover o evento.", "OK");
                }
            }
        }
    }
    private Color GetRandomColor() {
        var random = new Random();
        return Color.FromRgb(random.Next(100, 256), random.Next(100, 256), random.Next(100, 256));
    }
}