using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimMapMerge.Unity
{
    public class MapData
    {
        public int m_mapVersion;
        public bool m_isReferencePositionPublic;
        public byte[] m_mergedWorld;
        public int m_textureSize = 2048;
        private List<Pin> m_mergedPins;
        private Dictionary<long, List<Pin>> m_profilePins;

        public MapData()
        {
            m_mergedPins = new List<Pin>();
            m_profilePins = new Dictionary<long, List<Pin>>();
            m_mergedWorld = new byte[m_textureSize * m_textureSize];
        }

        public List<Pin> GetPins(long profileId, bool bMergePins)
        {
            return bMergePins ? GetMergedPins() : m_profilePins[profileId];
        }

        public void AddPin(string name, Vector3 pos, PinType type, bool isChecked)
        {
            if (!m_mergedPins.Exists(x => x.Name == name))
            {
                m_mergedPins.Add(new Pin
                {
                    Name = name,
                    Pos = pos,
                    Type = type,
                    IsChecked = isChecked
                });
            }
        }

        public void AddProfilePin(long m_playerId, string name, Vector3 pos, PinType type, bool isChecked)
        {
            if (m_profilePins.ContainsKey(m_playerId))
            {
                m_profilePins[m_playerId].Add(new Pin
                {
                    Name = name,
                    Pos = pos,
                    Type = type,
                    IsChecked = isChecked
                });
            }
            else
            {
                m_profilePins.Add(m_playerId, new List<Pin>
                {
                    new Pin
                    {
                        Name = name,
                        Pos = pos,
                        Type = type,
                        IsChecked = isChecked
                    }
                }); 
            }
        }

        public List<Pin> GetMergedPins()
        {
            return m_mergedPins;
        }
    }
}
