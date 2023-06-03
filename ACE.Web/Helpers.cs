namespace ACE.Web;

public static class Helpers
{
    /// <summary>
    /// Save a stream to a file destination
    /// </summary>
    public static void SaveFile(this Stream stream, string destination)
    {
        using (var fileStream = File.Create(destination))
        {
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyToAsync(fileStream);
        }
    }
}