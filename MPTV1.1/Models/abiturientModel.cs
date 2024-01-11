using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPTV1._1.Models
{
    public class abiturientModel
    {
        public string StNumb { get; set; }
        public string abGmail { get; set; }
        public string FIO { get; set; }
        public byte[] abState { get; set; }
        public byte[] abSNILS { get; set; }
        public byte[] abPhoto { get; set; }
        public byte[] abAchieve { get; set; }
        public byte[] abCert { get; set; }
    }
}
