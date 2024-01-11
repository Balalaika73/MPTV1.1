using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPTV1._1.Models
{
    internal class AbDisplayAbiturient
    {
        [DisplayName("Фамилия")]
        public string AbSur { get; set; }
        [DisplayName("Имя")]
        public string AbName { get; set; }
        [DisplayName("Отчество")]
        public string AbSecN { get; set; }
        [DisplayName("Средний балл")]
        public byte[] AbGPA { get; set; }
        [DisplayName("Кол-во достижений")]
        public byte[] AbAchieves { get; set; }

        public byte[] Passport { get; set; }

        public byte[] SBILS { get; set; }

        public byte[] SNILS { get; set; }

        public byte[] Notice { get; set; }
    }
}
