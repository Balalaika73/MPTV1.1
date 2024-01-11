using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPTV1._1.Models
{
    public class PCDisplayAbiturients
    {
        [DisplayName("Номер заявления")]
        public string StNumb { get; set; }
        [DisplayName("Фамилия")]
        public string AbSur { get; set; }
        [DisplayName("Имя")]
        public string AbName { get; set; }
        [DisplayName("Отчество")]
        public string AbSecN { get; set; }
        [DisplayName("Средний балл")]
        public string AbGPA { get; set; }
        [DisplayName("Кол-во достижений")]
        public string AbAchieves { get; set; }


    }
}
