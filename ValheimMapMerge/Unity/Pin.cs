using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimMapMerge.Unity
{
    public class Pin
    {
        public string Name { get; set; }
        public Vector3 Pos { get; set; }
        public PinType Type { get; set; }
        public bool IsChecked { get; set; }
    }
}
