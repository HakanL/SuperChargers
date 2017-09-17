using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArgs;

namespace SuperChargers
{
    public class Arguments
    {
        [ArgShortcut("f")]
        [ArgDescription("Path to audio file")]
        public string FileToPlay { get; set; }
    }
}
