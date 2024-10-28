namespace ACE.MySql;

public class Settings
{
    //Borrow Shard connection settings if a connection string not provided
    public string ConnectionString { get; set; } = "";
    public string DbName { get; set; } = "acex_mymoddb";
}