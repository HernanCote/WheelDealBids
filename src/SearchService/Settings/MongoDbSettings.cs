namespace SearchService.Settings;

public class MongoDbSettings
{
    public required string DatabaseName { get; set; }
    public required string ConnectionString { get; set; }
}