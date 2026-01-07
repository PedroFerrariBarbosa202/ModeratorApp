using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;


namespace ModeratorApp.Models {
    [Table("Volunteer_Event")]
    public class VolunteerEvent : BaseModel {
        [PrimaryKey("id")]
        public int id { get; set; }

        [Column("volunteer_id")]
        public int volunteer_ID { get; set; }

        [Column("event_id")]
        public int event_ID { get; set; }

        [Column("role_id")]
        public int role_ID { get; set; }

        [Column("date")]
        public DateOnly date { get; set; }

        [Column("time_begin")]
        public TimeOnly time_begin { get; set; }

        [Column("time_end")]
        public TimeOnly time_end { get; set; }
    }
}
