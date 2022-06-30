using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Gameplay.GameActions;
using System;

namespace SingleTierFarms
{
    public class SingleTierFarmsPlugin : IModKitPlugin, IInitializablePlugin
    {
        public string GetStatus() => "Active";

        public void Initialize(TimedTask timer)
        {
            ActionUtil.AddListener(new PlantSeedsGameActionAware());
        }
    }
}