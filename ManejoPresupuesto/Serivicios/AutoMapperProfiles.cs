using AutoMapper;
using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Serivicios
{
    public class AutoMapperProfiles: Profile
    {

        public AutoMapperProfiles()
        {
            CreateMap<Cuenta, CuentaCreacionViewModel>();

            CreateMap<TransaccionActualizacionViewModel, Transaccion>().ReverseMap();
        
        }

    }
}
