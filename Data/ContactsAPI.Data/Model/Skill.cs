using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsAPI.Data.Model
{
    [Table("Skills")]
    public class Skill : Base
    {
        public string Name { get; set; }

        public virtual ICollection<ContactSkill> ContactSkills { get; set; }

        public const int SkillNameMaxLengh = 40;

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {         
            modelBuilder.Entity<Skill>().Property(property => property.Name).HasMaxLength(SkillNameMaxLengh).IsRequired();
        }
    }
}
