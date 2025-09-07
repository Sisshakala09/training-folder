// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore;
// using ProfileBook.Api.Models;
// using ProfileBook.Api.Data;


// namespace ProfileBook.Api.Data;

// public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
// {
//     public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

//     public DbSet<Post> Posts => Set<Post>();
//     public DbSet<Comment> Comments => Set<Comment>();
//     public DbSet<PostLike> PostLikes => Set<PostLike>();
//     public DbSet<Message> Messages => Set<Message>();
//     public DbSet<Report> Reports => Set<Report>();
//     public DbSet<Group> Groups => Set<Group>();
//     public DbSet<GroupMember> GroupMembers => Set<GroupMember>();

//     protected override void OnModelCreating(ModelBuilder b)
//     {
//         base.OnModelCreating(b);

//         b.Entity<GroupMember>().HasKey(x => new { x.GroupId, x.UserId });

//         b.Entity<PostLike>()
//             .HasIndex(x => new { x.PostId, x.UserId })
//             .IsUnique();

//         b.Entity<Post>()
//             .HasOne(p => p.User)
//             .WithMany()
//             .HasForeignKey(p => p.UserId)
//             .OnDelete(DeleteBehavior.Cascade);

//         b.Entity<Comment>()
//             .HasOne(c => c.User)
//             .WithMany()
//             .HasForeignKey(c => c.UserId)
//             .OnDelete(DeleteBehavior.Restrict);

//         b.Entity<Comment>()
//             .HasOne(c => c.Post)
//             .WithMany(p => p.Comments)
//             .HasForeignKey(c => c.PostId)
//             .OnDelete(DeleteBehavior.Cascade);
//     }
// }





// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore;
// using ProfileBook.Api.Models;

// namespace ProfileBook.Api.Data
// {
//     public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
//     {
//         public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

//         public DbSet<Post> Posts => Set<Post>();
//         public DbSet<PostLike> PostLikes => Set<PostLike>();
//         public DbSet<PostComment> PostComments => Set<PostComment>();
//         public DbSet<Message> Messages => Set<Message>();
//         public DbSet<Report> Reports => Set<Report>();
//         public DbSet<Group> Groups => Set<Group>();
//         public DbSet<GroupMember> GroupMembers => Set<GroupMember>();

//         protected override void OnModelCreating(ModelBuilder b)
//         {
//             base.OnModelCreating(b);
//             b.Entity<PostLike>().HasIndex(x => new { x.PostId, x.UserId }).IsUnique();
//             b.Entity<GroupMember>().HasIndex(x => new { x.GroupId, x.UserId }).IsUnique();
//         }
//     }
// }




using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProfileBook.Api.Models;

namespace ProfileBook.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<PostLike> PostLikes => Set<PostLike>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<GroupMember> GroupMembers => Set<GroupMember>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // one like per user per post
            b.Entity<PostLike>()
                .HasIndex(x => new { x.PostId, x.UserId })
                .IsUnique();

            // composite key for group membership
            b.Entity<GroupMember>()
                .HasKey(x => new { x.GroupId, x.UserId });

            // relationships
            b.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Message>().HasIndex(m => m.SenderId);
            b.Entity<Message>().HasIndex(m => m.ReceiverId);
        }
    }
}