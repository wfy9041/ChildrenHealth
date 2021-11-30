namespace LaiLaiBear
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class LLBContext : DbContext
    {
        public LLBContext()
            : base("name=LLBContext")
        {
        }

        public virtual DbSet<image> image { get; set; }
        public virtual DbSet<user> user { get; set; }
        public virtual DbSet<login> login { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
