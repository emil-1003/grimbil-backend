using System;
using System.Collections.Generic;
using grimbil_ef.Models;
using Microsoft.EntityFrameworkCore;

namespace grimbil_ef.dbContext;

public partial class GrimbildbContext : DbContext
{
    public GrimbildbContext()
    {
    }

    public GrimbildbContext(DbContextOptions<GrimbildbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Picture> Pictures { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=localhost;uid=root;pwd=;database=grimbildb");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Commentid).HasName("PRIMARY");

            entity.ToTable("comments");

            entity.HasIndex(e => e.Postid, "FK_comments_posts");

            entity.HasIndex(e => e.Userid, "FK_comments_users");

            entity.Property(e => e.Commentid)
                .HasColumnType("int(11)")
                .HasColumnName("commentid");
            entity.Property(e => e.Comment1)
                .HasColumnType("text")
                .HasColumnName("comment");
            entity.Property(e => e.Postid)
                .HasColumnType("int(11)")
                .HasColumnName("postid");
            entity.Property(e => e.Userid)
                .HasColumnType("int(11)")
                .HasColumnName("userid");

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.Postid)
                .HasConstraintName("FK_comments_posts");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("FK_comments_users");
        });

        modelBuilder.Entity<Picture>(entity =>
        {
            entity.HasKey(e => e.Pictureid).HasName("PRIMARY");

            entity.ToTable("pictures");

            entity.HasIndex(e => e.Postid, "FK_pictures_posts");

            entity.Property(e => e.Pictureid)
                .HasColumnType("int(11)")
                .HasColumnName("pictureid");
            entity.Property(e => e.Picture1).HasColumnName("picture");
            entity.Property(e => e.Postid)
                .HasColumnType("int(11)")
                .HasColumnName("postid");

            entity.HasOne(d => d.Post).WithMany(p => p.Pictures)
                .HasForeignKey(d => d.Postid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_pictures_posts");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Postid).HasName("PRIMARY");

            entity.ToTable("posts");

            entity.HasIndex(e => e.Userid, "userid");

            entity.Property(e => e.Postid)
                .HasColumnType("int(11)")
                .HasColumnName("postid");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Userid)
                .HasColumnType("int(11)")
                .HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("FK__users");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Ratingid).HasName("PRIMARY");

            entity.ToTable("rating");

            entity.HasIndex(e => e.Postid, "FK_rating_posts");

            entity.HasIndex(e => e.Userid, "FK_rating_users");

            entity.Property(e => e.Ratingid)
                .HasColumnType("int(11)")
                .HasColumnName("ratingid");
            entity.Property(e => e.Postid)
                .HasColumnType("int(11)")
                .HasColumnName("postid");
            entity.Property(e => e.Rating1)
                .HasColumnType("int(11)")
                .HasColumnName("rating");
            entity.Property(e => e.Userid)
                .HasColumnType("int(11)")
                .HasColumnName("userid");

            entity.HasOne(d => d.Post).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.Postid)
                .HasConstraintName("FK_rating_posts");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("FK_rating_users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.Userid)
                .HasColumnType("int(11)")
                .HasColumnName("userid");
            entity.Property(e => e.Useremail)
                .HasMaxLength(50)
                .HasDefaultValueSql("''")
                .HasColumnName("useremail");
            entity.Property(e => e.Userpassword)
                .HasMaxLength(255)
                .HasDefaultValueSql("'0'")
                .HasColumnName("userpassword");
            entity.Property(e => e.Usertype)
                .HasColumnType("int(11)")
                .HasColumnName("usertype");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
