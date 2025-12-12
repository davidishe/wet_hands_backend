cd WetHands.WebAPI
# dotnet ef database drop --force --context AppDbContext
rm -rf /Users/akobiyada/Documents/SpecialProjects/30_PROPERTY_CAT/wet_hands_backend/WetHands.Infrastructure.Database/Migrations
dotnet ef migrations add mew_migration_Infrastructure_6 -c AppDbContext --project /Users/akobiyada/Documents/SpecialProjects/30_PROPERTY_CAT/wet_hands_backend/WetHands.Infrastructure.Database/WetHands.Infrastructure.Database.csproj
dotnet ef database update -c AppDbContext --project /Users/akobiyada/Documents/SpecialProjects/30_PROPERTY_CAT/wet_hands_backend/WetHands.Infrastructure.Database/WetHands.Infrastructure.Database.csproj
dotnet ef migrations script -c AppDbContext --project /Users/akobiyada/Documents/SpecialProjects/30_PROPERTY_CAT/wet_hands_backend/WetHands.Infrastructure.Database/WetHands.Infrastructure.Database.csproj
rm -rf /Users/akobiyada/Documents/SpecialProjects/30_PROPERTY_CAT/wet_hands_backend/WetHands.Infrastructure.Database/Migrations

