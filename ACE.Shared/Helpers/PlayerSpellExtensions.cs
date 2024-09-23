namespace ACE.Shared.Helpers;
public static class PlayerSpellExtensions
{
    /// <summary>
    /// Learns a spell, returning true or felse based on 
    /// </summary>
    public static bool LearnSpellById(this Player player, SpellId spellId)
    {
        if (Enum.IsDefined(typeof(SpellId), spellId))
        {
            return player.TryLearnSpell((uint)spellId);

        }

        return false;
    }

    /// <summary>
    /// Reimplementation of ACE version returning true if the spell was learned successfully
    /// </summary>
    public static bool TryLearnSpell(this Player player, uint spellId, bool uiOutput = true)
    {
        var spells = DatManager.PortalDat.SpellTable;

        if (!spells.Spells.ContainsKey(spellId))
        {
            GameMessageSystemChat errorMessage = new GameMessageSystemChat("SpellID not found in Spell Table", ChatMessageType.Broadcast);
            player.Session.Network.EnqueueSend(errorMessage);
            return false;
        }

        if (!player.AddKnownSpell(spellId))
        {
            if (uiOutput)
            {
                GameMessageSystemChat errorMessage = new GameMessageSystemChat("You already know that spell!", ChatMessageType.Broadcast);
                player.Session.Network.EnqueueSend(errorMessage);
            }
            return false;
        }

        GameEventMagicUpdateSpell updateSpellEvent = new GameEventMagicUpdateSpell(player.Session, (ushort)spellId);
        player.Session.Network.EnqueueSend(updateSpellEvent);

        // Check to see if we echo output to the client text area and do playscript animation
        if (uiOutput)
        {
            // Always seems to be this SkillUpPurple effect
            player.ApplyVisualEffects(PlayScript.SkillUpPurple);

            string message = $"You learn the {spells.Spells[spellId].Name} spell.\n";
            GameMessageSystemChat learnMessage = new GameMessageSystemChat(message, ChatMessageType.Broadcast);
            player.Session.Network.EnqueueSend(learnMessage);
        }
        else
        {
            player.Session.Network.EnqueueSend(new GameEventCommunicationTransientString(player.Session, "You have learned a new spell."));
        }

        return true;
    }


    public static void RemoveAllSpells(this Player player)//, bool withNetworking = true)
    {
        player.Biota.ClearSpells(player.BiotaDatabaseLock);
    }

    /// <summary>
    /// Counterpart to AddSpellsInBulk
    /// </summary>
    public static void RemoveSpellsInBulk(this Player player, MagicSchool school, uint spellLevel, bool withNetworking = true)
    {
        var spellTable = DatManager.PortalDat.SpellTable;

        foreach (var spellId in Player.PlayerSpellTable)
        {
            if (!spellTable.Spells.ContainsKey(spellId))
            {
                Console.WriteLine($"Unknown spell ID in PlayerSpellID table: {spellId}");
                continue;
            }

            if (player.RemoveKnownSpell(spellId))
            {
                var spell = new Spell(spellId, false);
                if (withNetworking)
                    player.Session.Network.EnqueueSend(new GameMessageSystemChat($"{spell.Name} removed from spellbook.", ChatMessageType.Broadcast));
            }

            //var spell = new Spell(spellId, false);
            //if (spell.School == school && spell.Formula.Level == spellLevel)
            //{
            //    if (withNetworking)
            //        player.LearnSpellWithNetworking(spell.Id, false);
            //    else
            //        player.AddKnownSpell(spell.Id);
            //}
        }
    }


    public static void UpdateSpellbook(this Player player)
    {
        var session = player.Session;
        foreach (var spell in player.Biota.PropertiesSpellBook)
        {
            GameEventMagicUpdateSpell updateSpellEvent = new GameEventMagicUpdateSpell(session, (ushort)spell.Key);
        }
    }
}
