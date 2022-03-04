using System.Linq;
using Singular.Dynamics;
using Singular.Helpers;
using Singular.Managers;
using Singular.Settings;
using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Logic.Combat;
using Styx.Logic.Inventory;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;

namespace Singular.ClassSpecific
{
    public static class HunterElementalShaman
    {
        #region Common 

        [Spec(TalentSpec.ElementalShaman)]
        [Behavior(BehaviorType.PreCombatBuffs)]
        [Class(WoWClass.Shaman)]
        [Context(WoWContext.All)]
        public static Composite CreateElementalShamanPreCombatBuffs()
        {
            return new PrioritySelector(
                new Decorator(
                    ret => StyxWoW.Me.Inventory.Equipped.MainHand.TemporaryEnchantment.Id != 283 && StyxWoW.Me.Inventory.Equipped.MainHand.ItemInfo.WeaponClass != WoWItemWeaponClass.FishingPole && SpellManager.HasSpell("Flametongue Weapon") &&
                    SpellManager.CanCast("Flametongue Weapon", null, false, false),
                new Sequence(
                    new Action(ret => Lua.DoString("CancelItemTempEnchantment(1)")),
                    new Action(ret => Logger.Write("Imbuing main hand weapon with Flametongue")),
                    new Action(ret => SpellManager.Cast("Flametongue Weapon", null))

                )),

                Spell.Cast("Lighting Shield", ret => StyxWoW.Me, ret => !StyxWoW.Me.HasAura("Lighting shield", 2)),
                new Decorator(ret => Totems.NeedToRecallTotems,
                    new Action(ret => Totems.RecallTotems())),

                Spell.Cast("Ancestral Guidance", ret => StyxWoW.Me, ret => !StyxWoW.Me.HasAura("Ancestral Guidance", null)),
                new Decorator(ret => Totems.NeedToRecallTotems, 
                    new Action(ret => Totems.RecallTotems()))
            );
        }

        public static Composite CreateElementalShamanRest()
        {
            return 
                new PrioritySelectorO(
                    new Decorator(
                        ret => !StyxWoW.Me.HasAura("Drink") && !StyxWoW.Me.HasAura("Food"),
                        CreateElementalShamanHeal()),
                    Rest.CreateDefaultRestBehaviour(),
                    Spell.Resurrect("Ancestral Spirit")
                    );
        }

        [Class(WoWClass.Shaman)]
        [Spec(TalentSpec.ElementalShaman)]
        [Behavior(BehaviorType.Heal)]
        [Context(WoWContext.Instances)]
        [Context(WoWContext.Normal)]

          public static Composite CreateElementalShamanHeal()
        {
            return
                new Decorator(
                    ret => SingularSettings.Instance.Shaman.EnhancementHeal,
                    new PrioritySelector(
                        // Heal the party in dungeons if the healer is dead
                        new Decorator(
                            ret => StyxWoW.Me.CurrentMap.IsDungeon && !StyxWoW.Me.IsInRaid &&
                                   Group.Healers.Count(h => h.IsAlive) == 0,
                            Restoration.CreateRestoShamanHealingOnlyBehavior()),

                        // This will work for both solo play and battlegrounds
                        new Decorator(
                            ret => Group.Healers.Any() && Group.Healers.Count(h => h.IsAlive) == 0,
                            new PrioritySelector(
                                Spell.Heal("Healing Wave",
                                    ret => StyxWoW.Me,
                                    ret => !SpellManager.HasSpell("Healing Surge") && StyxWoW.Me.HealthPercent <= 40),

                                Spell.Heal("Healing Surge",
                                    ret => StyxWoW.Me,
                                    ret => StyxWoW.Me.HealthPercent <= 40)))
                        ));
        }

        #endregion

        #region Normal Rotation 

        [Class(WoWClass.Shaman)]
        [Spec(TalentSpec.ElementalShaman)]
        [Behavior(BehaviorType.Pull)]
        [Context(BehaviorType.Normal)]

        public static Composite CreateElementalShamanNormalPull()
        {

        }
        #endregion
    
    }
}