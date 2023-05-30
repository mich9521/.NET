using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestoV2.Models
{
    public class Transaccion
    {
        public int Id { get; set; }

        public int usuarioId { get; set; }
         
        [Display(Name ="Fecha Transaccion")]

        [DataType(DataType.DateTime)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd hh:MM tt"));

        public decimal Monto { get; set; }  
        
        [Range(1, maximum: int.MaxValue, ErrorMessage ="Debe seleccionar una categoría")]
        [Display(Name ="Categoría")]
        public int CategoriaId { get; set; }    

        [StringLength(maximumLength:1000, ErrorMessage = "la nota no puede pasar de {1} caracteres")]
        public string Nota { get; set; }

        [Range(1, maximum:int.MaxValue, ErrorMessage ="Debe seleccionar una cuenta")]
        public int CuentaId { get; set; }

        [Display(Name ="Tipo Operación")]
        public TipoOperacion TipoOperacionId { get; set; } = TipoOperacion.Ingreso;

        public string Cuenta { get; set; }

        public string Categoria { get; set; }

    }
}
