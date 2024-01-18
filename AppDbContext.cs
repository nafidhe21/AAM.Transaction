using Microsoft.EntityFrameworkCore;
using AAM.Transaction.Model;

namespace AAM.Transaction
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TransactionRecord> Transactions { get; set; }
    }
}
