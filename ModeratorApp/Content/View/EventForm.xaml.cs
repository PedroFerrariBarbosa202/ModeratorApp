using ModeratorApp.Cards;
using ModeratorApp.Services;

namespace ModeratorApp;

public partial class EventForm : ContentView
{
    VerticalStackLayout layout;


    public EventForm(VerticalStackLayout _layout)
	{
		InitializeComponent();
        InitRolePicker();
        layout = _layout;
	}

    private async void InitRolePicker() {
        // clear picker before init
        RolePicker.Items.Clear();
        await DatabaseConnector.InitializeAsync();

        var events = await DatabaseConnector.Client
              .From<Models.Roles>()
              .Get();

        if (events == null) return;
        foreach (Models.Roles row in events.Models) {
            if (row != null) {
                RolePicker.Items.Add(row.name);
            }
        }
    }

    private void OnRolePickerChange(object sender, EventArgs e) {
        var role_data = new Models.Roles {
            name = RolePicker.SelectedItem?.ToString() ?? "None",
        };

        RoleReadCard role_read_card = new RoleReadCard(role_data);
        RoleStack.Children.Add(role_read_card);
    }

    private async void AddEvent(object sender, EventArgs e) {
        // creating event on database
        // create model for event to add to database
        var ev_model = new Models.Events{
            name = NameEntry.Text ?? "None",
            description = DescriptionEntry.Text ?? "None",
            date = DateOnly.FromDateTime(DateEntry.Date),
            time_begin = TimeOnly.FromTimeSpan(TimeBegin.Time),
            time_end = TimeOnly.FromTimeSpan(TimeEnd.Time),
            link = LinkEntry.Text ?? "None",
        };
        var inserted_event = await DatabaseConnector.Client
            .From<Models.Events>()
            .Insert(ev_model);

        var ev = inserted_event.Models.First();

        if (ev == null) {
            await Application.Current.MainPage.DisplayAlert("Commando SQL não reconhecido", "Coloque dados válidos", "Tentar novamente");
            return;
        }

        // connect roles to event
        foreach(var child in RoleStack.Children) {
            if(child is RoleReadCard r_card) {
                // get role with specific name
                var selected_role = await DatabaseConnector.Client
                 .From<Models.Roles>()
                 .Where(v => v.name == r_card.RoleName)
                 .Single();

                if (selected_role == null)
                    continue;

                // create model to insert into event_roles
                var ev_role_model = new Models.EventRole {
                    event_ID = ev.event_ID,
                    role_ID = selected_role.role_ID,
                    number_limit = r_card.num_limit,
                };

                // insert model
                await DatabaseConnector.Client
                  .From<Models.EventRole>()
                  .Insert(ev_role_model);
            }
        }

        var event_data = new Models.Events{
            event_ID = ev.event_ID,
            name = NameEntry.Text ?? "None",
            description = DescriptionEntry.Text ?? "None",
            date = DateOnly.FromDateTime(DateEntry.Date),
            time_begin = TimeOnly.FromTimeSpan(TimeBegin.Time),
            time_end = TimeOnly.FromTimeSpan(TimeEnd.Time),
            link = LinkEntry.Text ?? "None", 
        };

        EventCard ev_card = new EventCard(event_data);
        layout.Children.Add(ev_card);

        if (this.Parent is Layout parentLayout) {
            parentLayout.Children.Remove(this);
        }

        await DatabaseConnector.Client
        .From<Models.Notifications>()
        .Insert(new Models.Notifications {
            title = "Atualização importante",
            message = $"Um novo evento foi adicionado: {NameEntry.Text}!",
        });
    }
    private void OnCloseClicked(object sender, EventArgs e) {
        if (this.Parent is Layout parentLayout) {
            parentLayout.Children.Remove(this); 
        }
    }

    private Color GetRandomColor() {
        var random = new Random();
        return Color.FromRgb(random.Next(100, 256), random.Next(100, 256), random.Next(100, 256));
    }
}