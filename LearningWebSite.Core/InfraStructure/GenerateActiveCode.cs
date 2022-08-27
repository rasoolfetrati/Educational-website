using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningWebSite.Core.InfraStructure
{
    public static class GenerateActiveCode
    {
        public static string GenerateCode()
        {
            Random generator = new Random();
            String r = generator.Next(1, 1000000).ToString("D6");
            return r;
        }
    }
}
