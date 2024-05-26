using System.Security.Claims;

namespace ManejoPresupuesto.Serivicios
{

    public interface IServicioUsuarios
    {
        int ObtenerUsuarioId();
    }
    public class ServiciosUsuarios: IServicioUsuarios
    {
        private readonly HttpContext httpContext;
        public ServiciosUsuarios(IHttpContextAccessor httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext;
        }
        public int ObtenerUsuarioId()
        {

            if (httpContext.User.Identity.IsAuthenticated)
            {
                var idClaim = httpContext.User.Claims.Where(X => X.Type == ClaimTypes.NameIdentifier).FirstOrDefault();

                var id = int.Parse(idClaim.Value) ;
                return id ;
            }
            else
            {
                throw new ApplicationException("El Usuario no esta autenticado");
            }
            
        
        }
    }
}
