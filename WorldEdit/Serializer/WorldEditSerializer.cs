﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Eco.Mods.WorldEdit.Model;
using Eco.Shared.Math;
using K4os.Compression.LZ4.Streams;
using Newtonsoft.Json;

namespace Eco.Mods.WorldEdit.Serializer
{
	internal class WorldEditSerializer
	{
		public const float CurrentVersion = 1.3f;
		/* Version History:
		 * 1.0 - Old and unused format, supported blocks only.
		 * 1.1 - Support plants, objects, blocks at separate layers
		 * TODO: Write migration for this! 1.2 - Changed WorldEditPlantBlockData.PlantType form plant.GetType() to plant.Species.GetType()
		 *	added AuthorInformation
		 * 1.3 - Added Dimension information
		 * */

		private const string LZ4_HEADER = "LZ4";

		public string CurrentEcoVersion => Shared.EcoVersion.VersionNumber;
		private readonly List<WorldEditBlock> blockList = new List<WorldEditBlock>();
		private readonly List<WorldEditBlock> plantList = new List<WorldEditBlock>();
		private readonly List<WorldEditBlock> worldObjectList = new List<WorldEditBlock>();

		public List<WorldEditBlock> BlockList
		{
			get
			{
				return new List<WorldEditBlock>(this.blockList);
			}
			set
			{
				this.blockList.Clear();
				this.blockList.AddRange(value);
			}
		}

		public List<WorldEditBlock> PlantList
		{
			get
			{
				return new List<WorldEditBlock>(this.plantList);
			}
			set
			{
				this.plantList.Clear();
				this.plantList.AddRange(value);
			}
		}

		public List<WorldEditBlock> WorldObjectList
		{
			get
			{
				return new List<WorldEditBlock>(this.worldObjectList);
			}
			set
			{
				this.worldObjectList.Clear();
				this.worldObjectList.AddRange(value);
			}
		}

		public AuthorInformation AuthorInformation { get; set; }
		public Vector3i Dimension { get; private set; }

		public WorldEditSerializer()
		{

		}

		public static JsonSerializerSettings SerializerSettings
		{
			get
			{
				JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
				serializerSettings.Culture = System.Globalization.CultureInfo.InvariantCulture;
				serializerSettings.Converters.Add(new JsonQuaternionConverter());
				serializerSettings.Converters.Add(new JsonVector3iConverter());
				serializerSettings.Converters.Add(new JsonTypeConverter());
				return serializerSettings;
			}
		}

		public static WorldEditSerializer FromClipboard(WorldEditClipboard clipboard)
		{
			WorldEditSerializer serializer = new WorldEditSerializer();
			serializer.BlockList = clipboard.GetBlocks();
			serializer.PlantList = clipboard.GetPlants();
			serializer.WorldObjectList = clipboard.GetWorldObjects();
			serializer.Dimension = clipboard.Dimension;
			serializer.AuthorInformation = clipboard.AuthorInfo;
			return serializer;
		}

		public void Serialize(Stream stream)
		{
			EcoBlueprint schematic = EcoBlueprint.Create(this.blockList, this.plantList, this.worldObjectList, this.AuthorInformation, this.Dimension);
			Serialize(stream, schematic);
		}

		public void Serialize(Stream stream, object obj)
		{
			byte[] header = new byte[8];
			Array.Copy(Encoding.ASCII.GetBytes(LZ4_HEADER), header, System.Math.Min(8, LZ4_HEADER.Length));
			stream.Write(header, 0, 8);

			using (LZ4EncoderStream lZ4EncoderStream = LZ4Stream.Encode(stream, null, true))
			using (StreamWriter sw = new StreamWriter(lZ4EncoderStream, System.Text.Encoding.UTF8, 1024, true))
			using (JsonWriter writer = new JsonTextWriter(sw))
			{
				writer.Formatting = Formatting.None;
				JsonSerializer serializer = JsonSerializer.CreateDefault(SerializerSettings);
				serializer.Serialize(writer, obj);
			}
			stream.Seek(0, SeekOrigin.Begin);
		}

		public void Deserialize(Stream stream)
		{
			EcoBlueprint schematic = Deserialize<EcoBlueprint>(stream);

			if (!CurrentVersion.Equals(schematic.Version))
			{
				//TODO: Handle serialization version changes and support previous versions
				//throw new FileLoadException(message: $"EcoBlueprint file version missmatch [file version: {schematic.Version}, current version: {currentVersion}]");
			}

			if (!string.IsNullOrEmpty(schematic.EcoVersion) && !this.CurrentEcoVersion.Equals(schematic.EcoVersion))
			{
				//TODO: Handle ECO version changes and migrate from previous versions
				//throw new FileLoadException(message: $"EcoBlueprint file created in different Eco version [file version: {schematic.EcoVersion}, current version: {this.CurrentEcoVersion}]");
			}

			this.BlockList = schematic.Blocks;
			this.PlantList = schematic.Plants;
			this.WorldObjectList = schematic.Objects;
			this.AuthorInformation = schematic.Author;
		}

		public static T Deserialize<T>(Stream stream)
		{
			if (IsLZ4Stream(stream))
			{
				using (LZ4DecoderStream lZ4DecoderStream = LZ4Stream.Decode(stream, null, true))
				{
					return DeserializeJSON<T>(lZ4DecoderStream);
				}
			}
			else
			{
				return DeserializeJSON<T>(stream);
			}
		}

		private static T DeserializeJSON<T>(Stream stream)
		{
			using (StreamReader sr = new StreamReader(stream, System.Text.Encoding.UTF8, false, 1024, true))
			using (JsonReader reader = new JsonTextReader(sr))
			{
				JsonSerializer serializer = JsonSerializer.CreateDefault(SerializerSettings);
				return serializer.Deserialize<T>(reader);
			}
		}

		private static bool IsLZ4Stream(Stream stream)
		{
			byte[] buff = new byte[8];
			stream.Read(buff, 0, 8);
			string header = Encoding.ASCII.GetString(buff, 0, LZ4_HEADER.Length);
			if (LZ4_HEADER.Equals(header, StringComparison.Ordinal)) { return true; } else { stream.Seek(0, SeekOrigin.Begin); return false; }
		}
	}
}
