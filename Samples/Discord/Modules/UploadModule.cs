namespace Discord.Modules;

public class UploadModule : InteractionModuleBase<SocketInteractionContext>
{
    [Group("upload", "Upload things")]
    public class UploadGroup : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("loot", "Upload loot profile")]
        public async Task UploadLoot(
            [Summary("profile")] IAttachment attachment
            )
        {
            //Size cap of 1 mb
            if (attachment.Size > 1_000_000)
                RespondAsync("File is too large.");

            try
            {
                if (!attachment.Filename.EndsWith(".utl"))
                {
                    RespondAsync("Not a loot profile extension.");
                }
                else
                {
                    //By username?
                    var path = PatchClass.Settings.LootProfileUseUsername ?
                        Path.Combine(PatchClass.Settings.LootProfilePath, Context.User.Username) :
                    PatchClass.Settings.LootProfilePath;
                    Directory.CreateDirectory(path);    //Checks anyways, ensure path exists

                    using (HttpClient client = new HttpClient())
                    {
                        client.Timeout = TimeSpan.FromSeconds(10);

                        var stream = await client.GetStreamAsync(attachment.Url);
                        var filePath = Path.Combine(path, attachment.Filename);

                        // Use the stream as needed (e.g., save to a file)
                        using (var fileStream = File.Create(filePath))
                        {
                            await stream.CopyToAsync(fileStream);
                            await RespondAsync($"Downloaded loot profile: {attachment.Filename}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModManager.Log($"Failed to download attachment.");
            }
        }
    }
}
