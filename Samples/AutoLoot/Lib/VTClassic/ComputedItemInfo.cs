///////////////////////////////////////////////////////////////////////////////
//File: ComputedItemInfo.cs
//
//Description: Helper class for adding up buffed values for computed requirements.
//  Originally from the Mag edition.
//
//This file is Copyright (c) 2009-2010 VirindiPlugins
//
//The original copy of this code can be obtained from http://www.virindi.net/repos/virindi_public
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////

using AutoLoot.Lib;

namespace AutoLoot.Lib.VTClassic
{
    internal class ComputedItemInfo
    {
        private readonly WorldObject gameItemInfo;

        public ComputedItemInfo(WorldObject gameItemInfo)
        {
            this.gameItemInfo = gameItemInfo;
        }

        struct SpellInfo<T>
        {
            public readonly T Key;
            public readonly double Change;
            public readonly double Bonus;

            public SpellInfo(T key, double change)
                : this(key, change, 0)
            {
            }

            public SpellInfo(T key, double change, double bonus)
            {
                Key = key;
                Change = change;
                Bonus = bonus;
            }
        }

        static ComputedItemInfo()
        {
            LongValueKeySpellEffects[1616] = new SpellInfo<IntValueKey>(IntValueKey.MaxDamage, 20); // Blood Drinker VI
            LongValueKeySpellEffects[2096] = new SpellInfo<IntValueKey>(IntValueKey.MaxDamage, 22); // Infected Caress
            LongValueKeySpellEffects[5183] = new SpellInfo<IntValueKey>(IntValueKey.MaxDamage, 24); // Incantation of Blood Drinker Post Feb-2013
            LongValueKeySpellEffects[4395] = new SpellInfo<IntValueKey>(IntValueKey.MaxDamage, 24); // Incantation of Blood Drinker Post Feb-2013
            LongValueKeySpellEffects[2598] = new SpellInfo<IntValueKey>(IntValueKey.MaxDamage, 2, 2); // Minor Blood Thirst
            LongValueKeySpellEffects[2586] = new SpellInfo<IntValueKey>(IntValueKey.MaxDamage, 4, 4); // Major Blood Thirst
            LongValueKeySpellEffects[4661] = new SpellInfo<IntValueKey>(IntValueKey.MaxDamage, 7, 7); // Epic Blood Thirst
            LongValueKeySpellEffects[6089] = new SpellInfo<IntValueKey>(IntValueKey.MaxDamage, 10, 10); // Legendary Blood Thirst
            LongValueKeySpellEffects[3688] = new SpellInfo<IntValueKey>(IntValueKey.MaxDamage, 300); // Prodigal Blood Drinker
            LongValueKeySpellEffects[2604] = new SpellInfo<IntValueKey>(IntValueKey.ArmorLevel, 20, 20); // Minor Impenetrability
            LongValueKeySpellEffects[2592] = new SpellInfo<IntValueKey>(IntValueKey.ArmorLevel, 40, 40); // Major Impenetrability
            LongValueKeySpellEffects[4667] = new SpellInfo<IntValueKey>(IntValueKey.ArmorLevel, 60, 60); // Epic Impenetrability
            LongValueKeySpellEffects[6095] = new SpellInfo<IntValueKey>(IntValueKey.ArmorLevel, 80, 80); // Legendary Impenetrability



            DoubleValueKeySpellEffects[3258] = new SpellInfo<DoubleValueKey>(DoubleValueKey.ElementalDamageVersusMonsters, .06); // Spirit Drinker VI
            DoubleValueKeySpellEffects[3259] = new SpellInfo<DoubleValueKey>(DoubleValueKey.ElementalDamageVersusMonsters, .07); // Infected Spirit Caress
            DoubleValueKeySpellEffects[5182] = new SpellInfo<DoubleValueKey>(DoubleValueKey.ElementalDamageVersusMonsters, .08); // Incantation of Spirit Drinker Post Feb-2013
            DoubleValueKeySpellEffects[4414] = new SpellInfo<DoubleValueKey>(DoubleValueKey.ElementalDamageVersusMonsters, .08); // Incantation of Spirit Drinker, this spell on the item adds 1 more % of damage over a user casted 8 Post Feb-2013

            DoubleValueKeySpellEffects[3251] = new SpellInfo<DoubleValueKey>(DoubleValueKey.ElementalDamageVersusMonsters, .01, .01); // Minor Spirit Thirst
            DoubleValueKeySpellEffects[3250] = new SpellInfo<DoubleValueKey>(DoubleValueKey.ElementalDamageVersusMonsters, .03, .03); // Major Spirit Thirst
            DoubleValueKeySpellEffects[4670] = new SpellInfo<DoubleValueKey>(DoubleValueKey.ElementalDamageVersusMonsters, .05, .05); // Epic Spirit Thirst
            DoubleValueKeySpellEffects[6098] = new SpellInfo<DoubleValueKey>(DoubleValueKey.ElementalDamageVersusMonsters, .07, .07); // Legendary Spirit Thirst

            DoubleValueKeySpellEffects[3735] = new SpellInfo<DoubleValueKey>(DoubleValueKey.ElementalDamageVersusMonsters, .15); // Prodigal Spirit Drinker


            DoubleValueKeySpellEffects[1592] = new SpellInfo<DoubleValueKey>(DoubleValueKey.AttackBonus, .15); // Heart Seeker VI
            DoubleValueKeySpellEffects[2106] = new SpellInfo<DoubleValueKey>(DoubleValueKey.AttackBonus, .17); // Elysa's Sight
            DoubleValueKeySpellEffects[4405] = new SpellInfo<DoubleValueKey>(DoubleValueKey.AttackBonus, .20); // Incantation of Heart Seeker

            DoubleValueKeySpellEffects[2603] = new SpellInfo<DoubleValueKey>(DoubleValueKey.AttackBonus, .03, .03); // Minor Heart Thirst
            DoubleValueKeySpellEffects[2591] = new SpellInfo<DoubleValueKey>(DoubleValueKey.AttackBonus, .05, .05); // Major Heart Thirst
            DoubleValueKeySpellEffects[4666] = new SpellInfo<DoubleValueKey>(DoubleValueKey.AttackBonus, .07, .07); // Epic Heart Thirst
            DoubleValueKeySpellEffects[6094] = new SpellInfo<DoubleValueKey>(DoubleValueKey.AttackBonus, .09, .09); // Legendary Heart Thirst


            DoubleValueKeySpellEffects[1605] = new SpellInfo<DoubleValueKey>(DoubleValueKey.MeleeDefenseBonus, .15); // Defender VI
            DoubleValueKeySpellEffects[2101] = new SpellInfo<DoubleValueKey>(DoubleValueKey.MeleeDefenseBonus, .17); // Cragstone's Will
            DoubleValueKeySpellEffects[4400] = new SpellInfo<DoubleValueKey>(DoubleValueKey.MeleeDefenseBonus, .20); // Incantation of Defender Post Feb-2013

            DoubleValueKeySpellEffects[2600] = new SpellInfo<DoubleValueKey>(DoubleValueKey.MeleeDefenseBonus, .03, .03); // Minor Defender
            DoubleValueKeySpellEffects[3985] = new SpellInfo<DoubleValueKey>(DoubleValueKey.MeleeDefenseBonus, .04, .04); // Mukkir Sense
            DoubleValueKeySpellEffects[2588] = new SpellInfo<DoubleValueKey>(DoubleValueKey.MeleeDefenseBonus, .05, .05); // Major Defender
            DoubleValueKeySpellEffects[4663] = new SpellInfo<DoubleValueKey>(DoubleValueKey.MeleeDefenseBonus, .07, .07); // Epic Defender
            DoubleValueKeySpellEffects[6091] = new SpellInfo<DoubleValueKey>(DoubleValueKey.MeleeDefenseBonus, .09, .09); // Legendary Defender

            DoubleValueKeySpellEffects[3699] = new SpellInfo<DoubleValueKey>(DoubleValueKey.MeleeDefenseBonus, .25); // Prodigal Defender


            DoubleValueKeySpellEffects[1480] = new SpellInfo<DoubleValueKey>(DoubleValueKey.ManaCBonus, 1.60); // Hermetic Link VI
            DoubleValueKeySpellEffects[2117] = new SpellInfo<DoubleValueKey>(DoubleValueKey.ManaCBonus, 1.70); // Mystic's Blessing
            DoubleValueKeySpellEffects[4418] = new SpellInfo<DoubleValueKey>(DoubleValueKey.ManaCBonus, 1.80); // Incantation of Hermetic Link

            DoubleValueKeySpellEffects[3201] = new SpellInfo<DoubleValueKey>(DoubleValueKey.ManaCBonus, 1.05, 1.05); // Feeble Hermetic Link
            DoubleValueKeySpellEffects[3199] = new SpellInfo<DoubleValueKey>(DoubleValueKey.ManaCBonus, 1.10, 1.10); // Minor Hermetic Link
            DoubleValueKeySpellEffects[3202] = new SpellInfo<DoubleValueKey>(DoubleValueKey.ManaCBonus, 1.15, 1.15); // Moderate Hermetic Link
            DoubleValueKeySpellEffects[3200] = new SpellInfo<DoubleValueKey>(DoubleValueKey.ManaCBonus, 1.20, 1.20); // Major Hermetic Link
            DoubleValueKeySpellEffects[6086] = new SpellInfo<DoubleValueKey>(DoubleValueKey.ManaCBonus, 1.25, 1.25); // Epic Hermetic Link
            DoubleValueKeySpellEffects[6087] = new SpellInfo<DoubleValueKey>(DoubleValueKey.ManaCBonus, 1.30, 1.30); // Legendary Hermetic Link
        }

        static readonly Dictionary<int, SpellInfo<IntValueKey>> LongValueKeySpellEffects = new Dictionary<int, SpellInfo<IntValueKey>>();
        static readonly Dictionary<int, SpellInfo<DoubleValueKey>> DoubleValueKeySpellEffects = new Dictionary<int, SpellInfo<DoubleValueKey>>();

        public double BuffedAverageDamage
        {
            get
            {
                double variance = gameItemInfo.GetValueDouble(DoubleValueKey.Variance, 0.0);
                int maxDamage = GetBuffedLogValueKey(IntValueKey.MaxDamage);
                double minDamage = maxDamage - variance * maxDamage;

                return (minDamage + maxDamage) / 2;
            }
        }

        public double CalcedBuffedTinkedDamage
        {
            get
            {
                double variance = gameItemInfo.GetValueDouble(DoubleValueKey.Variance, 0.0);
                int maxDamage = GetBuffedLogValueKey(IntValueKey.MaxDamage);

                int numberOfTinksLeft = Math.Max(10 - gameItemInfo.GetValueInt(IntValueKey.NumberTimesTinkered, 0), 0);

                if (gameItemInfo.GetValueInt(IntValueKey.Imbued, 0) == 0)
                    numberOfTinksLeft--; // Factor in an imbue tink

                // If this is not a loot generated item, it can't be tinked
                if (gameItemInfo.GetValueInt(IntValueKey.Material, 0) == 0)
                    numberOfTinksLeft = 0;

                for (int i = 1; i <= numberOfTinksLeft; i++)
                {
                    double ironTinkDoT = CalculateDamageOverTime(maxDamage + 24 + 1, variance);
                    double graniteTinkDoT = CalculateDamageOverTime(maxDamage + 24, variance * .8);

                    if (ironTinkDoT >= graniteTinkDoT)
                        maxDamage++;
                    else
                        variance *= .8;
                }

                return CalculateDamageOverTime(maxDamage + 24, variance);
            }
        }

        public bool CanReachTargetValues(double targetCalcedBuffedTinkedDoT, double targetBuffedMeleeDefenseBonus, double targetBuffedAttackBonus)
        {
            double buffedMeleeDefenseBonus = GetBuffedDoubleValueKey(DoubleValueKey.MeleeDefenseBonus);
            double buffedAttackBonus = GetBuffedDoubleValueKey(DoubleValueKey.AttackBonus);

            double variance = gameItemInfo.GetValueDouble(DoubleValueKey.Variance, 0.0);
            int maxDamage = GetBuffedLogValueKey(IntValueKey.MaxDamage);

            int numberOfTinksLeft = Math.Max(10 - gameItemInfo.GetValueInt(IntValueKey.NumberTimesTinkered, 0), 0);

            if (gameItemInfo.GetValueInt(IntValueKey.Imbued, 0) == 0)
                numberOfTinksLeft--; // Factor in an imbue tink

            // If this is not a loot generated item, it can't be tinked
            if (gameItemInfo.GetValueInt(IntValueKey.Material, 0) == 0)
                numberOfTinksLeft = 0;

            for (int i = 1; i <= numberOfTinksLeft; i++)
            {
                if (buffedMeleeDefenseBonus < targetBuffedMeleeDefenseBonus)
                    buffedMeleeDefenseBonus += .01;
                else if (buffedAttackBonus < targetBuffedAttackBonus)
                    buffedAttackBonus += .01;
                else
                {
                    double ironTinkDoT = CalculateDamageOverTime(maxDamage + 24 + 1, variance);
                    double graniteTinkDoT = CalculateDamageOverTime(maxDamage + 24, variance * .8);

                    if (ironTinkDoT >= graniteTinkDoT)
                        maxDamage++;
                    else
                        variance *= .8;
                }
            }

            return CalculateDamageOverTime(maxDamage + 24, variance) >= targetCalcedBuffedTinkedDoT && buffedMeleeDefenseBonus >= targetBuffedMeleeDefenseBonus && buffedAttackBonus >= targetBuffedAttackBonus;
        }

        public int TotalRatings
        {
            get
            {
                /*
                DamRating = 370,
                DamResRating = 371,
                CritRating = 372,
                CritResistRating = 373,
                CritDamRating = 374,
                CritDamResistRating = 375,
                HealBoostRating = 376,
                VitalityRating = 379,
                */
                return gameItemInfo.GetValueInt((IntValueKey)370, 0) + gameItemInfo.GetValueInt((IntValueKey)371, 0) + gameItemInfo.GetValueInt((IntValueKey)372, 0) + gameItemInfo.GetValueInt((IntValueKey)373, 0) + gameItemInfo.GetValueInt((IntValueKey)374, 0) + gameItemInfo.GetValueInt((IntValueKey)375, 0) + gameItemInfo.GetValueInt((IntValueKey)376, 0) + gameItemInfo.GetValueInt((IntValueKey)379, 0);
            }
        }

        public static double CalculateDamageOverTime(int maxDamage, double variance)
        {
            return CalculateDamageOverTime(maxDamage, variance, 0.1d, 2d);
        }

        /// <summary>
        /// maxDamage * ((1 - critChance) * (2 - variance) / 2 + (critChance * critMultiplier));
        /// </summary>
        /// <param name="maxDamage"></param>
        /// <param name="variance"></param>
        /// <param name="critChance"></param>
        /// <param name="critMultiplier"></param>
        /// <returns></returns>
        public static double CalculateDamageOverTime(int maxDamage, double variance, double critChance, double critMultiplier)
        {
            return maxDamage * ((1 - critChance) * (2 - variance) / 2 + critChance * critMultiplier);
        }

        public double BuffedMissileDamage
        {
            get
            {
                return GetBuffedLogValueKey(IntValueKey.MaxDamage) + (GetBuffedDoubleValueKey(DoubleValueKey.DamageBonus) - 1) * 100 / 3 + GetBuffedLogValueKey(IntValueKey.ElementalDmgBonus);
            }
        }

        public int GetBuffedLogValueKey(IntValueKey key)
        {
            return GetBuffedLogValueKey(key, 0);
        }

        public int GetBuffedLogValueKey(IntValueKey key, int defaultValue)
        {
            int value = gameItemInfo.GetValueInt(key, defaultValue);
            var spells = gameItemInfo.GetSpells();
            for (int i = 0; i < spells.Count; i++)
            {
                int spellId = (int)spells[i].Id;

                if (LongValueKeySpellEffects.ContainsKey(spellId) && LongValueKeySpellEffects[spellId].Key == key && LongValueKeySpellEffects[spellId].Bonus != 0)
                    value += (int)LongValueKeySpellEffects[spellId].Bonus;
            }

            return value;
        }

        public double GetBuffedDoubleValueKey(DoubleValueKey key)
        {
            return GetBuffedDoubleValueKey(key, 0d);
        }

        public double GetBuffedDoubleValueKey(DoubleValueKey key, double defaultValue)
        {
            double value = gameItemInfo.GetValueDouble(key, defaultValue);
            var spells = gameItemInfo.GetSpells();
            for (int i = 0; i < spells.Count; i++)
            {
                int spellId = (int)spells[i].Id;

                if (DoubleValueKeySpellEffects.ContainsKey(spellId) && DoubleValueKeySpellEffects[spellId].Key == key && DoubleValueKeySpellEffects[spellId].Bonus != 0)
                {
                    if ((int)DoubleValueKeySpellEffects[spellId].Change == 1)
                        value *= DoubleValueKeySpellEffects[spellId].Bonus;
                    else
                        value += DoubleValueKeySpellEffects[spellId].Bonus;
                }
            }

            return value;
        }
    }
}
