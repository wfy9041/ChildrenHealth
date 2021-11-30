namespace LaiLaiBear
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("user")]
    public partial class user
    {
        public long ID { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(30)]
        public string KS { get; set; }

        [StringLength(30)]
        public string CH { get; set; }

        [StringLength(30)]
        public string Sex { get; set; }

        [StringLength(30)]
        public string ZYH { get; set; }

        [StringLength(30)]
        public string SJYS { get; set; }

        [StringLength(30)]
        public string Age { get; set; }

        [StringLength(30)]
        public string BBBH { get; set; }

        public int SJRQ { get; set; }

        [StringLength(30)]
        public string BBMYD { get; set; }

        [StringLength(30)]
        public string XY_bxb { get; set; }

        [StringLength(30)]
        public string XY_hxb { get; set; }

        [StringLength(30)]
        public string XY_xhdb { get; set; }

        [StringLength(30)]
        public string XY_xxb { get; set; }

        [StringLength(30)]
        public string NY_nhxbjs { get; set; }

        [StringLength(30)]
        public string NY_nbxbjs { get; set; }

        [StringLength(30)]
        public string NY_wfljj { get; set; }

        [StringLength(30)]
        public string NY_blgx { get; set; }

        [StringLength(30)]
        public string FB_hxb { get; set; }

        [StringLength(30)]
        public string FB_bxb { get; set; }

        [StringLength(30)]
        public string FB_cn { get; set; }

        [StringLength(30)]
        public string FB_bn { get; set; }

        [StringLength(30)]
        public string FB_jj { get; set; }

        [StringLength(30)]
        public string JYYS { get; set; }

        [StringLength(30)]
        public string SHYS { get; set; }

        public int BGRQ { get; set; }
        public bool isPrintf { get; set; }
        [StringLength(200)]
        public string diagnose { set; get; }
    }
}
