using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Models;

public partial class EcommerceContext : DbContext
{
    public EcommerceContext()
    {
    }

    public EcommerceContext(DbContextOptions<EcommerceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Category> Category { get; set; }

    public virtual DbSet<NameRole> NameRoles { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=HUY;Initial Catalog=ecommerce;Persist Security Info=True;User ID=sa;Password=123;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserRole>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetUserRoles_RoleId");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetUserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Namerole_RoleId_AspNetUserRole");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserRoles).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_category_1");

            entity.ToTable("category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<NameRole>(entity =>
        {
            entity.ToTable("NameRole");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NameRole1).HasColumnName("name_role");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("money")
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.OrderInfo).HasColumnName("order_info");
            entity.Property(e => e.PartnerCode).HasColumnName("partner_code");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.ToTable("Order_details");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Amount)
                .HasColumnType("money")
                .HasColumnName("amount");
            entity.Property(e => e.DetailedAddress).HasColumnName("detailed_address");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Fullname).HasColumnName("fullname");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PartnerCode).HasColumnName("partnerCode");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.Products).HasColumnName("products");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Discount)
                .HasColumnType("money")
                .HasColumnName("discount");
            entity.Property(e => e.IdCategory).HasColumnName("idCategory");
            entity.Property(e => e.ImageProduct).HasColumnName("image_product");
            entity.Property(e => e.ImageSpecifications).HasColumnName("image_specifications");
            entity.Property(e => e.Model)
                .HasMaxLength(50)
                .HasColumnName("model");
            entity.Property(e => e.NameProduct).HasColumnName("name_product");
            entity.Property(e => e.Origin)
                .HasMaxLength(50)
                .HasColumnName("origin");
            entity.Property(e => e.PriceProduct)
                .HasColumnType("money")
                .HasColumnName("price_product");
            entity.Property(e => e.Producer)
                .HasMaxLength(50)
                .HasColumnName("producer");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdCategory)
                .HasConstraintName("FK_category");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("Role_Permissions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PermissionsId)
                .HasMaxLength(450)
                .HasColumnName("permissions_id");
            entity.Property(e => e.RoleId)
                .HasMaxLength(450)
                .HasColumnName("role_id");

            entity.HasOne(d => d.Permissions).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionsId)
                .HasConstraintName("fk_role_permission_appnetrole");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("fk_role_id_role_permissions_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
