using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ModeratorApp.Models {
        [Table("Volunteer")]
        public class Volunteer : BaseModel {
            [PrimaryKey("volunteer_id")]
            public int volunteer_ID { get; set; }

            [Column("name")]
            public string name { get; set; }

            [Column("email")]
            public string email { get; set; }

            [Column("password")]
            public string password { get; set; }

            [Column("age")]
            public int age { get; set; }

            [Column("user_img")]
            public byte[] user_img { get; set; }
    }
}
