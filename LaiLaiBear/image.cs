namespace LaiLaiBear
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("image")]
    public partial class image
    {
        public long ID { get; set; }
        [StringLength(30)]
        public string BBBH { get; set; }

        [StringLength(100)]
        public string PicName { get; set; }

        [StringLength(5)]
        public string Count { get; set; }
    }
}
