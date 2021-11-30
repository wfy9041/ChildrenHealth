namespace LaiLaiBear
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("login")]
    public partial class login
    {
        public long ID { get; set; }
        [Required]
        [StringLength(30)]
        public string user { get; set; }
        [Required]
        [StringLength(30)]
        public string pwd { get; set; }
    }
}
