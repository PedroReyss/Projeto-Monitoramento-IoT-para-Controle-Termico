using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PBL.Enum
{
    public enum TipoEnum
    {
        [Display(Name = "Selecione uma opção")]
        Selecione,
        Administrador,
        Comum
    }
}
