using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;


namespace ModeratorApp.Models {
    [Table("Volunteer_Sector")]
    public class VolunteerSector : BaseModel {
        [PrimaryKey("id")]
        public int ID { get; set; }

        [Column("volunteer_id")]
        public int volunteer_ID { get; set; }

        [Column("sector_id")]
        public int sector_ID { get; set; }
    }
}
