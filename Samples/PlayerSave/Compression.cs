using System.IO.Compression;

namespace PlayerSave;

//https://www.infoworld.com/article/3660629/how-to-compress-and-decompress-strings-in-c-sharp.html
public static class Compression
{
    public static byte[] ToGZip(this string str) => CompressGzip(Encoding.UTF8.GetBytes(str));
    public static byte[] CompressGzip(byte[] bytes)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
            {
                gzipStream.Write(bytes, 0, bytes.Length);
            }
            return memoryStream.ToArray();
        }
    }

    public static string GZipToString(this byte[] bytes) => Encoding.UTF8.GetString(DecompressGzip(bytes));
    public static string GZipToString(this MemoryStream memoryStream) => Encoding.UTF8.GetString(DecompressGzip(memoryStream));
    //Decompress GZipped bytes to a stream
    public static byte[] DecompressGzip(byte[] bytes)
    {
        using (var memoryStream = new MemoryStream(bytes))
        {
            return DecompressGzip(memoryStream);
        }
    }
    private static byte[] DecompressGzip(MemoryStream memoryStream)
    {
        using (var outputStream = new MemoryStream())
        {
            using (var decompressStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                decompressStream.CopyTo(outputStream);
            }
            return outputStream.ToArray();
        }
    }

    public async static Task<byte[]> CompressGzipAsync(this string str) => await CompressGzipAsync(Encoding.UTF8.GetBytes(str));
    public async static Task<byte[]> CompressGzipAsync(byte[] bytes)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
            {
                await gzipStream.WriteAsync(bytes, 0, bytes.Length);
            }
            return memoryStream.ToArray();
        }
    }

    public static async Task<string> GzipToStringAsync(this byte[] bytes) => Encoding.UTF8.GetString(await DecompressGzipAsync(bytes));
    public async static Task<byte[]> DecompressGzipAsync(byte[] bytes)
    {
        using (var memoryStream = new MemoryStream(bytes))
        {
            using (var outputStream = new MemoryStream())
            {
                using (var decompressStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    await decompressStream.CopyToAsync(outputStream);
                }
                return outputStream.ToArray();
            }
        }
    }

    public static byte[] CompressBrotli(this string str) => CompressBrotli(Encoding.UTF8.GetBytes(str));
    public static byte[] CompressBrotli(byte[] bytes)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var brotliStream = new BrotliStream(memoryStream, CompressionLevel.Optimal))
            {
                brotliStream.Write(bytes, 0, bytes.Length);
            }
            return memoryStream.ToArray();
        }
    }

    public static string BrotliToString(this byte[] bytes) => Encoding.UTF8.GetString(DecompressBrotli(bytes));
    public static byte[] DecompressBrotli(byte[] bytes)
    {
        using (var memoryStream = new MemoryStream(bytes))
        {
            using (var outputStream = new MemoryStream())
            {
                using (var decompressStream = new BrotliStream(memoryStream, CompressionMode.Decompress))
                {
                    decompressStream.CopyTo(outputStream);
                }
                return outputStream.ToArray();
            }
        }
    }

    public async static Task<byte[]> CompressBrotliAsync(this string str) => await CompressBrotliAsync(Encoding.UTF8.GetBytes(str));
    public static async Task<byte[]> CompressBrotliAsync(byte[] bytes)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var brotliStream = new BrotliStream(memoryStream, CompressionLevel.Optimal))
            {
                await brotliStream.WriteAsync(bytes, 0, bytes.Length);
            }
            return memoryStream.ToArray();
        }
    }

    public static async Task<string> BrotliToStringAsync(this byte[] bytes) => Encoding.UTF8.GetString(await DecompressBrotliAsync(bytes));
    public static async Task<byte[]> DecompressBrotliAsync(byte[] bytes)
    {
        using (var memoryStream = new MemoryStream(bytes))
        {
            using (var outputStream = new MemoryStream())
            {
                using (var brotliStream = new BrotliStream(memoryStream, CompressionMode.Decompress))
                {
                    await brotliStream.CopyToAsync(outputStream);
                }
                return outputStream.ToArray();
            }
        }
    }
}
