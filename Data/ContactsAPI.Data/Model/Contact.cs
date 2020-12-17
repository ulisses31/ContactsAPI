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
    [Table("Contacts")]
    public class Contact : Base
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Fullname { get { return Firstname + " " + Lastname; } }

        // Address
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public string Email { get; set; }
        public string MobilePhoneNumber { get; set; }

        public virtual ICollection<ContactSkill> ContactSkills { get; set; } = new HashSet<ContactSkill>();

        [JsonIgnore]
        public virtual User User { get; set; }


        public const int AddressLine1MaxLength = 150;
        public const int AddressLine2MaxLength = 150;
        public const int CityMaxLength = 50;
        public const int PostalCodeMaxLength = 50;
        public const int CountryMaxLength = 30;
        public const int EmailMaxLength = 200;
        public const int MobilePhoneNumberMaxLength = 20;


        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>().Property(property => property.Firstname).HasMaxLength(Constants.FirstNameMaxLength).IsRequired();
            modelBuilder.Entity<Contact>().Property(property => property.Lastname).HasMaxLength(Constants.LastNameMaxLength).IsRequired();

            modelBuilder.Entity<Contact>().Property(property => property.AddressLine1).HasMaxLength(AddressLine1MaxLength);
            modelBuilder.Entity<Contact>().Property(property => property.AddressLine2).HasMaxLength(AddressLine2MaxLength);
            modelBuilder.Entity<Contact>().Property(property => property.City).HasMaxLength(CityMaxLength);
            modelBuilder.Entity<Contact>().Property(property => property.PostalCode).HasMaxLength(PostalCodeMaxLength);
            modelBuilder.Entity<Contact>().Property(property => property.Country).HasMaxLength(CountryMaxLength);

            modelBuilder.Entity<Contact>().Property(property => property.Email).HasMaxLength(EmailMaxLength);
            modelBuilder.Entity<Contact>().Property(property => property.MobilePhoneNumber).HasMaxLength(MobilePhoneNumberMaxLength);
        }

    }
}
