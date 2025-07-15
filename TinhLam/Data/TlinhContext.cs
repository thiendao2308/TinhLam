using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TinhLam.Data;

public partial class TlinhContext : DbContext
{
    public TlinhContext()
    {
    }

    public TlinhContext(DbContextOptions<TlinhContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CartsSession> CartsSessions { get; set; }

    public virtual DbSet<CartsUser> CartsUsers { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    public virtual DbSet<HomeBannerImage> HomeBannerImages { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostComment> PostComments { get; set; }

    public virtual DbSet<PostImage> PostImages { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductSize> ProductSizes { get; set; }

    public virtual DbSet<RedeemPoint> RedeemPoints { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=tinhlam.database.windows.net;Initial Catalog=tinhlam;Persist Security Info=True;User ID=tinhlam;Password=tl@123456;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CartsSession>(entity =>
        {
            entity.HasKey(e => e.CartSessionId).HasName("PK__CartsSes__A4069C67BADFFC96");

            entity.ToTable("CartsSession");

            entity.Property(e => e.CartSessionId).HasColumnName("CartSessionID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.SessionId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("SessionID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.CartsSessions)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__CartsSess__Produ__534D60F1");

            entity.HasOne(d => d.ProductSize).WithMany(p => p.CartsSessions)
                .HasForeignKey(d => d.ProductSizeId)
                .HasConstraintName("FK_CartsSession_ProductSize");
        });

        modelBuilder.Entity<CartsUser>(entity =>
        {
            entity.HasKey(e => e.CartUserId).HasName("PK__CartsUse__13B5FFA4E9816598");

            entity.ToTable("CartsUser");

            entity.Property(e => e.CartUserId).HasColumnName("CartUserID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.CartsUsers)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__CartsUser__Produ__5812160E");

            entity.HasOne(d => d.ProductSize).WithMany(p => p.CartsUsers)
                .HasForeignKey(d => d.ProductSizeId)
                .HasConstraintName("FK_CartsUser_ProductSize");

            entity.HasOne(d => d.User).WithMany(p => p.CartsUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__CartsUser__UserI__5629CD9C");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2BC0AA47CE");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(255);
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChatMess__3214EC076C3D8DF9");

            entity.ToTable("ChatMessage");

            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ChatMessa__UserI__756D6ECB");
        });

        modelBuilder.Entity<HomeBannerImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__HomeBann__7516F4EC3BFCD44E");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.Caption).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Link).HasMaxLength(500);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF8CAAD945");

            entity.ToTable(tb => tb.HasTrigger("trg_UpdateRewardPoints"));

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.CustomerName).HasMaxLength(255);
            entity.Property(e => e.DiscountAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.District).HasMaxLength(255);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.OrderTime).HasDefaultValueSql("(CONVERT([time],getdate()))");
            entity.Property(e => e.PaymentMethod).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.RewardPointsUsed).HasDefaultValue(0);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.StreetAddress).HasMaxLength(500);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Ward).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Orders__UserID__4316F928");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30C7D70EB07");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__OrderDeta__Order__49C3F6B7");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__OrderDeta__Produ__4AB81AF0");

            entity.HasOne(d => d.ProductSize).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductSizeId)
                .HasConstraintName("FK_OrderDetails_ProductSize");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Posts__AA126038CC6C6531");

            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.Author).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.PostedBy)
                .HasMaxLength(100)
                .HasDefaultValue("Tinh Lâm");
            entity.Property(e => e.Tags).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<PostComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__PostComm__C3B4DFAA198DC418");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.CommentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.PostId).HasColumnName("PostID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Post).WithMany(p => p.PostComments)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__PostComme__PostI__22401542");

            entity.HasOne(d => d.User).WithMany(p => p.PostComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__PostComme__UserI__2334397B");
        });

        modelBuilder.Entity<PostImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__PostImag__7516F4EC543634B0");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.Caption).HasMaxLength(255);
            entity.Property(e => e.DisplayOrder).HasDefaultValue(0);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.PostId).HasColumnName("PostID");

            entity.HasOne(d => d.Post).WithMany(p => p.PostImages)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK__PostImage__PostI__2704CA5F");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6EDCD281F8A");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Image).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(255);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Products__Catego__398D8EEE");
        });

        modelBuilder.Entity<ProductSize>(entity =>
        {
            entity.HasKey(e => e.ProductSizeId).HasName("PK__ProductS__9DADF571772605DC");

            entity.Property(e => e.ProductSizeId).HasColumnName("ProductSizeID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Size).HasMaxLength(50);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductSizes)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ProductSi__Produ__0880433F");
        });

        modelBuilder.Entity<RedeemPoint>(entity =>
        {
            entity.HasKey(e => e.RedeemId).HasName("PK__RedeemPo__C9E468F747FB2ECF");

            entity.Property(e => e.RedeemId).HasColumnName("RedeemID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.PointsUsed).HasDefaultValue(0);
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RedeemDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Order).WithMany(p => p.RedeemPoints)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RedeemPoi__Order__5DCAEF64");

            entity.HasOne(d => d.User).WithMany(p => p.RedeemPoints)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RedeemPoi__UserI__5CD6CB2B");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC104C2CC7");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4770DCDE2").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534CF7429BA").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CustomerName).HasMaxLength(255);
            entity.Property(e => e.Diachi).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.HieuLuc).HasDefaultValue(true);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.RandomKey)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RewardPoints).HasDefaultValue(0);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("User");
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.WishlistId).HasName("PK__Wishlist__233189CBBE353571");

            entity.Property(e => e.WishlistId).HasColumnName("WishlistID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Wishlists__Produ__4E88ABD4");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Wishlists__UserI__4D94879B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
