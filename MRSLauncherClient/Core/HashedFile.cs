using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRSLauncherClient
{
    public class HashedFile
    {
        public HashedFile(string path, string hash)
        {
            this.Path = path;
            this.Hash = hash;
        }

        public string Path { get; private set; }
        public string Hash { get; private set; }
    }
}
