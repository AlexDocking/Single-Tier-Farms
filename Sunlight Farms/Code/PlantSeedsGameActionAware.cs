using Eco.Core.Utils;
using Eco.Gameplay.GameActions;
using Eco.Gameplay.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunlightFarms
{
    internal class PlantSeedsGameActionAware : IGameActionAware
    {
        public void ActionPerformed(GameAction action)
        {
            if (action is PlantSeeds)
            {
                OnSeedsPlanted(action as PlantSeeds);
            }
        }

        public LazyResult ShouldOverrideAuth(GameAction action)
        {
            return LazyResult.FailedNoMessage;
        }
        private void OnSeedsPlanted(PlantSeeds plantSeeds)
        {
            if (PlantDestroyer.DestroyPlantIfNecessary(plantSeeds.ActionLocation))
            {
                User user = plantSeeds.Citizen;
                user.MsgLocStr("Plants needs sunlight!", Eco.Shared.Services.NotificationStyle.Error);
                plantSeeds.CurrentPack.EarlyResult = Result.FailLocStr("Couldn't plant without sun");
            }
        }
    }
}