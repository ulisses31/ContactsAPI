using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ContactsAPI.Data.Model
{
    [Table("Users")]
    public class User : Base
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public string Username { get; set; }

        [JsonIgnore]
        public byte[] PasswordHash { get; set; }

        [JsonIgnore]
        public byte[] PasswordSalt { get; set; }

        public int? ContactId { get; set; }
        public virtual Contact Contact { get; set; }

        public const int UsernameMaxLengh = 30;

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Property(property => property.Firstname).HasMaxLength(Constants.FirstNameMaxLength).IsRequired();
            modelBuilder.Entity<User>().Property(property => property.Lastname).HasMaxLength(Constants.LastNameMaxLength).IsRequired();
            modelBuilder.Entity<User>().Property(property => property.Username).HasMaxLength(UsernameMaxLengh).IsRequired();

            modelBuilder.Entity<User>().Property(property => property.ContactId).IsRequired();

            modelBuilder.Entity<User>()
                .HasOne<Contact>(sc => sc.Contact).WithOne(s => s.User)
                .HasForeignKey<User>(sc => sc.ContactId);
        }
    }
}