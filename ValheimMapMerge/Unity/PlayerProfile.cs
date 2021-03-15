using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ValheimMapMerge.Unity;

namespace ValheimMapMerge
{
	public class WorldPlayerData
	{
		public Vector3 m_spawnPoint = Vector3.zero;
		public bool m_haveCustomSpawnPoint;

		public Vector3 m_logoutPoint = Vector3.zero;
		public bool m_haveLogoutPoint;

		public Vector3 m_deathPoint = Vector3.zero;
		public bool m_haveDeathPoint;

		public Vector3 m_homePoint = Vector3.zero;
		public byte[] m_mapData;
	}

	public class PlayerStats
	{
		public int m_kills;
		public int m_deaths;
		public int m_crafts;
		public int m_builds;
	}

	public class PlayerProfile
    {
		public PlayerProfile(string path)
        {
			m_filepath = path;
        }

		public string m_filename => Path.GetFileName(m_filepath);		

		private int m_version = 0;
		private string m_filepath = "";
		private string m_playerName = "";
		private long m_playerID;
		private string m_startSeed = "";
		private byte[] m_playerData;

		public Dictionary<long, WorldPlayerData> m_worldData = new Dictionary<long, WorldPlayerData>();

		public PlayerStats m_playerStats = new PlayerStats();

		public long GetPlayerId()
        {
			return m_playerID;
        }

		public bool LoadPlayerFromDisk(out string error)
		{
			error = string.Empty;
			ZPackage zPackage = LoadPlayerDataFromDisk();
			//backup
			DirectoryInfo dir = Directory.CreateDirectory("backup");
			File.WriteAllBytes($"{dir.FullName}/{m_filename}", zPackage.GetArray());

			if (zPackage == null)
			{
				error = "No player data";
				return false;
			}

			m_version = zPackage.ReadInt();
			if (!VersionInfo.IsPlayerVersionCompatible(m_version))
			{
				error = "Player data is not compatible, ignoring";
				return false;
			}

			if (m_version >= 28)
			{
				m_playerStats.m_kills = zPackage.ReadInt();
				m_playerStats.m_deaths = zPackage.ReadInt();
				m_playerStats.m_crafts = zPackage.ReadInt();
				m_playerStats.m_builds = zPackage.ReadInt();
			}

			int num2 = zPackage.ReadInt();
			for (int i = 0; i < num2; i++)
			{
				long key = zPackage.ReadLong();
				WorldPlayerData worldPlayerData = new WorldPlayerData();

				worldPlayerData.m_haveCustomSpawnPoint = zPackage.ReadBool();
				worldPlayerData.m_spawnPoint = zPackage.ReadVector3();
				worldPlayerData.m_haveLogoutPoint = zPackage.ReadBool();
				worldPlayerData.m_logoutPoint = zPackage.ReadVector3();
				if (m_version >= 30)
				{
					worldPlayerData.m_haveDeathPoint = zPackage.ReadBool();
					worldPlayerData.m_deathPoint = zPackage.ReadVector3();
				}
				worldPlayerData.m_homePoint = zPackage.ReadVector3(); 
				if (m_version >= 29 && zPackage.ReadBool())
				{
					worldPlayerData.m_mapData = zPackage.ReadByteArray();
				}
				m_worldData.Add(key, worldPlayerData);
			}

			m_playerName = zPackage.ReadString();
			m_playerID = zPackage.ReadLong();
			m_startSeed = zPackage.ReadString();

			m_playerData = null;
			if (zPackage.ReadBool())
			{
				m_playerData = zPackage.ReadByteArray();
			}

			return true;
		}

		private ZPackage LoadPlayerDataFromDisk()
		{
			FileStream fileStream;
			try
			{
				fileStream = File.OpenRead(m_filepath);
			}
			catch
			{
				ZPackage result = null;
				return result;
			}
			byte[] data;
			try
			{
				BinaryReader expr_40 = new BinaryReader(fileStream);
				int count = expr_40.ReadInt32();
				data = expr_40.ReadBytes(count);
				int count2 = expr_40.ReadInt32();
				expr_40.ReadBytes(count2);
			}
			catch
			{
				fileStream.Dispose();
				ZPackage result = null;
				return result;
			}
			fileStream.Dispose();
			return new ZPackage(data);
		}

		public bool SaveToDisk(long worldId, byte[] mergedMap)
		{
			try
			{
				ZPackage zPackage = new ZPackage();
				zPackage.Write(m_version);
				zPackage.Write(m_playerStats.m_kills);
				zPackage.Write(m_playerStats.m_deaths);
				zPackage.Write(m_playerStats.m_crafts);
				zPackage.Write(m_playerStats.m_builds);
				zPackage.Write(m_worldData.Count);
				foreach (KeyValuePair<long, WorldPlayerData> current in m_worldData)
				{
					zPackage.Write(current.Key);
					zPackage.Write(current.Value.m_haveCustomSpawnPoint);
					zPackage.Write(current.Value.m_spawnPoint);
					zPackage.Write(current.Value.m_haveLogoutPoint);
					zPackage.Write(current.Value.m_logoutPoint);
					zPackage.Write(current.Value.m_haveDeathPoint);
					zPackage.Write(current.Value.m_deathPoint);
					zPackage.Write(current.Value.m_homePoint);
					zPackage.Write(current.Value.m_mapData != null);
					if (current.Value.m_mapData != null)
					{
						if (current.Key == worldId)
						{
							zPackage.Write(mergedMap);
						}
						else
						{
							zPackage.Write(current.Value.m_mapData);
						}
					}
				}
				zPackage.Write(m_playerName);
				zPackage.Write(m_playerID);
				zPackage.Write(m_startSeed);
				if (m_playerData != null)
				{
					zPackage.Write(true);
					zPackage.Write(m_playerData);
				}
				else
				{
					zPackage.Write(false);
				}
				byte[] array = zPackage.GenerateHash();
				byte[] array2 = zPackage.GetArray();
				Directory.CreateDirectory("merge");
				FileStream expr_221 = File.Create($"merge/{m_filename}");
				BinaryWriter expr_227 = new BinaryWriter(expr_221);
				expr_227.Write(array2.Length);
				expr_227.Write(array2);
				expr_227.Write(array.Length);
				expr_227.Write(array);
				expr_227.Flush();
				expr_221.Flush(true);
				expr_221.Close();
				expr_221.Dispose();

				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
	}
}
