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

    // 如果需要进一步配置，可以在此重写 OnModelCreating 方法
}