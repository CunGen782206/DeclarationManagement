using DeclarationManagement.Model;
using Microsoft.EntityFrameworkCore;

namespace DeclarationManagement;

/// <summary>
/// 环境
/// </summary>
public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; } //这个名称需要和数据库中的名称一致
    public DbSet<ApplicationForm> ApplicationForms { get; set; }
    public DbSet<ApprovalRecord> ApprovalRecords { get; set; }
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