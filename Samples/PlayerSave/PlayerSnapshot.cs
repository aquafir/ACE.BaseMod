namespace PlayerSave
{

    public class PlayerSnapshot
    {
        public DateTime Time { get; set; } = DateTime.Now;

        public Dictionary<Skill, (uint? InitLevel, double? LastUsed, ushort? LevelFromPP, uint? PP, uint? ResistanceAtLastCheck, SkillAdvancementClass? SAC)> Skills { get; set; } = new();
        public Dictionary<PropertyAttribute, (uint? ExperienceSpent, uint? Ranks, uint? StartingValue)> Attributes { get; set; } = new();
        public Dictionary<Vital, uint> Vitals { get; set; } = new();
        public Dictionary<PropertyInt, int> Ints { get; set; } = new();
        public Dictionary<PropertyInt64, long> Longs { get; set; } = new();
        public Dictionary<PropertyFloat, double> Floats { get; set; } = new();
        public Dictionary<PropertyBool, bool> Bools { get; set; } = new();
        public Dictionary<PropertyString, string> Strings { get; set; } = new();
        public Dictionary<PropertyInstanceId, uint> IIDs { get; set; } = new();
        public Dictionary<PropertyDataId, uint> DIDs { get; set; } = new();
        public Dictionary<PositionType, Position> Positions { get; set; } = new();


        public static PlayerSnapshot Take(Player player)
        {
            //character.
            PlayerSnapshot snapshot = new();
            Character character = player.Character;

            snapshot.Bools = player.GetAllPropertyBools().ToDictionary(e => e.Key, e => e.Value);
            snapshot.DIDs = player.GetAllPropertyDataId().ToDictionary(e => e.Key, e => e.Value);
            snapshot.Floats = player.GetAllPropertyFloat().ToDictionary(e => e.Key, e => e.Value);
            snapshot.IIDs = player.GetAllPropertyInstanceId().ToDictionary(e => e.Key, e => e.Value);
            snapshot.Ints = player.GetAllPropertyInt().ToDictionary(e => e.Key, e => e.Value);
            snapshot.Longs = player.GetAllPropertyInt64().ToDictionary(e => e.Key, e => e.Value);
            snapshot.Strings = player.GetAllPropertyString().ToDictionary(e => e.Key, e => e.Value);

            //player.Vitals
            //snapshot.Skills = player.Skills.ToDictionary(e => e.Key, e => 
            //new {(e.Value.InitLevel, e.Value.LevelFromPP, e.Value.PP, uint ? ResistanceAtLastCheck, SkillAdvancementClass ? SAC)});
            //snapshot.Attributes = player.Attributes.ToDictionary(e => e.Key, e => new { (e.Value.ExperienceSpent, e.Value.Ranks, e.Value.StartingValue) });
            //snapshot = player.ToDictionary(e => e.Key, e => e.Value);

            //foreach (var entry in player.GetAllPropertyBools())
            //{
            //    PropertyBool.
            //}

            return null;
        }

        public static void Apply(Session session)
        {
            var player = session.Player;

            WorldObject wo = null;
            Creature cr = wo as Creature;
            CreatureSkill cs = new(cr, Skill.Alchemy, new PropertiesSkill()
            {
                InitLevel = 0,
                LastUsedTime = 0,
                LevelFromPP = 0,
                PP = 0,
                ResistanceAtLastCheck = 0,
                SAC = SkillAdvancementClass.Inactive
            });

            CreatureVital cv = new(cr, new PropertyAttribute2nd() { });
            CreatureAttribute ca = new(cr, PropertyAttribute.Coordination)
            {
                ExperienceSpent = 0u,
                //ModifierType = ModifierType.None,
                Ranks = 0u,
                StartingValue = 0u,
            };

            // PropertyAttribute2nd
            //wo.GetAllPropertyBools

            GameMessagePrivateUpdateSkill skill = new(wo, cs);
            GameMessagePrivateUpdateAttribute attrs = new(wo, ca);
            //GameMessagePrivateUpdateVital vital = new(wo, cv);  //Difference from attr 2nd level?
            GameMessagePrivateUpdateAttribute2ndLevel attr2nd = new(wo, Vital.Health, 0u);
            GameMessagePrivateUpdatePropertyInt ints = new(wo, PropertyInt.AugmentationInnateCoordination, 0);
            GameMessagePrivateUpdatePropertyInt64 int64s = new(wo, PropertyInt64.AvailableExperience, 0l);
            GameMessagePrivateUpdatePropertyFloat floats = new(wo, PropertyFloat.AbsorbMagicDamage, 0.0);
            GameMessagePrivateUpdatePropertyBool bools = new(wo, PropertyBool.Attackable, false);
            GameMessagePrivateUpdatePropertyString strings = new(wo, PropertyString.DisplayName, "");
            GameMessagePrivateUpdateInstanceID iids = new(wo, PropertyInstanceId.ActivationTarget, 0);
            GameMessagePrivateUpdateDataID dids = new(wo, PropertyDataId.AccountHouseId, 0u);
            GameMessagePrivateUpdatePosition positions = new(wo, PositionType.Target, new Position());
            //var foo = new GameMessagePublicUpdateVital(uint ranks, uint baseValue, uint totalInvestment, uint currentValue)

            //GameMessagePrivateUpdatePropertyInt test = new(worldObject, ACE.Entity.Enum.Properties.PropertyInt.TotalExperience, 0);

            //Also have public properties
            //GameMessagePublicUpdatePropertyInt
        }
    }

    internal record struct SkillRecord(uint? Item1, double? Item2, ushort? Item3, uint? Item4, uint? Item5, SkillAdvancementClass? Item6)
    {
        public static implicit operator (uint?, double?, ushort?, uint?, uint?, SkillAdvancementClass?)(SkillRecord value)
        {
            return (value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6);
        }

        public static implicit operator SkillRecord((uint?, double?, ushort?, uint?, uint?, SkillAdvancementClass?) value)
        {
            return new SkillRecord(value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6);
        }
    }
}
