using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsAPI.Data.Model
{
    [Table("ContactSkills")]
    public class ContactSkill : Base
    {
        public int ContactId { get; set; }
        public virtual Contact Contact { get; set; }

        public int SkillId { get; set; }
        public virtual Skill Skill { get; set; }

        [Range(Constants.SkillLevelMin, Constants.SkillLevelMax)]
        public int? Level { get; set; }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContactSkill>().Property(property => property.ContactId).IsRequired();
            modelBuilder.Entity<ContactSkill>().Property(property => property.SkillId).IsRequired();

            modelBuilder.Entity<ContactSkill>()
                .HasOne<Contact>(sc => sc.Contact)
                .WithMany(s => s.ContactSkills)
                .HasForeignKey(sc => sc.ContactId);

            modelBuilder.Entity<ContactSkill>()
                .HasOne<Skill>(sc => sc.Skill)
                .WithMany(s => s.ContactSkills)
                .HasForeignKey(sc => sc.SkillId);

        }

    }
}
