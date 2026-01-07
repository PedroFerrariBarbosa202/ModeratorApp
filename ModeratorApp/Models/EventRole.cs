using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;


namespace ModeratorApp.Models {
    [Table("Event_Role")]
    public class EventRole : BaseModel {
        [PrimaryKey("role_id")]
        public int role_ID { get; set; }

        [PrimaryKey("event_id")]
        public int event_ID { get; set; }

        [Column("number_limit")]
        public int number_limit { get; set; }

    }
}
