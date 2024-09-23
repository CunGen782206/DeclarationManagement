using DeclarationManagement.Model;
using Microsoft.EntityFrameworkCore;

namespace DeclarationManagement;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<ApplicationForm> ApplicationForms { get; set; }
    public DbSet<ApprovalRecord> ApprovalRecords { get; set; }
    public DbSet<ApprovalFlow> ApprovalFlows { get; set; }
    public DbSet<TableSummary> TableSummaries { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // 可选：配置模型关系和约束
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 在此定义外键、关系和表配置
    }
}