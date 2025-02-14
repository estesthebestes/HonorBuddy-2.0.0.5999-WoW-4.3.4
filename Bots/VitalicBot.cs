using Styx;
using Styx.CommonBot;
using Styx.CommonBot.Profiles;
using Styx.CommonBot.Routines;
using Styx.TreeSharp;

namespace VitalicBot
{
    public class VitalicBot : BotBase
    {
        private Composite _root;
        public override string Name { get { return "Vitalic Bot"; } }
        public override Composite Root { get { return _root ?? (_root = RootBehavior); } }
        public override PulseFlags PulseFlags { get { return PulseFlags.Objects | PulseFlags.Lua; } }

        public override void Start()
        {
            ProfileManager.LoadEmpty();
        }

        private Composite RootBehavior
        {
            get
            {
		        return new Switch<bool>(ret => StyxWoW.Me.Combat,
                    new SwitchArgument<bool>(true, RoutineManager.Current.CombatBehavior),
                    new SwitchArgument<bool>(false, RoutineManager.Current.PreCombatBuffBehavior)
                );
            }
        }
    }
}