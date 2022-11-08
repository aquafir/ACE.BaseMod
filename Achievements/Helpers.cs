namespace Achievements
{
    static class Helpers
    {
        private static async Task WriteWithRetry(this FileInfo file, TimeSpan retryDelay, string content, int retryCount = 5)
        {
            for (;;)
            {
                try
                {
                    //Get a stream.  OpenRead only allows read share
                    await File.WriteAllTextAsync(file.FullName, content);
                    return;
                }
                //https://learn.microsoft.com/en-us/dotnet/standard/io/handling-io-errors
                catch (IOException) //when ((ex.HResult & 0x0000FFFF) == 32)
                {
                    //Eat IO exceptions while retrying
                    if (0 <= retryCount--)
                    {
                        throw;
                    }
                }

                await Task.Delay(retryDelay);
            }
        }

        private static async Task<string> ReadWithRetry(this FileInfo file, TimeSpan retryDelay, int retryCount = 5)
        {
            for(;;) 
            {
                try
                {
                    //Get a stream.  OpenRead only allows read share
                    StreamReader reader = new(file.OpenRead());

                    //Return stream if there's no problem
                    return await reader.ReadToEndAsync();
                }
                //https://learn.microsoft.com/en-us/dotnet/standard/io/handling-io-errors
                catch (IOException) //when ((ex.HResult & 0x0000FFFF) == 32)
                {
                    //Eat IO exceptions while retrying
                    if(0 <= retryCount--)
                    {
                        throw;
                    }
                }

                await Task.Delay(retryDelay);
            }
        }
    }
}