using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimMapMerge.Unity
{
	public struct Vector2i
	{
		public static Vector2i zero = new Vector2i(0, 0);
		public int x;
		public int y;

		public Vector2i(int _x, int _y)
		{
			this.x = _x;
			this.y = _y;
		}
	}
}