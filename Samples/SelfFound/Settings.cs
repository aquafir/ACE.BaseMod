using ACE.Entity.Enum.Properties;

namespace SelfFound;

public class Settings
{
    public PropertyInstanceId OriginalFinderId { get; set; } = (PropertyInstanceId)9090;

    public PropertyString OriginalFinderName { get; set; } = (PropertyString)9091;

    public PropertyBool SelfFound { get; set; } = (PropertyBool)9092;

    //Spams player with items made SF
    public bool NotifyClaims { get; set; } = false;

    public bool UseFinderName {get;set;} = true;

    public bool Hardcore { get; set; } = true;
}