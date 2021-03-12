using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimMapMerge.Unity
{
	public struct ZDOID
	{
		public static ZDOID None = new ZDOID(0L, 0u);
		private long m_userID;
		private uint m_id;
		private int m_hash;

		public ZDOID(long userID, uint id)
		{
			this.m_userID = userID;
			this.m_id = id;
			this.m_hash = 0;
		}

		public long userID
		{
			get
			{
				return this.m_userID;
			}
		}

		public uint id
		{
			get
			{
				return this.m_id;
			}
		}
	}
}
