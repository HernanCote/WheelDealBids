namespace AuctionService.Utils;

using Data;

public class SeedInitialData
{
    public static void Initialize(WebApplication app)
    {
        try
        {
            DbInitializer.InitializeDatabase(app);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}