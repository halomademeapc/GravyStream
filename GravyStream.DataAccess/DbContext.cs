using GravyStream.Schema;
using Microsoft.EntityFrameworkCore;

namespace GravyStream.DataAccess
{
    public class VodContext : DbContext
    {
        public VodContext(DbContextOptions<VodContext> options) : base(options)
        {
        }

        public DbSet<Person> People { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Contributor> Contributors { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<VideoReaction> VideoReactions { get; set; }
        public DbSet<MediaStream> MediaStreams { get; set; }
        public DbSet<AudioStream> AudioStreams { get; set; }
        public DbSet<VideoStream> VideoStreams { get; set; }
        public DbSet<SubtitleStream> SubtitleStreams { get; set; }
        public DbSet<ConversionJob> ConversionJobs { get; set; }
        public DbSet<VideoTag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("Vod");

            builder.Entity<Person>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasAlternateKey(p => p.UserName);
                entity.Property(p => p.Id).ValueGeneratedOnAdd();
                entity.Property(p => p.Registered).ValueGeneratedOnAdd();
            });

            builder.Entity<Contributor>(entity =>
            {
                entity.HasKey(c => new { c.VideoId, c.PersonId });
                entity.HasOne(c => c.Person)
                    .WithMany(p => p.Contributions)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(c => c.Video)
                    .WithMany(v => v.Contributors)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasOne(c => c.Person)
                    .WithMany(p => p.Comments)
                    .IsRequired(true)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(c => c.Video)
                    .WithMany(v => v.Comments)
                    .IsRequired(true)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(c => c.Responses)
                    .WithOne(r => r.Parent)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.Property(c => c.Id).ValueGeneratedOnAdd();
            });

            builder.Entity<VideoReaction>(entity =>
            {
                entity.HasKey(vr => new { vr.PersonId, vr.VideoId });
                entity.HasOne(vr => vr.Person)
                    .WithMany(p => p.Reactions)
                    .IsRequired(true)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(vr => vr.Video)
                    .WithMany(v => v.Reactions)
                    .IsRequired(true)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(vr => vr.Reaction)
                    .WithMany(r => r.VideoReactions)
                    .IsRequired(true)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Video>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Id).ValueGeneratedOnAdd();
                entity.Property(p => p.Uploaded).ValueGeneratedOnAdd();
                entity.HasAlternateKey(v => v.Slug);
            });

            builder.Entity<MediaStream>(entity =>
            {
                entity.ToTable("Streams");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Id).ValueGeneratedOnAdd();
                entity.HasOne(s => s.Video)
                    .WithMany(v => v.Streams)
                    .IsRequired(true)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<VideoTag>(entity =>
            {
                entity.HasKey(t => new { t.VideoId, t.Tag });
                entity.HasOne(t => t.Video)
                    .WithMany(v => v.Tags)
                    .IsRequired(true)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ConversionJob>(entity =>
            {
                entity.HasKey(j => j.StreamId);
                entity.HasOne(j => j.Stream)
                    .WithOne(s => s.ConversionJob)
                    .IsRequired(true)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
