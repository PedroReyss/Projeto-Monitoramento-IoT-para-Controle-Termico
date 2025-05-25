using Microsoft.AspNetCore.Http;
using System;

namespace PBL.Models
{
    public class FuncionarioViewModel : PadraoViewModel
    {
        public string Nome { get; set; }
        public int Idade { get; set; }
        public string Cargo { get; set; }
        public IFormFile Foto { get; set; }
        public byte[] FotoEmByte { get; set; }
        public string FotoEmBase64
        {
            get
            {
                if (FotoEmByte != null)
                    return Convert.ToBase64String(FotoEmByte);
                else
                    return string.Empty;
            }
        }
    }
}
