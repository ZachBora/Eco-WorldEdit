﻿using System;
using System.Collections.Generic;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using Eco.Gameplay.Objects;
using Eco.Mods.WorldEdit.Serializer;
using Eco.Shared.Math;
using Newtonsoft.Json;

namespace Eco.Mods.WorldEdit.Model
{
	internal struct WorldEditWorldObjectBlockData : IWorldEditBlockData
	{
		public Type WorldObjectType { get; private set; }
		[JsonConverter(typeof(JsonQuaternionConverter))] public Quaternion Rotation { get; private set; }
		public Dictionary<Type, Object> Components { get; private set; }

		public static WorldEditWorldObjectBlockData From(WorldObject worldObject)
		{
			WorldEditWorldObjectBlockData worldObjectData = new WorldEditWorldObjectBlockData();

			worldObjectData.WorldObjectType = worldObject.GetType();
			worldObjectData.Rotation = worldObject.Rotation;

			worldObjectData.Components = new Dictionary<Type, Object>();
			if (worldObject.HasComponent<StorageComponent>())
			{
				StorageComponent storageComponent = worldObject.GetComponent<StorageComponent>();
				List<InventoryStack> inventoryStacks = new List<InventoryStack>();

				foreach (ItemStack stack in storageComponent.Inventory.Stacks)
				{
					if (stack.Empty()) continue;
					inventoryStacks.Add(InventoryStack.Create(stack));
				}

				worldObjectData.Components.Add(typeof(StorageComponent), inventoryStacks);
			}

			if (worldObject.HasComponent<CustomTextComponent>())
			{
				CustomTextComponent textComponent = worldObject.GetComponent<CustomTextComponent>();
				worldObjectData.Components.Add(typeof(CustomTextComponent), textComponent.TextData.Text);
			}

			return worldObjectData;
		}

		[JsonConstructor]
		public WorldEditWorldObjectBlockData(Type worldObjectType, Quaternion rotation, Dictionary<Type, Object> components)
		{
			this.WorldObjectType = worldObjectType ?? throw new ArgumentNullException(nameof(worldObjectType));
			this.Rotation = rotation;
			this.Components = components;
		}

		public void SetRotation(Quaternion rot)
		{
			this.Rotation = rot;
		}

		public IWorldEditBlockData Clone()
		{
			return new WorldEditWorldObjectBlockData(this.WorldObjectType, this.Rotation, new Dictionary<Type, Object>(this.Components));
		}
	}
}