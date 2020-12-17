using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContactsAPI.Data
{
    public abstract class Base: IBase
    {
        public int Id { get; set; }


        public static void OnModelCreating<TEntity>(ModelBuilder modelBuilder)
                where TEntity : class, IBase
        {
            modelBuilder.Entity<TEntity>().HasKey(entity => entity.Id);
        }
    }
}
