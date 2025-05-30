using System;
using System.Collections;

namespace PBL.Models
{
    public class DispositivoViewModel : PadraoViewModel
    {
        public string Apelido { get; set; }
        public string DeviceId { get; set; }
        public string EntityName { get; set; }
        public int? ValorUltimaMedicao { get; set; }
        public DateTime DataUltimaMedicao { get; set; }
    }
}
