using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using HarmonyLib;

namespace Harmony
{
	public static class PatchInfoSerialization
	{
		class Binder : SerializationBinder
		{
			public override Type BindToType(string assemblyName, string typeName)
			{
				var types = new Type[] {
					typeof(PatchInfo),
					typeof(Patch[]),
					typeof(Patch)
				};
				foreach (var type in types)
					if (typeName == type.FullName)
						return type;
				var typeToDeserialize = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
				return typeToDeserialize;
			}
		}

		public static byte[] Serialize(this PatchInfo patchInfo)
		{
#pragma warning disable XS0001
			using (var streamMemory = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(streamMemory, patchInfo);
				return streamMemory.GetBuffer();
			}
#pragma warning restore XS0001
		}

		public static PatchInfo Deserialize(byte[] bytes)
		{
			var formatter = new BinaryFormatter { Binder = new Binder() };
#pragma warning disable XS0001
			var streamMemory = new MemoryStream(bytes);
#pragma warning restore XS0001
			return (PatchInfo)formatter.Deserialize(streamMemory);
		}

		// general sorting by (in that order): before, after, priority and index
		public static int PriorityComparer(object obj, int index, int priority, string[] before, string[] after)
		{
			var trv = Traverse.Create(obj);
			var theirOwner = trv.Field("owner").GetValue<string>();
			var theirPriority = trv.Field("priority").GetValue<int>();
			var theirIndex = trv.Field("index").GetValue<int>();

			if (before != null && Array.IndexOf(before, theirOwner) > -1)
				return -1;
			if (after != null && Array.IndexOf(after, theirOwner) > -1)
				return 1;

			if (priority != theirPriority)
				return -(priority.CompareTo(theirPriority));

			return index.CompareTo(theirIndex);
		}
	}
}