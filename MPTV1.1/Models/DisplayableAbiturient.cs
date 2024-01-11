using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.ComponentModel;

public partial class DisplayableAbiturient
{
    [DisplayName("Фамилия")]
    public string AbSur { get; set; }
    [DisplayName("Имя")]
    public string AbName { get; set; }
    [DisplayName("Отчество")]
    public string AbSecN { get; set; }
    [DisplayName("Средний балл")]
    public string AbGPA { get; set; }
}
