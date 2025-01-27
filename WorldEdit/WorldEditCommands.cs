﻿using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Mods.WorldEdit.Commands;
using Eco.Mods.WorldEdit.Utils;
using Eco.Shared.Localization;
using Eco.Shared.Math;
using Eco.Shared.Utils;
using Eco.Shared.Voxel;
using System;

namespace Eco.Mods.WorldEdit
{
	[ChatCommandHandler]
	public class WorldEditCommands
	{
		[ChatCommand("Lists of world edit commands", "we", ChatAuthorizationLevel.Admin)] public static void WorldEdit(User user) { }

		[ChatSubCommand("WorldEdit", "Gives the player a Wand for using world edit", "wand", ChatAuthorizationLevel.Admin)]
		public static void Wand(User user)
		{
			try
			{
				user.Inventory.AddItems(WorldEditManager.GetWandItemStack());
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "Removes the wand from the players inventory", "rmwand", ChatAuthorizationLevel.Admin)]
		public static void RmWand(User user)
		{
			try
			{
				user.Inventory.TryRemoveItems(WorldEditManager.GetWandItemStack());
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "Sets The Selected Area to the desired Block", "set", ChatAuthorizationLevel.Admin)]
		public static void Set(User user, string pTypeName)
		{
			try
			{
				pTypeName = pTypeName.Replace(" ", "");
				SetCommand command = new SetCommand(user, pTypeName);
				if (command.Invoke())
				{
					user.Player.MsgLoc($"{command.BlocksChanged} blocks changed in {command.ElapsedMilliseconds}ms.");
				}
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "Clears the Selected Area", "del", ChatAuthorizationLevel.Admin)]
		public static void Delete(User user)
		{
			try
			{
				SetCommand command = new SetCommand(user, typeof(World.Blocks.EmptyBlock));
				if (command.Invoke())
				{
					user.Player.MsgLoc($"{command.BlocksChanged} blocks cleared in {command.ElapsedMilliseconds}ms.");
				}
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "Drain water in Selected Area", "drain", ChatAuthorizationLevel.Admin)]
		public static void Drain(User user)
		{
			try
			{
				ReplaceCommand command = new ReplaceCommand(user, "Water", "Empty");
				if (command.Invoke())
				{
					user.Player.MsgLoc($"{command.BlocksChanged} water blocks drained in {command.ElapsedMilliseconds}ms.");
				}
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}
		[ChatSubCommand("WorldEdit", "Replace a Specific Block Type with Another Block Example: replace sand, dirt, this will replace sand with dirt", "replace", ChatAuthorizationLevel.Admin)]
		public static void Replace(User user, string pTypeNames)
		{
			try
			{
				string[] splitted = pTypeNames.Trim().Split(',');
				string toFind = splitted[0].ToLower().Replace(" ", "");

				string toReplace = string.Empty;

				if (splitted.Length >= 2)
					toReplace = splitted[1].Trim().ToLower().Replace(" ", "");

				ReplaceCommand command = new ReplaceCommand(user, toFind, toReplace);
				if (command.Invoke())
				{
					user.Player.MsgLoc($"{command.BlocksChanged} blocks changed.");
				}
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "Sets the Area on the outside of the selection to selected wall type", "walls", ChatAuthorizationLevel.Admin)]
		public static void Walls(User user, string typeName)
		{
			try
			{
				typeName = typeName.Replace(" ", "");
				WallsCommand command = new WallsCommand(user, typeName);
				if (command.Invoke())
				{
					user.Player.MsgLoc($"{command.BlocksChanged} blocks changed.");
				}
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
        }

        [ChatSubCommand("WorldEdit", "Creates an cylinder on first point for type, width north south, width west east, height", "cyl", ChatAuthorizationLevel.Admin)]
        public static void Cyl(User user, string typeName, int widthNorthSouth, int widthWestEast, int height = 1)
        {
            try
            {
                typeName = typeName.Replace(" ", "");
                CylCommand command = new CylCommand(user, typeName, widthNorthSouth, widthWestEast, height, true);
                if (command.Invoke())
                {
                    user.Player.MsgLoc($"{command.BlocksChanged} blocks changed.");
                }
            }
            catch (WorldEditCommandException e)
            {
                user.Player.ErrorLocStr(e.Message);
            }
            catch (Exception e)
            {
                Log.WriteError(Localizer.Do($"{e}"));
            }
        }

        [ChatSubCommand("WorldEdit", "Creates an hollow cylinder on first point for type, width north south, width west east, height", "hcyl", ChatAuthorizationLevel.Admin)]
        public static void HCyl(User user, string typeName, int widthNorthSouth, int widthWestEast, int height = 1)
        {
            try
            {
                typeName = typeName.Replace(" ", "");
                CylCommand command = new CylCommand(user, typeName, widthNorthSouth, widthWestEast, height, false);
                if (command.Invoke())
                {
                    user.Player.MsgLoc($"{command.BlocksChanged} blocks changed.");
                }
            }
            catch (WorldEditCommandException e)
            {
                user.Player.ErrorLocStr(e.Message);
            }
            catch (Exception e)
            {
                Log.WriteError(Localizer.Do($"{e}"));
            }
        }

        [ChatSubCommand("WorldEdit", "Duplicate the Selected Area based on repeating amounts to a direction", "stack", ChatAuthorizationLevel.Admin)]
		public static void Stack(User user, string directionAndAmount = "1", int offset = 0)
		{
			try
			{
				StackCommand command = new StackCommand(user, directionAndAmount, offset);
				if (command.Invoke())
				{
					user.Player.MsgLoc($"{command.BlocksChanged} blocks changed.");
				}
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "Move blocks in the Selected Area to a direction", "move", ChatAuthorizationLevel.Admin)]
		public static void Move(User user, string directionAndAmount = "1")
		{
			try
			{
				MoveCommand command = new MoveCommand(user, directionAndAmount);
				if (command.Invoke())
				{
					user.Player.MsgLoc($"{command.BlocksChanged} blocks moved.");
				}
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "expand the selected area by the specified amount in the looking direction", "expand", ChatAuthorizationLevel.Admin)]
		public static void Expand(User user, string directionAndAmount = "1")
		{
			try
			{
				SelectionCommand command = SelectionCommand.ExpandCommand(user, directionAndAmount);
				if (command.Invoke())
				{
					user.Player.MsgLoc($"Expanded selection {command.amount} {command.direction}");
				}
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "Reduce the selected area by the specified amount in the looking direction if not provided", "reduce", ChatAuthorizationLevel.Admin)]
		public static void Reduce(User user, string directionAndAmount = "1")
		{
			try
			{
				SelectionCommand command = SelectionCommand.ReduceCommand(user, directionAndAmount);
				if (command.Invoke())
				{
					user.Player.MsgLoc($"Contracted selection {command.amount} {command.direction}");
				}
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "shift will move the selected area by the specified amount in the looking direction", "shift", ChatAuthorizationLevel.Admin)]
		public static void Shift(User user, string directionAndAmount = "1")
		{
			try
			{
				SelectionCommand command = SelectionCommand.ShiftCommand(user, directionAndAmount);
				if (command.Invoke())
				{
					user.Player.MsgLoc($"Shifted selection {command.amount} {command.direction}");
				}
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "Upme will move the player upwards by the specified amount and place a block under the player if needed", "upme", ChatAuthorizationLevel.Admin)]
		public static void Upme(User user, int count = 1)
		{
			try
			{
				WorldEditCommand command = new UpmeCommand(user, count);
				if (command.Invoke())
				{
					//Silence?
				}
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "undo will revert the last action done using world edit, up to 10 times", "undo", ChatAuthorizationLevel.Admin)]
		public static void Undo(User user, int count = 1)
		{
			try
			{
				UserSession userSession = WorldEditManager.GetUserSession(user);
				if (userSession.ExecutingCommand != null && userSession.ExecutingCommand.IsRunning) throw new WorldEditCommandException("You can't use undo right now!"); //TODO: Probably need to rework that and impliment aborting

				if (count > userSession.ExecutedCommands.Count) count = userSession.ExecutedCommands.Count;
				if (count.Equals(0)) throw new WorldEditCommandException("Nothing to undo");

				for (int i = 1; i <= count; i++)
				{
					if (userSession.ExecutedCommands.TryPop(out WorldEditCommand command))
					{
						if (command.Undo())
						{
							if (!count.Equals(1)) { user.Player.MsgLoc($"Undo {i}/{count} done."); }
						}
					}
					else
					{
						throw new WorldEditCommandException("Nothing to undo");
					}
				}
				user.Player.MsgLoc($"Undo done.");
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteErrorLine(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "Redo will revert the last undo action, up to 10 times", "redo", ChatAuthorizationLevel.Admin)]
		public static void Redo(User user, int count = 1)
		{
			try
			{
				UserSession userSession = WorldEditManager.GetUserSession(user);
				if (userSession.ExecutingCommand != null && userSession.ExecutingCommand.IsRunning) throw new WorldEditCommandException("You can't use redo right now!"); //TODO: Probably need to rework that and impliment aborting

				if (count > userSession.UndoneCommands.Count) count = userSession.UndoneCommands.Count;
				if (count.Equals(0)) throw new WorldEditCommandException("Nothing to redo");

				for (int i = 1; i <= count; i++)
				{
					if (userSession.UndoneCommands.TryPop(out WorldEditCommand command))
					{
						if (command.Redo())
						{
							if (!count.Equals(1)) { user.Player.MsgLoc($"Redo {i}/{count} done."); }
						}
					}
					else
					{
						throw new WorldEditCommandException("Nothing to Redo");
					}
				}
				user.Player.MsgLoc($"Redo done.");
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteErrorLine(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "copy will copy the selected area ready for pasting or exporting", "copy", ChatAuthorizationLevel.Admin)]
		public static void Copy(User user)
		{
			try
			{
				WorldEditCommand command = new CopyCommand(user);
				if (command.Invoke())
				{
					user.Player.MsgLoc($"Copy done in {command.ElapsedMilliseconds}ms.");
				}
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "paste will paste the copied selection or imported schematic from where the player is standing", "paste", ChatAuthorizationLevel.Admin)]
		public static void Paste(User user, bool skipEmpty = false)
		{
			try
			{
				WorldEditCommand command = new PasteCommand(user, skipEmpty);
				if (command.Invoke())
				{
					user.Player.MsgLoc($"Paste done in {command.ElapsedMilliseconds}ms.");
				}
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "Copy and clean selected area", "cut", ChatAuthorizationLevel.Admin)]
		public static void Cut(User user)
		{
			try
			{
				UserSession userSession = WorldEditManager.GetUserSession(user);
				WorldRange region = userSession.Selection;

				WorldEditCommand command = new CopyCommand(user);
				if (command.Invoke(region))
				{
					user.Player.MsgLoc($"Copy done in {command.ElapsedMilliseconds}ms.");
					command = new SetCommand(user, typeof(World.Blocks.EmptyBlock));
					if (command.Invoke(region))
					{
						user.Player.MsgLoc($"{command.BlocksChanged} blocks cleared in {command.ElapsedMilliseconds}ms.");
					}
				}
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "rotate will rotate all blocks and items in your clipboard, usable by degrees IE: 90, 180, 270", "rotate", ChatAuthorizationLevel.Admin)]
		public static void Rotate(User user, int degrees = 90)
		{
			try
			{
				WorldEditCommand command = new RotateCommand(user, degrees);
				if (command.Invoke())
				{
					user.Player.MsgLoc($"Rotation in clipboard done.");
				}
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "export will turn your copied selection into a schematic that you can share with friends or import into a fresh world!", "export", ChatAuthorizationLevel.Admin)]
		public static void Export(User user, string fileName)
		{
			try
			{
				WorldEditCommand command = new ExportCommand(user, fileName);
				if (command.Invoke())
				{
					user.Player.MsgLoc($"Export done in {command.ElapsedMilliseconds}ms.");
				}
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "import will import a schematic that you or someone else has exported", "import", ChatAuthorizationLevel.Admin)]
		public static void Import(User user, string fileName = null)
		{
			try
			{
				WorldEditCommand command;

				if (!string.IsNullOrEmpty(fileName))
				{
					command = new ImportCommand(user, fileName);
					if (command.Invoke())
					{
						user.Player.MsgLoc($"Import done in {command.ElapsedMilliseconds}ms. Use /paste");
					}
					else
					{
						new PrintBlueprintListCommand(user).Invoke();
					}
				}
				else
				{
					new PrintBlueprintListCommand(user).Invoke();
				}
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e) { Log.WriteException(e); }
		}

		[ChatSubCommand("WorldEdit", "distr will give you a detailed list of all items in your selected area", "distr", ChatAuthorizationLevel.Admin)]
		public static void Distr(User user, string type = "brief", string fileName = null)
		{
			type = type.Trim().Replace(" ", "");
			if (type.Contains("brief")) type = "brief";
			if (type.Contains("detail")) type = "detail";
			switch (type)
			{
				case "brief":
				case "b":
					type = "brief";
					break;
				case "detail":
				case "d":
					type = "detail";
					break;
				default:
					type = "brief";
					break;
			}
			if (!string.IsNullOrEmpty(fileName))
			{
				fileName = fileName.Trim().Replace(" ", string.Empty);
			}

			try
			{
				WorldEditCommand command = new DistrCommand(user, type, fileName);
				if (command.Invoke())
				{
					//Output done in he command
				}
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "BInfo will give you a information about blueprint", "binfo", ChatAuthorizationLevel.Admin)]
		public static void BInfo(User user, string fileName, string outFileName = null)
		{
			if (!string.IsNullOrEmpty(outFileName))
			{
				outFileName = outFileName.Trim().Replace(" ", string.Empty);
			}

			try
			{
				if (!string.IsNullOrEmpty(fileName))
				{
					WorldEditCommand command = new BlueprintInfoCommand(user, fileName, outFileName);
					if (command.Invoke())
					{
						//Output done in he command
					}
				}
				else
				{
					throw new WorldEditCommandException("Blueprint file name not provided");
				}

			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "Set max growth for Plants and Trees in Selected Area", "grow", ChatAuthorizationLevel.Admin)]
		public static void Grow(User user)
		{
			try
			{
				WorldEditCommand command = new GrowCommand(user);
				if (command.Invoke())
				{
					user.Player.MsgLoc($"Grow done.");
				}

			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "Set First Position to given coordinate or player position", "setpos1", ChatAuthorizationLevel.Admin)]
		public static void SetPos1(User user, string coordinate = null)
		{
			try
			{
				Vector3i pos;

				if (!String.IsNullOrEmpty(coordinate))
				{
					if (!WorldEditUtils.ParseCoordinateArgs(user, coordinate, out pos)) { return; }
				}
				else
				{
					pos = user.Position.Round();
				}

				pos.X = pos.X < 0 ? pos.X + Shared.Voxel.World.VoxelSize.X : pos.X;
				pos.Z = pos.Z < 0 ? pos.Z + Shared.Voxel.World.VoxelSize.Z : pos.Z;
				pos.X %= Shared.Voxel.World.VoxelSize.X;
				pos.Z %= Shared.Voxel.World.VoxelSize.Z;

				UserSession session = WorldEditManager.GetUserSession(user);
				session.SetFirstPosition(pos);

				user.Player.MsgLoc($"First Position set to {pos}");
			}
			catch (Exception e)
			{
				Log.WriteErrorLineLocStr(e.ToString());

			}
		}

		[ChatSubCommand("WorldEdit", "Set Second Position to given coordinate or player position", "setpos2", ChatAuthorizationLevel.Admin)]
		public static void SetPos2(User user, string coordinate = null)
		{
			try
			{
				Vector3i pos;

				if (!String.IsNullOrEmpty(coordinate))
				{
					if (!WorldEditUtils.ParseCoordinateArgs(user, coordinate, out pos)) { return; }
				}
				else
				{
					pos = user.Position.Round();
				}

				pos.X = pos.X < 0 ? pos.X + Shared.Voxel.World.VoxelSize.X : pos.X;
				pos.Z = pos.Z < 0 ? pos.Z + Shared.Voxel.World.VoxelSize.Z : pos.Z;
				pos.X %= Shared.Voxel.World.VoxelSize.X;
				pos.Z %= Shared.Voxel.World.VoxelSize.Z;

				UserSession session = WorldEditManager.GetUserSession(user);
				session.SetSecondPosition(pos);

				user.Player.MsgLoc($"Second Position set to {pos}");
			}
			catch (Exception e)
			{
				Log.WriteErrorLineLocStr(e.ToString());
			}
		}

		[ChatSubCommand("WorldEdit", "Resets selection and both positions", "reset", ChatAuthorizationLevel.Admin)]
		public static void Reset(User user)
		{
			try
			{
				UserSession session = WorldEditManager.GetUserSession(user);
				session.ResetSelection();

				user.Player.MsgLoc($"WorldEdit: Positions reset");
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}
		#region Claim related commands
		[ChatSubCommand("WorldEdit", "Select current claim where player stands on ground level", "selclaim", ChatAuthorizationLevel.Admin)]
		public static void Selclaim(User user)
		{
			try
			{
				Vector3i pos = user.Position.Round();
				Vector2i claimPos = PlotUtil.RawPlotPos(pos.XZ).RawXY;
				UserSession session = WorldEditManager.GetUserSession(user);

				session.SetFirstPosition(claimPos.X_Z(pos.Y - 1));
				session.SetSecondPosition(WorldEditUtils.SecondPlotPos(claimPos).X_Z(pos.Y - 1));

				user.Player.MsgLoc($"First Position set to {session.Selection.min}");
				user.Player.MsgLoc($"Second Position set to {session.Selection.max}");
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "Add current claim where player stands to the selection", "addclaim", ChatAuthorizationLevel.Admin)]
		public static void Addclaim(User user)
		{
			try
			{
				Vector3i pos = user.Position.Round();
				Vector2i claimPos = PlotUtil.RawPlotPos(pos.XZ).RawXY;
				UserSession session = WorldEditManager.GetUserSession(user);

				WorldRange range = session.Selection;
				range.ExtendToInclude(claimPos.X_Z(pos.Y - 1));
				range.ExtendToInclude(WorldEditUtils.SecondPlotPos(claimPos).X_Z(pos.Y - 1));
				session.SetSelection(range);

				user.Player.MsgLoc($"First Position now at {session.Selection.min}");
				user.Player.MsgLoc($"Second Position now at {session.Selection.max}");
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}

		[ChatSubCommand("WorldEdit", "Expands selection to include amount of claims in given direction or where player looking", "expclaim", ChatAuthorizationLevel.Admin)]
		public static void Expclaim(User user, string args = "1")
		{
			try
			{
				UserSession session = WorldEditManager.GetUserSession(user);
				if (!session.Selection.IsSet()) throw new WorldEditCommandException("Please set both points first!");
				Direction direction = WorldEditUtils.ParseDirectionAndAmountArgs(user, args, out int amount);
				if (direction == Direction.Unknown ||
					direction == Direction.None ||
					direction == Direction.Up ||
					direction == Direction.Down) { throw new WorldEditCommandException("Unable to determine direction"); }
				WorldRange range = session.Selection;
				Vector3i pos = default;
				if (range.min.y <= range.max.y) pos.y = range.min.y; else pos.y = range.max.y;
				switch (direction)
				{
					case Direction.Left:
					case Direction.Back:
						if (range.min.x <= range.max.x) pos.x = range.min.x; else pos.x = range.max.x;
						if (range.min.z <= range.max.z) pos.z = range.min.z; else pos.z = range.max.z;
						break;
					case Direction.Right:
					case Direction.Forward:
						if (range.min.x <= range.max.x) pos.x = range.max.x; else pos.x = range.min.x;
						if (range.min.z <= range.max.z) pos.z = range.max.z; else pos.z = range.min.z;
						break;
				}
				pos += direction.ToVec() * (PlotUtil.PropertyPlotLength - 1) * amount;
				Vector2i claimPos = PlotUtil.RawPlotPos(pos.XZ).RawXY;
				range.ExtendToInclude(claimPos.X_Z(pos.Y));
				range.ExtendToInclude(WorldEditUtils.SecondPlotPos(claimPos).X_Z(pos.Y));
				session.SetSelection(range);

				user.Player.MsgLoc($"First Position now at {session.Selection.min}");
				user.Player.MsgLoc($"Second Position now at {session.Selection.max}");
			}
			catch (WorldEditCommandException e)
			{
				user.Player.ErrorLocStr(e.Message);
			}
			catch (Exception e)
			{
				Log.WriteError(Localizer.Do($"{e}"));
			}
		}
		#endregion Claim related commands
		[ChatSubCommand("WorldEdit", "Show World Edit version", "WEversion", ChatAuthorizationLevel.Admin)]
		public static void Version(User user)
		{
			user.Player.MsgLocStr($"World Edit Version:" + EcoWorldEdit.Version);
		}
	}
}