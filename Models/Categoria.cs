using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestoV2.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength:50, ErrorMessage = "No puede ser mayor a (i) caracteres")]
        public string Nombre { get; set; }
        [Display(Name = "Tipo Operación ")]
        public TipoOperacion TipoOperacionId { get; set; }

        public int usuarioId { get; set; }
    }

}
