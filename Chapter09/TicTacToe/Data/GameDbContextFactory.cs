using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TicTacToe.Data
{
    public class GameDbContextFactory :  IDesignTimeDbContextFactory<GameDbContext>
    {
        public GameDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder =
             new DbContextOptionsBuilder<GameDbContext>();
            optionsBuilder.UseSqlServer(@"Server= (localdb)\MSSQLLocalDB;Database=TicTacToe; Trusted_Connection=True;MultipleActiveResultSets=true");
            return new GameDbContext(optionsBuilder.Options);
        }
    }
}
