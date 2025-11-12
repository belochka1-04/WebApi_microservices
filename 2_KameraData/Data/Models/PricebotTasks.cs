using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Models
{
    [Table("pricebot_tasks")] // Optional: Specifies the table name in the database
    public class PricebotTask
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("part_number")]
        public string? PartNumber { get; set; }

        [Column("shop_id")]
        public int? ShopId { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("price")]
        public decimal? Price { get; set; }

        [Column("error_message")]
        public string? ErrorMessage { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("Site_part_number")]
        public string? SitePartNumber { get; set; }

        [Column("page_link")]
        public string? PageLink { get; set; }

        [Column("found_parts")]
        public int? FoundParts { get; set; }

        [Column("availability")]
        public string? Availability { get; set; }

        [Column("part_name")]
        public string? PartName { get; set; }

        [Column("your_price")]
        public decimal? YourPrice { get; set; }

        [Column("regular_price")]
        public decimal? RegularPrice { get; set; }

        [Column("special_field_1")]
        public string? SpecialField1 { get; set; }

        [Column("special_field_2")]
        public string? SpecialField2 { get; set; }

        [Column("special_field_3")]
        public string? SpecialField3 { get; set; }

    }
}

