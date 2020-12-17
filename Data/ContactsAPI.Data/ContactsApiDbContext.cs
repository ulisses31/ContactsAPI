using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ContactsAPI.Data
{
    // NOTE:
    // To create the database migration script please run in the Package Manager Console (having selected the ContactsAPI.Data project):
    // Add-Migration Init
    // later to create the database you will need to run
    // Update-Database

    public class ContactsApiDbContext : DbContext
    {
        protected readonly IConfiguration _configuration;

        public ContactsApiDbContext()
        {
            _configuration = new ConfigurationBuilder().AddJsonFile(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\appsettings.json").Build();
        }

        public ContactsApiDbContext([NotNull] IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected virtual IList<Assembly> Assemblies
        {
            get
            {
                return new List<Assembly>() {
                    {
                        Assembly.Load("ContactsAPI.Data")
                    }
                };
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var assembly in Assemblies)
            {
                // Loads all types from an assembly which have an interface of IBase and is a public class
                var classes = assembly.GetTypes().Where(s => s.GetInterfaces().Any(_interface => _interface.Equals(typeof(IBase)) && s.IsClass && !s.IsAbstract && s.IsPublic));

                foreach (var _class in classes)
                {
                    // On Model Creating
                    var onModelCreatingMethod = _class.GetMethods().FirstOrDefault(x => x.Name == "OnModelCreating" && x.IsStatic);

                    if (onModelCreatingMethod != null)
                    {
                        onModelCreatingMethod.Invoke(_class, new object[] { builder });
                    }

                    // On Base Model Creating
                    if (_class.BaseType == null || _class.BaseType != typeof(Base))
                    {
                        continue;
                    }

                    var baseOnModelCreatingMethod = _class.BaseType.GetMethods().FirstOrDefault(x => x.Name == "OnModelCreating" && x.IsStatic);

                    if (baseOnModelCreatingMethod == null)
                    {
                        continue;
                    }

                    var baseOnModelCreatingGenericMethod = baseOnModelCreatingMethod.MakeGenericMethod(new Type[] { _class });

                    if (baseOnModelCreatingGenericMethod == null)
                    {
                        continue;
                    }

                    baseOnModelCreatingGenericMethod.Invoke(typeof(Base), new object[] { builder });
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            // Sets the database connection from appsettings.json
            if (_configuration["ConnectionStrings:ContactsApiDbContext"] != null)
            {
                builder.UseSqlServer(_configuration["ConnectionStrings:ContactsApiDbContext"]);
            }
        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is IBase)
                {
                    // Here we can add some extra generic assignments before saving
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}
