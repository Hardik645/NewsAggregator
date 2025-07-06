using Microsoft.EntityFrameworkCore;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.DAL.Context
{
    public class NewsAggregatorDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SavedArticle> SavedArticles { get; set; }
        public DbSet<NotificationPreference> NotificationPreferences { get; set; }
        public DbSet<NotificationKeyword> NotificationKeywords { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ArticleFeedback> ArticleFeedbacks { get; set; }
        public DbSet<CategoryKeyword> CategoryKeywords { get; set; }
        public DbSet<HiddenKeyword> HiddenKeywords { get; set; }

        public NewsAggregatorDbContext(DbContextOptions<NewsAggregatorDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Username).IsRequired();
                entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Role).IsRequired();
            });

            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Title).IsRequired();
                entity.Property(a => a.Url).IsRequired();
                entity.Property(a => a.Content).IsRequired();
                entity.Property(a => a.PublishedAt).IsRequired();
                entity.HasOne(a => a.Source)
                      .WithMany(s => s.Articles)
                      .HasForeignKey(a => a.SourceId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(a => a.Category)
                      .WithMany(c => c.Articles)
                      .HasForeignKey(a => a.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Source>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired();
                entity.Property(s => s.ApiKey).IsRequired();
                entity.Property(s => s.ApiUrl).IsRequired();
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(c => c.Name).IsUnique();
            });

            modelBuilder.Entity<SavedArticle>(entity =>
            {
                entity.HasKey(sa => sa.Id);
                entity.HasOne(sa => sa.User)
                      .WithMany(u => u.SavedArticles)
                      .HasForeignKey(sa => sa.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(sa => sa.Article)
                      .WithMany(a => a.SavedArticles)
                      .HasForeignKey(sa => sa.ArticleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(sa => sa.SavedAt).IsRequired();
            });

            modelBuilder.Entity<NotificationPreference>(entity =>
            {
                entity.HasKey(np => np.Id);
                entity.HasOne(np => np.User)
                      .WithMany(u => u.NotificationPreferences)
                      .HasForeignKey(np => np.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(np => np.Category)
                      .WithMany(c => c.NotificationPreferences)
                      .HasForeignKey(np => np.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(np => np.Enabled).IsRequired();
            });

            modelBuilder.Entity<NotificationKeyword>(entity =>
            {
                entity.HasKey(nk => nk.Id);
                entity.HasOne(nk => nk.User)
                      .WithMany(u => u.NotificationKeywords)
                      .HasForeignKey(nk => nk.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(nk => nk.Keyword).IsRequired();
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.HasOne(n => n.User)
                      .WithMany(u => u.Notifications)
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(n => n.Article)
                      .WithMany(a => a.Notifications)
                      .HasForeignKey(n => n.ArticleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(n => n.SentAt).IsRequired();
                entity.Property(n => n.Type).IsRequired();
            });

            modelBuilder.Entity<ArticleFeedback>(entity =>
            {
                entity.HasKey(af => af.Id);
                entity.HasOne(af => af.User)
                      .WithMany(u => u.ArticleFeedbacks)
                      .HasForeignKey(af => af.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(af => af.Article)
                      .WithMany(a => a.ArticleFeedbacks)
                      .HasForeignKey(af => af.ArticleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(af => af.IsLike).IsRequired();
                entity.Property(af => af.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<CategoryKeyword>(entity =>
            {
                entity.HasKey(ck => ck.Id);
                entity.Property(ck => ck.Keyword).IsRequired();
                entity.HasOne(ck => ck.Category)
                      .WithMany(c => c.CategoryKeywords)
                      .HasForeignKey(ck => ck.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<HiddenKeyword>(entity =>
            {
                entity.HasKey(hk => hk.Id);
                entity.Property(hk => hk.Keyword)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.HasIndex(hk => hk.Keyword)
                      .IsUnique();
                entity.Property(hk => hk.IsDeleted)
                      .IsRequired();
            });
        }
    }
}