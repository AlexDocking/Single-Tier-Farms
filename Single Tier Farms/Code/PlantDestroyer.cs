using Eco.Gameplay.Items;
using Eco.Gameplay.Plants;
using Eco.Mods.TechTree;
using Eco.Shared.Math;
using Eco.Shared.Utils;
using Eco.Simulation.Agents;
using Eco.World;
using Eco.World.Blocks;
using System;
using System.Collections.Generic;

namespace SingleTierFarms
{
    internal static class PlantDestroyer
    {
        /// <summary>
        /// If the plant has no overhead sunlight then destroy it
        /// </summary>
        /// <param name="plantLocation"></param>
        /// <returns>Whether the plant was destroyed</returns>
        internal static bool DestroyPlantIfNecessary(Vector3i plantLocation)
        {
            if (!HasSunlight(plantLocation))
            {
                DestroyPlantAt(plantLocation);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Destroys the plant at a given location
        /// </summary>
        /// <param name="plantLocation"></param>
        private static void DestroyPlantAt(Vector3i plantLocation)
        {
            List<BlockChange> blockChanges = new List<BlockChange>();
            Plant plant = PlantBlock.GetPlant(plantLocation);
            PlantBlock plantBlock = World.GetBlock(plantLocation) as PlantBlock;
            if (plant != null)
            {
                plant.Destroy();
                BlockChange blockChange = new BlockChange();
                blockChange.Position = plantLocation;
                blockChange.BlockType = typeof(EmptyBlock);
                blockChanges.Add(blockChange);
            }
            if (plantBlock != null)
            {
                plantBlock.Destroyed(plantLocation, Block.Empty);
            }
            if (plant != null || plantBlock != null)
            {
                BlockChange blockChange = new BlockChange();
                blockChange.Position = plantLocation;
                blockChange.BlockType = typeof(EmptyBlock);
                blockChanges.Add(blockChange);
            }
            World.BatchApply(blockChanges);
        }

        /// <summary>
        /// Only dirt and tilled dirt should block planting as all multi-tiered farms rely on there being dirt on all levels, while roofs can be allowed
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        private static bool AllowsSunlightToPassThrough(Block block)
        {
            if (block is DirtBlock || block is TilledDirtBlock)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Checks whether there are any blocks above this that would block the light
        /// </summary>
        /// <param name="plantLocation"></param>
        /// <returns>Whether there is a view right up to the sun</returns>
        private static bool HasSunlight(Vector3i plantLocation)
        {
            //Look at every block above the plant and stop when we get to an opaque block or we reach the top of the world
            for (int i = 1; i <= World.MaxLandOrWaterUnwrapped(plantLocation.XZ); i++)
            {
                Vector3i position = plantLocation + Vector3i.Up * i;
                Block block = World.GetBlock(position);
                if (!AllowsSunlightToPassThrough(block))
                {
                    return false;
                }
            }
            return true;
        }
    }
}