namespace Api.Data
{
    // users to play with:
    // 1: admin
    // 2: sholeh
    // 3: barb
    // 4: todd
    // 5: sHaYYa

    // all passwords: Password123

    public static class SeedContext
    {
        public static async Task InitializeAsync(Context context,
            ContextVisitors hSContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IMapper mapper)
        {
            //if (hSContext.Database.GetPendingMigrations().Count() > 0)
            //{
            //    await hSContext.Database.MigrateAsync();
            //}

            if (context.Database.GetPendingMigrations().Count() > 0)
            {
                await context.Database.MigrateAsync();
            }

            // Roles
            if (!roleManager.Roles.Any())
            {
                foreach (var role in SD.Roles)
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = role });
                }
            }

            // Status
            if (!context.UserStatus.Any())
            {
                foreach (var status in SD.Statuses)
                {
                    var statusToAdd = new UserStatus { Name = status };
                    context.UserStatus.Add(statusToAdd);
                }
            }

            // Country
            if (!context.Country.Any())
            {
                foreach (var country in SD.GetCountries())
                {
                    context.Country.Add(new Country { Name = country });
                }
            }

            // Badge
            if (!context.Badge.Any())
            {
                foreach (var badge in SD.GetBadges())
                {
                    context.Badge.Add(mapper.Map<Badge>(badge));
                }
            }

            if (context.ChangeTracker.HasChanges())
            {
                await context.SaveChangesAsync();
            }

            if (!userManager.Users.Any())
            {
                var pinkBadgeId = await context.Badge.Where(x => x.Color == SD.Pink).Select(x => x.Id).FirstOrDefaultAsync();
                var worldId = await context.Country.Where(x => x.Name == SD.DefaultCountry).Select(x => x.Id).FirstOrDefaultAsync();
                var offlineStatusId = await context.UserStatus.Where(x => x.Name == SD.Offline).Select(x => x.Id).FirstOrDefaultAsync();

                // "Admin" -> "Admin"
                var adminPhoto = new Photo
                {
                    PhotoUrl = "https://randomuser.me/api/portraits/men/1.jpg",
                };

                var adminProfile = new UserProfile
                {
                    Photo = adminPhoto,
                    BadgeId = pinkBadgeId,
                    CountryId = worldId,
                    StatusId = offlineStatusId
                };

                var admin = new ApplicationUser
                {
                    PlayerName = "Admin",
                    Email = "admin@hokmshelem.com",
                    UserName = "admin",
                    EmailConfirmed = true,
                    UserProfile = adminProfile
                };

                await userManager.CreateAsync(admin, SD.DefaultPassword);
                await userManager.AddToRolesAsync(admin, new[] { SD.Admin, SD.Moderator, SD.Player });

                // Sholeh
                var sholehPhoto = new Photo
                {
                    PhotoUrl = "https://randomuser.me/api/portraits/women/1.jpg",
                };

                var sholehProfile = new UserProfile
                {
                    Photo = sholehPhoto,
                    BadgeId = pinkBadgeId,
                    CountryId = worldId,
                    StatusId = offlineStatusId
                };

                var sholeh = new ApplicationUser
                {
                    PlayerName = "Sholeh",
                    Email = "sholeh@test.com",
                    UserName = "sholeh",
                    EmailConfirmed = true,
                    UserProfile = sholehProfile,
                    LockoutEnabled = true
                };

                await userManager.CreateAsync(sholeh, SD.DefaultPassword);
                await userManager.AddToRolesAsync(sholeh, new[] { SD.Moderator, SD.Player });


                // Barb
                var barbPhoto = new Photo
                {
                    PhotoUrl = "https://randomuser.me/api/portraits/women/4.jpg",
                };

                var barbProfile = new UserProfile
                {
                    Photo = barbPhoto,
                    BadgeId = pinkBadgeId,
                    CountryId = worldId,
                    StatusId = offlineStatusId
                };

                var barb = new ApplicationUser
                {
                    PlayerName = "BaRb",
                    Email = "barb@test.com",
                    UserName = "barb",
                    EmailConfirmed = true,
                    UserProfile = barbProfile,
                    LockoutEnabled = true
                };

                await userManager.CreateAsync(barb, SD.DefaultPassword);
                await userManager.AddToRoleAsync(barb, SD.Player);

                // Todd
                var toddPhoto = new Photo
                {
                    PhotoUrl = "https://randomuser.me/api/portraits/men/9.jpg",
                };

                var toddProfile = new UserProfile
                {
                    Photo = toddPhoto,
                    BadgeId = pinkBadgeId,
                    CountryId = worldId,
                    StatusId = offlineStatusId
                };

                var todd = new ApplicationUser
                {
                    PlayerName = "Todd",
                    Email = "todd@test.com",
                    UserName = "todd",
                    EmailConfirmed = true,
                    UserProfile = toddProfile,
                    LockoutEnabled = true
                };

                await userManager.CreateAsync(todd, SD.DefaultPassword);
                await userManager.AddToRoleAsync(todd, SD.Player);


                //var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
                //var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                //var dummyUsers = JsonSerializer.Deserialize<List<ApplicationUser>>(userData);

                //foreach (var user in dummyUsers)
                //{
                //    var badge = await context.Badge.FirstOrDefaultAsync(c => c.Color == SD.GetBadgeColorByRate(user.UserProfile.Rate));
                //    user.UserProfile.Badge = badge;
                //    user.LockoutEnabled = true;
                //    user.EmailConfirmed = true;

                //    await userManager.CreateAsync(user, SD.DefaultPassword);
                //    await userManager.AddToRoleAsync(user, SD.Player);
                //}
            }

            if (context.ChangeTracker.HasChanges())
            {
                await context.SaveChangesAsync();
            }
        }
    }
}
