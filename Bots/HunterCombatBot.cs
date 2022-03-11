using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Levelbot.Actions.Combat;
using Styx.Combat.CombatRoutine;
using Styx.Logic;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles;
using Styx.Logic.POI;
using Styx.Logic.Profiles;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Action = TreeSharp.Action;
using Sequence = TreeSharp.Sequence;
using Styx.WoWInternals;

namespace Styx.Bot.CustomBots
{
    public class CombatBot : BotBase
    {
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);
    
        #region Overrides of BotBase

        public override string Name 
        {
            get { return "CombatBot"; }
        }    

        private Composite _root;
        public override Composite Root
        {
            get 
            {
                return _root ?? (_root =
                    new PrioritySelector(
                        CreateCombatBehavior()
                        )
                    );
             }

         }
   



        public override void Initialize()
            {
             BotEvents.Player.OnMapChanged += Player_OnMapChanged;
             }

        private void Player_OnMapChanged(BotEvents.Player.MapChangedEventArgs args)
        {
            _root = null; 
        }

        public override void Pulse(){}

        public override PulseFlags Pulseflags 
        {
             get { return PulseFlags.All;}
        }

        public override void Start()
        {
            Targeting.Instance.IncludeTargetsFilter += IncludeTargetsFilter;

        }
    }
}

#endregion