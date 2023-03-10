using ChatApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Text.RegularExpressions;

namespace ChatApp.Data
{
    public class ChatAppDatabase : DbContext
    {
        public ChatAppDatabase(DbContextOptions options) : base(options)
        {
        }
        public DbSet<UserModel> users { get; set; }
        public DbSet<BlackListToken> blackListTokens { get; set; }
       
    }
}
