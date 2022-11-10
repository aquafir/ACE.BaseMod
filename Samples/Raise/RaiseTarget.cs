using ACE.Entity.Enum.Properties;

namespace Raise
{
    public enum RaiseTarget
    {
        //Match attributes with ACE.Entity.Enum.Properties.PropertyAttribute to work with casting
        Str = PropertyAttribute.Strength,
        End = PropertyAttribute.Endurance,
        Quick = PropertyAttribute.Quickness,
        Coord = PropertyAttribute.Coordination,
        Focus = PropertyAttribute.Focus,
        Self = PropertyAttribute.Self,
        //Ratings
        World, Offense, Defense
    }
}
