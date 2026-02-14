using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;


namespace ModeratorApp.Models {
    [Table("Sectors")]
    public class Sector : BaseModel {
        [PrimaryKey("sector_id")]
        public int sector_ID { get; set; }

        [Column("name")]
        public string name { get; set; }

        [Column("color")]
        public string color { get; set; }
    }
}
