using AutoMapper;
using ManejoPresupuestoV2.Models;

namespace ManejoPresupuestoV2.Servicios
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Cuenta, CuentaCreacionViewModel>();

            CreateMap<TransaccionActualizacionViewModel, Transaccion>().ReverseMap();

        }
    }
}
