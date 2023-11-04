using ACE.DatLoader.Entity;
using ACE.Server.Network.GameEvent.Events;
using ACE.Server.Network.Structure;
using System.Diagnostics;
using System.Text;

namespace ExtendACE;

public static class StatusEffectManager
{
    //static readonly HashSet<Creature> stunList = new();

    const float speed = 1f;
    //static uint motionTableId = MotionTableId;
    const MotionCommand command = MotionCommand.Kneel;
    const MotionStance stance = MotionStance.NonCombat;
    private static Motion stunMotion = new(stance, command, speed);

    //Create a spell that is used as a fake stun enchantment
    static Spell stunSpell = new Spell(SpellId.UNKNOWN__GUESSEDNAME10);
    static SpellBase stunBase = stunSpell._spellBase.Clone();

    static uint category = 733;

    public static void Init()
    {

    }

    public static void Stun(this Creature creature, double duration)
    {
        //p.SendMessage($"You have been stunned by {Name}.");

        var spell = new Spell(SpellId.FireProtectionOther1);

        var sb = spell._spellBase;
        sb.MetaSpellType = SpellType.Boost;
        sb.Category = SpellCategory.FireProtection + (category++);
        sb.MetaSpellId = 10000+ category;
        sb.Duration = duration;
        sb.Power = 0;

        sb.Bitfield = (uint)SpellFlags.Beneficial;
        //var enchantmentRegistry = creature.CreateEnchantmentRegistry(spell);


        //spell._Duration = duration;

        //spell._spell = spell._spell.Clone();
        var addResult = creature.CreateEnchantmentResult(spell);
        var enchantment = addResult.Enchantment.CreateEnchantment(spell, creature);

        for (ushort i = 1; i < 100; i++)
        {
            enchantment.SpellID = i;
            enchantment.SpellCategory++;
            //var addResult = creature.EnchantmentManager.Add(spell, creature, null);

            creature.EnchantmentManager.ClearCache();

            if (creature is Player targetPlayer)
            {
                targetPlayer.Session.Network.EnqueueSend(new GameEventMagicUpdateEnchantment(targetPlayer.Session, enchantment));

                targetPlayer.HandleSpellHooks(spell);
            }
        }

        //creature.CreateEnchantment()

        //Get stun duration
        //float motionLength = MotionTable.GetAnimationLength(p.MotionTableId, stance, command, speed);

        ////Add current time before queued stun will play
        //p.GetCurrentMotionState(out var pStance, out var pMotion);
        //if (pStance != MotionStance.Invalid && pMotion != MotionCommand.Ready)
        //    motionLength += MotionTable.GetAnimationLength(p.MotionTableId, stance, command);


        //var actionChain = new ActionChain();
        //actionChain.AddAction(creature, () =>
        //{
        //    //Todo figure out what to actually disable
        //    creature.EnqueueBroadcastMotion(stunMotion);

        //    creature.OnAttackDone();
        //    creature.FailCast(false);
        //});
        //actionChain.AddDelaySeconds(motionLength);
        //actionChain.AddAction(creature, () => creature.SendMessage("The stun has worn off."));
        //actionChain.EnqueueChain();
    }
}
