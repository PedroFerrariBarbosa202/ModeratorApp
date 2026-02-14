using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using ModeratorApp.Cards;

namespace ModeratorApp.Services {
    public static class CardManager {
        public struct EventData {
            public int event_id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string date { get; set; }
            public string time_begin { get; set; }
            public string time_end { get; set; }
            public string link { get; set; }
            public string color { get; set; }

            public Color color_fromHex => Color.FromArgb(color);
        }

        public struct ClientData {
            public int client_id { get; set; }
            public string name { get; set; }
            public int age { get; set; }
            public string email { get; set; }
            public string password { get; set; }
            public string color { get; set; }
            public byte[] user_img { get; set; }
        }

        public struct RoleData {
            public int role_id { get; set; }
            public string name { get; set; }
            public string color { get; set; }
        }

        public class TagData {
            public int tag_id { get; set; }
            public string name { get; set; }
            public Color color { get; set; }
        }

        public static EventCard add_event(EventData e_data, Layout stackLayout) {
            var card = new EventCard(e_data);
            stackLayout.Children.Add(card);
            return card;
        }

        public static ClientCard add_client(ClientData c_data, EventData e_data, Layout stackLayout) {
            var card = new ClientCard(c_data, e_data);
            stackLayout.Children.Add(card);
            return card;
        }

        public static RoleReadCard add_role(RoleData role_data, Layout stackLayout) {
            var card = new RoleReadCard(role_data);
            stackLayout.Children.Add(card);
            return card;
        }

        public static RoleManageCard add_role_manage(RoleData role_data, Layout stackLayout) {
            var card = new RoleManageCard(role_data);
            stackLayout.Children.Add(card);
            return card;
        }
        public static SubRoleShowCard add_sub_role_manage(RoleData role_data, EventData event_data, ClientData c_data, Layout stackLayout) {
            var card = new SubRoleShowCard(role_data, event_data, c_data);
            stackLayout.Children.Add(card);
            return card;
        }
        public static RoleLimitCard add_role_limit(RoleData role_data, EventData event_data, Layout stackLayout) {
            var card = new RoleLimitCard(role_data, event_data);
            stackLayout.Children.Add(card);
            return card;
        }
        public static Color GetRandomColor() {
            var random = new Random();
            return Color.FromRgb(random.Next(100, 256), random.Next(100, 256), random.Next(100, 256));
        }
    }
}
