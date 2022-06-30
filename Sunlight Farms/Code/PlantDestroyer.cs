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

namespace SunlightFarms
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
        /// Checks whether there are any blocks above this that would block the light. Glass and framed glass can be used without blocking sunlight
        /// </summary>
        /// <param name="plantLocation"></param>
        /// <returns>Whether there is a view right up to the sun</returns>
        private static bool HasSunlight(Vector3i plantLocation)
        {
            //Look at every block above the plant
            for (int i = 1; i <= World.MaxLandOrWaterUnwrapped(plantLocation.XZ); i++)
            {
                Vector3i position = plantLocation + Vector3i.Up * i;
                Block block = World.GetBlock(position);
                //Only solid blocks that aren't glass or framed glass block the sun
                if (block.Is<Solid>())
                {
                    //Dirt, stone, constructed blocks and world objects are all represented by an item
                    if (block is IRepresentsItem)
                    {
                        Type representedItemType = ((IRepresentsItem)block).RepresentedItemType;
                        if (representedItemType != typeof(GlassItem) && representedItemType != typeof(FramedGlassItem))
                        {
                            return false;
                        }
                    }
                    //If it's some other type of block that's okay. I don't know what blocks these might be though
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}