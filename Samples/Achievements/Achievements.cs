using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace Achievements
{

    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public List<Kill> Kills { get; set; }
        public ulong Crits { get; set; }
        public ulong Resists { get; set; }
        public ulong Evades { get; set; }
        public BigInteger DamageDealt { get; set; }
        public BigInteger DamageTaken { get; set; }
        public ulong MostDamageDealt { get; set; }
        public ulong MostDamageTaken { get; set; }
    }

    public class Kill
    {
        public Dictionary<string, uint> Kills { get; set; } = new();
    }

    public class AchievementContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        public string DbPath { get; }

        public BloggingContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "blogging.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }

    //public class Blog
    //{
    //    public int BlogId { get; set; 
    //    public List<Post> Posts { get; } = new();
    //}

    //public class Post
    //{
    //    public int PostId { get; set; }
}
