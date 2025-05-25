using System.Collections;

namespace PBL.Models
{
    public class UsuarioViewModel : PadraoViewModel
    {
        public int IdPessoa { get; set; }
        public string NomeFuncionario { get; set; }
        public int Tipo { get; set; }
        public string Username { get; set; }
        public string Senha { get; set; }
    }
}
