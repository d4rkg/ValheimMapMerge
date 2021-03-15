using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValheimMapMerge.Unity;

namespace ValheimMapMerge.Helpers
{
    public class MapMerge
    {        
        private long _worldId;
        private MapData _mapData;
        private List<PlayerProfile> _profiles;

        public MapMerge()
        {
            _worldId = 0;
            _mapData = new MapData();
            _profiles = new List<PlayerProfile>();
        }

        public long GetWorldId()
        {
            return _worldId;
        }

        public byte [] GetMergedWorld()
        {
            return _mapData.m_mergedWorld;
        }

        public List<PlayerProfile> GetProfiles()
        {
            return _profiles;
        }

        public void Reset()
        {
            _worldId = 0;
            _profiles = new List<PlayerProfile>();
            _mapData = new MapData();
        }

        public void LoadProfilesFromDisk(string[] files)
        {
            foreach (string file in files)
            {                
                PlayerProfile mpp = new PlayerProfile(file);
                if (!mpp.LoadPlayerFromDisk(out string error))
                {                    
                    return;
                }                
                _profiles.Add(mpp);
            }
        }

        public bool LocateSharedWorld()
        {
            Dictionary<long, int> _worlds = new Dictionary<long, int>();
            foreach (var pp in _profiles)
            {
                foreach (var w in pp.m_worldData)
                {
                    if (_worlds.ContainsKey(w.Key))
                        _worlds[w.Key]++;
                    else
                        _worlds.Add(w.Key, 1);
                }
            }
            _worldId = _worlds.SingleOrDefault(x => x.Value == _profiles.Count()).Key;
            return _worldId != 0;
        }

        public bool CreateMergedWorldWithPins(out string error)
        {
            try
            {
                error = string.Empty;
                foreach (var pp in _profiles)
                {
                    ZPackage zPackage = new ZPackage(pp.m_worldData[_worldId].m_mapData);
                    _mapData.m_mapVersion = zPackage.ReadInt();
                    int textureSize = zPackage.ReadInt();

                    if (_mapData.m_textureSize != textureSize)
                    {
                        return false;
                    }

                    for (int i = 0; i < (_mapData.m_textureSize * _mapData.m_textureSize); i++)
                    {
                        _mapData.m_mergedWorld[i] = (byte)(_mapData.m_mergedWorld[i] | Convert.ToByte(zPackage.ReadBool()));
                    }

                    if (_mapData.m_mapVersion >= 2)
                    {
                        int pinCount = zPackage.ReadInt();
                        for (int j = 0; j < pinCount; j++)
                        {
                            string name = zPackage.ReadString();
                            Vector3 pos = zPackage.ReadVector3();
                            PinType type = (PinType)zPackage.ReadInt();
                            bool isChecked = _mapData.m_mapVersion >= 3 && zPackage.ReadBool();

                            _mapData.AddPin(name, pos, type, isChecked);
                            _mapData.AddProfilePin(pp.GetPlayerId(), name, pos, type, isChecked);
                        }
                    }
                    if (_mapData.m_mapVersion >= 4)
                    {
                        _mapData.m_isReferencePositionPublic = zPackage.ReadBool();
                    }                    
                }
                return true;
            }
            catch(Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public byte[] GetMergedMapData(long profileId, bool bMergePins)
        {
            ZPackage zPackage = new ZPackage();
            zPackage.Write(_mapData.m_mapVersion);
            zPackage.Write(_mapData.m_textureSize);

            for (int i = 0; i < (_mapData.m_textureSize * _mapData.m_textureSize); i++)
            {
                zPackage.Write(_mapData.m_mergedWorld[i]);
            }           

            List<Pin> pins = _mapData.GetPins(profileId, bMergePins);
            zPackage.Write(pins.Count);
            foreach (Pin p in pins)
            {                
                zPackage.Write(p.Name);
                zPackage.Write(p.Pos);
                zPackage.Write((int)p.Type);
                zPackage.Write(p.IsChecked);                
            }
            zPackage.Write(_mapData.m_isReferencePositionPublic);
            return zPackage.GetArray();
        }
    }
}
