using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ModeratorApp.Cards;
using ModeratorApp.Content;
using ModeratorApp.Services;
using System.Data;
using System.Diagnostics;
using TEST_APP.Services;

namespace ModeratorApp;

public partial class EventPage : ContentPage {
    Models.Events ev_data = new Models.Events();
    bool _isOpen = false;
    public EventPage(Models.Events data) {
        InitializeComponent();
        // get data from specific event
        ev_data = data;

        MainText.Text = "Event ID: " + data.event_ID.ToString();
        DescriptionText.Text = data.description;
        Link.Text = "\nLink: " + data.link;

        AddRoles();
        ShowClients();
    }

    private async void ShowClients() {
        // add loading screen
        Loading loading_page = new Loading();
        ContentGrid.Children.Add(loading_page);

        // get all events from Volunteer_event connected to a specific ID
        var response = await DatabaseConnector.Client
             .From<Models.VolunteerEvent>()
             .Where(v => v.event_ID == ev_data.event_ID)
             .Get();

        HashSet<string> seen_volunteers = new HashSet<string>();

        foreach (Models.VolunteerEvent row in response.Models) {
            var volunteer = await DatabaseConnector.Client
               .From<Models.Volunteer>()
               .Where(v => v.volunteer_ID == row.volunteer_ID)
               .Single();

                //if client doent exist, continue
                if (volunteer == null)
                    continue;

                string? client_name = volunteer.name;

                //if name doent exist, continue
                if (client_name == null)
                    continue;

                //add name to HashSet if its not already there
                if (seen_volunteers.Contains(client_name)) {
                    //if client in Hash set
                    continue;
                }
                else {
                    //if client not in HashSet, add it
                    var client_data = new Models.Volunteer {
                        volunteer_ID = volunteer.volunteer_ID,
                        name = client_name,
                        age = volunteer.age,
                        email = volunteer.email,
                        user_img = volunteer.user_img
                    };

                    // add client card to stack
                    ClientCard client_card = new ClientCard(client_data, ev_data);
                    ClientStackLayout.Children.Add(client_card);
   
                    seen_volunteers.Add(client_name);

                    // add role
                    var vol_ev_roles = await DatabaseConnector.Client
                          .From<Models.VolunteerEvent>()
                          .Where(v => v.event_ID == ev_data.event_ID)
                          .Where(v => v.volunteer_ID == volunteer.volunteer_ID)
                          .Get();

                    foreach (Models.VolunteerEvent role_row in vol_ev_roles.Models) {
                        var role = await DatabaseConnector.Client
                          .From<Models.Roles>()
                          .Where(v => v.role_ID == role_row.role_ID)
                          .Single();

                        if (role == null)
                            continue;

                        var role_data = new Models.Roles {
                                role_ID = role.role_ID,
                                name = role.name,
                        };

                        VerticalStackLayout role_stack = client_card.RoleStackLayout;
                        var sub_role_card = new SubRoleShowCard(role_data, ev_data, client_data);
                        role_stack.Children.Add(sub_role_card);

                        Debug.WriteLine("Added Role to " + client_name);
                    }
            }
        }

        // remove loading page
        ContentGrid.Children.Remove(loading_page);
    }

    private async void AddRoles() {
        await DatabaseConnector.InitializeAsync();

        // get all role_ids that are associated with a event
        var response = await DatabaseConnector.Client
          .From<Models.EventRole>()
          .Where(v => v.event_ID == ev_data.event_ID)
          .Get();

        foreach(Models.EventRole row in response.Models) {
            // get role info by role_ID
            var role = await DatabaseConnector.Client
              .From<Models.Roles>()
              .Where(v => v.role_ID == row.role_ID)
              .Single();
            
            if(role == null) continue;

            var role_data = new Models.Roles {
                role_ID = role.role_ID,
                name = role.name,
            };

            RoleLimitCard role_limit = new RoleLimitCard(role_data, ev_data);
            RoleStack.Children.Add(role_limit);
        }
    }

    private void ShowRoles(object sender, EventArgs e) {
        if (!_isOpen) {
            ArrowImage.Source = "seta_cima.png";
            GeneralRoleContainer.HeightRequest = 400;
        }
        else {
            ArrowImage.Source = "seta_baixo.png";
            GeneralRoleContainer.HeightRequest = 40;
        }
        _isOpen = !_isOpen;
    }
}
