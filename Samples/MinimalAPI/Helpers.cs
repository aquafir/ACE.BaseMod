namespace MinimalAPI;

public static class Helpers
{
    public static void SaveFile(this Stream stream, string destination)
    {
        using (var fileStream = File.Create(destination))
        {
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyToAsync(fileStream);
        }
    }
}