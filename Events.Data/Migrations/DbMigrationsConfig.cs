namespace Events.Data.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Events.Data;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public sealed class DbMigrationsConfig : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public DbMigrationsConfig()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                var adminEmail = "darencantrell@gmail.com";
                var adminUserName = adminEmail;
                var adminFullName = "System Administrator";
                var adminPassword = "dc";
                string adminRole = "Administrator";

                CreateAdminUser(context, adminEmail, adminUserName, adminFullName, adminPassword, adminRole);
                CreateSeveralEvents(context);
            }
        }

        private void CreateAdminUser(ApplicationDbContext context, string adminEmail, string adminUserName, string adminFullName, string adminPassword, string adminRole)
        {
            //Create the "admin" user
            var adminUser = new ApplicationUser
            {
                UserName = adminUserName,
                FullName = adminFullName,
                Email = adminEmail
            };
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 1,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            var userCreateResult = userManager.Create(adminUser, adminPassword);
            if (!userCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", userCreateResult.Errors));
            }

            //Create the "Administrator" role
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var roleCreateResult = roleManager.Create(new IdentityRole(adminRole));
            if (!roleCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", roleCreateResult.Errors));
            }

            //Add the "admin user to the "Administrator role
            var addAdminRoleResult = userManager.AddToRole(adminUser.Id, adminRole);
            if (!addAdminRoleResult.Succeeded)
            {
                throw new Exception(string.Join("; ", addAdminRoleResult.Errors));
            }
        }

        private void CreateSeveralEvents(ApplicationDbContext context)
        {
            context.Events.Add(new Event
            {
                Title = "Part @ Work",
                StartDateTime = DateTime.Now.AddDays(5).AddHours(21).AddMinutes(30),
                Author = context.Users.First(),
            });

            context.Events.Add(new Event
            {
                Title = "Passed Event <Anonymous>",
                StartDateTime = DateTime.Now.AddDays(-2).AddHours(10).AddMinutes(30),
                Duration = TimeSpan.FromHours(1.5),
                Comments = new HashSet<Comment>()
                {
                    new Comment() { Text = "<Anonymous> comment" },
                    new Comment() { Text = "User comment", Author = context.Users.First() }
                }
            });

            context.Events.Add(new Event
            {
                Title = "Blamanche",
                StartDateTime = DateTime.Now.AddDays(15).AddHours(2).AddMinutes(10),
                Author = context.Users.First(),
                Duration = TimeSpan.FromHours(3),
                Comments = new HashSet<Comment>()
                {
                    new Comment() { Text = "First comment" },
                    new Comment() { Text = "Second comment", Author = context.Users.First() },
                    new Comment() { Text = "Third comment", Author = context.Users.First() }
                }
            });

            context.Events.Add(new Event
            {
                Title = "Herring",
                StartDateTime = DateTime.Now.AddDays(-15).AddHours(2).AddMinutes(10),
                Duration = TimeSpan.FromHours(3),
                Comments = new HashSet<Comment>()
                {
                    new Comment() { Text = "Arse" },
                    new Comment() { Text = "Bollocks", Author = context.Users.First() },
                    new Comment() { Text = "Tits", Author = context.Users.First() }
                }
            });

            context.Events.Add(new Event
            {
                Title = "Herring",
                StartDateTime = DateTime.Now.AddDays(25).AddHours(2).AddMinutes(10),
                Author = context.Users.First(),
                Duration = TimeSpan.FromHours(3),
            });
        }
    }
}
