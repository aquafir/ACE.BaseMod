namespace ACE.Shared.Settings;
public interface ISettings
{
    Task CreateAsync();
    Task CreateOrLoadAsync();
}
