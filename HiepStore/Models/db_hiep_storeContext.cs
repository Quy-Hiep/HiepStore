using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HiepStore.Models
{
    public partial class db_hiep_storeContext : DbContext
    {
        public db_hiep_storeContext()
        {
        }

        public db_hiep_storeContext(DbContextOptions<db_hiep_storeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Brand> Brands { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<District> Districts { get; set; } = null!;
        public virtual DbSet<Location> Locations { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Page> Pages { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductType> ProductTypes { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Shipper> Shippers { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;
        public virtual DbSet<TransactStatus> TransactStatuses { get; set; } = null!;
        public virtual DbSet<Ward> Wards { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=HIEP\\SQLEXPRESS;Initial Catalog=db_hiep_store;User ID=sa;Password=sa");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("accounts");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Avatar)
                    .HasMaxLength(255)
                    .HasColumnName("avatar");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(150)
                    .HasColumnName("first_name");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

                entity.Property(e => e.LastLogin)
                    .HasColumnType("datetime")
                    .HasColumnName("last_login");

                entity.Property(e => e.LastName)
                    .HasMaxLength(150)
                    .HasColumnName("last_name");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .HasColumnName("password");

                entity.Property(e => e.Phone)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("phone");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.Salt)
                    .HasMaxLength(10)
                    .HasColumnName("salt")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("fk_accounts_roles");
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.ToTable("brands");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Alias)
                    .HasMaxLength(250)
                    .HasColumnName("alias");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.CreatedUser).HasColumnName("created_user");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

                entity.Property(e => e.IsPublished).HasColumnName("is_published");

                entity.Property(e => e.IsShowOnHomePage).HasColumnName("is_show_on_home_page");

                entity.Property(e => e.Levels).HasColumnName("levels");

                entity.Property(e => e.MetaDesc)
                    .HasMaxLength(250)
                    .HasColumnName("metaDesc");

                entity.Property(e => e.MetaKey)
                    .HasMaxLength(250)
                    .HasColumnName("metaKey");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .HasColumnName("name");

                entity.Property(e => e.Ordering).HasColumnName("ordering");

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.SchemaMarkup).HasColumnName("schema_markup");

                entity.Property(e => e.Thumb)
                    .HasMaxLength(250)
                    .HasColumnName("thumb");

                entity.Property(e => e.Title)
                    .HasMaxLength(250)
                    .HasColumnName("title");

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Alias)
                    .HasMaxLength(250)
                    .HasColumnName("alias");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.CreatedUser).HasColumnName("created_user");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

                entity.Property(e => e.IsPublished).HasColumnName("is_published");

                entity.Property(e => e.IsShowOnHomePage).HasColumnName("is_show_on_home_page");

                entity.Property(e => e.Levels).HasColumnName("levels");

                entity.Property(e => e.MetaDesc)
                    .HasMaxLength(250)
                    .HasColumnName("metaDesc");

                entity.Property(e => e.MetaKey)
                    .HasMaxLength(250)
                    .HasColumnName("metaKey");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .HasColumnName("name");

                entity.Property(e => e.Ordering).HasColumnName("ordering");

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.SchemaMarkup).HasColumnName("schema_markup");

                entity.Property(e => e.Thumb)
                    .HasMaxLength(250)
                    .HasColumnName("thumb");

                entity.Property(e => e.Title)
                    .HasMaxLength(250)
                    .HasColumnName("title");

                entity.Property(e => e.UpdateUser).HasColumnName("update_user");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customers");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");

                entity.Property(e => e.Avatar)
                    .HasMaxLength(255)
                    .HasColumnName("avatar");

                entity.Property(e => e.Birthday)
                    .HasColumnType("datetime")
                    .HasColumnName("birthday");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.DistrictId).HasColumnName("district_id");

                entity.Property(e => e.Email)
                    .HasMaxLength(150)
                    .HasColumnName("email")
                    .IsFixedLength();

                entity.Property(e => e.FirstName)
                    .HasMaxLength(255)
                    .HasColumnName("first_name");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

                entity.Property(e => e.LastLogin)
                    .HasColumnType("datetime")
                    .HasColumnName("last_login");

                entity.Property(e => e.LastName)
                    .HasMaxLength(255)
                    .HasColumnName("last_name");

                entity.Property(e => e.LocationId).HasColumnName("location_id");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .HasColumnName("password");

                entity.Property(e => e.Phone)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("phone");

                entity.Property(e => e.Salt)
                    .HasMaxLength(8)
                    .HasColumnName("salt")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.WardId).HasColumnName("ward_id");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("fk_Customers_districts");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("fk_Customers_locations");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.WardId)
                    .HasConstraintName("fk_Customers_wards");
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.ToTable("districts");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("locations");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Levels).HasColumnName("levels");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.NameWithType)
                    .HasMaxLength(100)
                    .HasColumnName("name_with_type");

                entity.Property(e => e.Parent).HasColumnName("parent");

                entity.Property(e => e.Slug)
                    .HasMaxLength(100)
                    .HasColumnName("slug");

                entity.Property(e => e.Type)
                    .HasMaxLength(10)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address).HasColumnName("address");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.District).HasColumnName("district");

                entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

                entity.Property(e => e.IsPaid).HasColumnName("is_paid");

                entity.Property(e => e.LocationId).HasColumnName("location_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Note).HasColumnName("note");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasColumnName("order_date");

                entity.Property(e => e.PaymentAt)
                    .HasColumnType("datetime")
                    .HasColumnName("payment_at");

                entity.Property(e => e.PaymentId).HasColumnName("payment_id");

                entity.Property(e => e.ShipAt)
                    .HasColumnType("datetime")
                    .HasColumnName("ship_at");

                entity.Property(e => e.TotalMoney).HasColumnName("total_money");

                entity.Property(e => e.TransactStatusId).HasColumnName("transact_status_id");

                entity.Property(e => e.Ward).HasColumnName("ward");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("fk_orders_customers");

                entity.HasOne(d => d.TransactStatus)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.TransactStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_orders_transact_status");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("order_details");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.OrderNumber).HasColumnName("order_number");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.TotalMoney).HasColumnName("total_money");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("fk_order_details_orders");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("fk_order_details_products");
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.ToTable("pages");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Alias)
                    .HasMaxLength(250)
                    .HasColumnName("alias");

                entity.Property(e => e.Contents).HasColumnName("contents");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

                entity.Property(e => e.IsPublished).HasColumnName("is_published");

                entity.Property(e => e.MetaDesc)
                    .HasMaxLength(250)
                    .HasColumnName("meta_desc");

                entity.Property(e => e.MetaKey)
                    .HasMaxLength(250)
                    .HasColumnName("meta_key");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .HasColumnName("name");

                entity.Property(e => e.Ordering).HasColumnName("ordering");

                entity.Property(e => e.Thumb)
                    .HasMaxLength(250)
                    .HasColumnName("thumb");

                entity.Property(e => e.Title)
                    .HasMaxLength(250)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("posts");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId).HasColumnName("account_id");

                entity.Property(e => e.Alias)
                    .HasMaxLength(255)
                    .HasColumnName("alias");

                entity.Property(e => e.Author)
                    .HasMaxLength(255)
                    .HasColumnName("author");

                entity.Property(e => e.BrandId).HasColumnName("brand_id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.Contents).HasColumnName("contents");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

                entity.Property(e => e.IsHot).HasColumnName("is_hot");

                entity.Property(e => e.IsNewfeed).HasColumnName("is_newfeed");

                entity.Property(e => e.IsPublished).HasColumnName("is_published");

                entity.Property(e => e.MetaDesc)
                    .HasMaxLength(255)
                    .HasColumnName("meta_desc");

                entity.Property(e => e.MetaKey)
                    .HasMaxLength(255)
                    .HasColumnName("meta_key");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Subtitle).HasColumnName("subtitle");

                entity.Property(e => e.TagsId).HasColumnName("tags_id");

                entity.Property(e => e.Thumb)
                    .HasMaxLength(255)
                    .HasColumnName("thumb");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");

                entity.Property(e => e.Views).HasColumnName("views");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("fk_posts_accounts");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("fk_posts_brands");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("fk_posts_categories");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("fk_posts_products");

                entity.HasOne(d => d.Tags)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.TagsId)
                    .HasConstraintName("fk_posts_tags");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Alias)
                    .HasMaxLength(255)
                    .HasColumnName("alias");

                entity.Property(e => e.BrandId).HasColumnName("brand_id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

                entity.Property(e => e.IsShowOnHomePage).HasColumnName("is_show_on_home_page");

                entity.Property(e => e.MetaDesc)
                    .HasMaxLength(255)
                    .HasColumnName("metaDesc");

                entity.Property(e => e.MetaKey)
                    .HasMaxLength(255)
                    .HasColumnName("metaKey");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Ordering).HasColumnName("ordering");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.ShortDesc)
                    .HasMaxLength(255)
                    .HasColumnName("short_desc");

                entity.Property(e => e.Tags).HasColumnName("tags");

                entity.Property(e => e.Thumb)
                    .HasMaxLength(255)
                    .HasColumnName("thumb");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.Property(e => e.UnitsInStock).HasColumnName("units_in_stock");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UserCreated).HasColumnName("user_created");

                entity.Property(e => e.UserUpdated).HasColumnName("user_updated");

                entity.Property(e => e.Video)
                    .HasMaxLength(255)
                    .HasColumnName("video");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("fk_products_brands");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("fk_products_categories");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("fk_product_type");
            });

            modelBuilder.Entity<ProductType>(entity =>
            {
                entity.ToTable("product_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .HasColumnName("description");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Shipper>(entity =>
            {
                entity.ToTable("shippers");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Company)
                    .HasMaxLength(150)
                    .HasColumnName("company");

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .HasColumnName("phone")
                    .IsFixedLength();

                entity.Property(e => e.ShipAt)
                    .HasColumnType("datetime")
                    .HasColumnName("ship_at");

                entity.Property(e => e.Shippername)
                    .HasMaxLength(150)
                    .HasColumnName("shippername");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("tags");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<TransactStatus>(entity =>
            {
                entity.ToTable("transact_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");
            });

            modelBuilder.Entity<Ward>(entity =>
            {
                entity.ToTable("wards");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
