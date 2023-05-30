using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuestoV2.Models
{
    public class CuentaCreacionViewModel : Cuenta
    { 
        public IEnumerable<SelectListItem> TiposCuentas { get; set; }
    }
}
