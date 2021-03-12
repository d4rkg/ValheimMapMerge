using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimMapMerge.Unity
{
	internal class VersionInfo
	{
		public static int m_major = 0;

		public static int m_minor = 147;

		public static int m_patch = 3;

		public static int m_playerVersion = 33;

		public static int[] m_compatiblePlayerVersions = new int[]
		{
			32,
			31,
			30,
			29,
			28,
			27
		};

		public static int m_worldVersion = 26;

		public static int[] m_compatibleWorldVersions = new int[]
		{
			25,
			24,
			23,
			22,
			21,
			20,
			19,
			18,
			17,
			16,
			15,
			14,
			13,
			11,
			10,
			9
		};

		public static bool IsPlayerVersionCompatible(int version)
		{
			if (version == m_playerVersion)
			{
				return true;
			}
			int[] compatiblePlayerVersions = m_compatiblePlayerVersions;
			for (int i = 0; i < compatiblePlayerVersions.Length; i++)
			{
				int num = compatiblePlayerVersions[i];
				if (version == num)
				{
					return true;
				}
			}
			return false;
		}
	}
}
