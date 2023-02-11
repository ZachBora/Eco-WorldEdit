using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using System.Threading.Tasks;
using Eco.Gameplay.Players;
using Eco.Mods.WorldEdit.Utils;
using Eco.Shared.Math;
using Eco.Shared.Utils;
using Eco.World;

namespace Eco.Mods.WorldEdit.Commands
{
	internal class CylCommand : WorldEditCommand
	{
		private Type blockType;
		private double widthNorthSouth;
		private double widthWestEast;
		private int height;
		private bool filled;

		public CylCommand(User user, string blockType, int widthNS, int widthWE, int height, bool filled = true) : base(user)
		{
			if (!this.UserSession.Selection.IsSet()) throw new WorldEditCommandException("Please set both points first!");
			this.blockType = BlockUtils.GetBlockType(blockType) ?? throw new WorldEditCommandException($"No BlockType with name {blockType} found!");
			this.widthNorthSouth = widthNS;
			this.widthWestEast = widthWE;
			this.height = height;
			this.filled = filled;
		}

		protected override void Execute(WorldRange selection)
		{
			WorldEditBlockManager blockManager = new WorldEditBlockManager(this.UserSession);

            List<Vector3i> cylinderPoints = CylBuilder(selection.min);

            foreach(Vector3i point in cylinderPoints)
            { 
                if (!WorldEditBlockManager.IsImpenetrable(point))
                {
                    this.AddBlockChangedEntry(point);
                    blockManager.SetBlock(blockType, point);
                    this.BlocksChanged++;
                }
            }
        }

        private List<Vector3i> CylBuilder(Vector3i center)
		{
            List<Vector3i> cylinderPoints = new();

            this.widthNorthSouth += 0.5;
            this.widthWestEast += 0.5;

            if (this.height == 0) { return cylinderPoints; }
            else if (this.height < 0)
            {
                this.height = -height;
                center = center.AddY(this.height);
            }

            if (center.Y < 1)
            {
                center.Y = 1;
            }

            double invRadiusX = 1 / this.widthNorthSouth;
            double invRadiusZ = 1 / this.widthWestEast;

            int ceilRadiusX = (int)Math.Ceiling(this.widthNorthSouth);
            int ceilRadiuxZ = (int)Math.Ceiling(this.widthWestEast);

            double nextXn = 0;

            for (int x = 0; x <= ceilRadiusX; ++x)
            {
                double xn = nextXn;
                nextXn = (x + 1) * invRadiusX;
                double nextZn = 0;

                for (int z = 0; z <= ceilRadiuxZ; ++z)
                {
                    double zn = nextZn;
                    nextZn = (z + 1) * invRadiusZ;

                    double distanceSq = lengthSq(xn, zn);
                    if (distanceSq > 1)
                    {
                        if (z == 0)
                        {
                            return cylinderPoints;
                        }
                        break;
                    }

                    if (!this.filled)
                    {
                        if (lengthSq(nextXn, zn) <= 1 && lengthSq(xn, nextZn) <= 1)
                        {
                            continue;
                        }
                    }

                    for(int y = 0; y < this.height; ++y)
                    {
                        cylinderPoints.Add(center + new Vector3i(x, y, z));
                        cylinderPoints.Add(center + new Vector3i(-x, y, z));
                        cylinderPoints.Add(center + new Vector3i(x, y, -z));
                        cylinderPoints.Add(center + new Vector3i(-x, y, -z));
                    }
                }
            }

            return cylinderPoints;
        }

        private static double lengthSq(double x, double y, double z)
        {
            return (x * x) + (y * y) + (z * z);
        }

        private static double lengthSq(double x, double z)
        {
            return (x * x) + (z * z);
        }
    }
}
