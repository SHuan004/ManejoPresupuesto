
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Serivicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;


namespace ManejoPresupuesto.Controllers
{
    public class TiposCuentasController: Controller
    {
        private readonly IrepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;

        public TiposCuentasController(IrepositorioTiposCuentas repositorioTiposCuentas,
            IServicioUsuarios servicioUsuarios)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var TiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);


            return View(TiposCuentas);  

        }

        public IActionResult Crear()
        {

                return View(); 
        
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta) 
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            tipoCuenta.UsuarioId = usuarioId;

            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(tipoCuenta.Nombre, usuarioId);

            if (yaExisteTipoCuenta)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), 
                    $"El nombe {tipoCuenta.Nombre} ya esta registrado");
                return View(tipoCuenta);

                
            }
            await repositorioTiposCuentas.Crear(tipoCuenta);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Editar (int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId( usuarioId,id);

            if (tipoCuenta == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);
        }

        [HttpPost]

        public async Task<ActionResult> Editar(TipoCuenta tipoCuenta)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(usuarioId, tipoCuenta.Id);

            if ( tipoCuentaExiste == null) 
            { 
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTiposCuentas.Actualizar(tipoCuenta);

            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Borrar (int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(usuarioId, id);

            if (tipoCuenta == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);


        }

        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(usuarioId, id);

            if (tipoCuenta == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTiposCuentas.Borrar(id);

            return RedirectToAction("Index");   
        }



        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre,int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(nombre, usuarioId,id);

            if (yaExisteTipoCuenta)
            {

                return Json($"El nombre {nombre} ya existe");
                
            }

            return Json(true);
        }


        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);

            var idsTiposCuentas = tiposCuentas.Select(x => x.Id);

            var idsTiposCuentasNoPertenecenAlUsuario = ids.Except(idsTiposCuentas).ToList();

            if (idsTiposCuentasNoPertenecenAlUsuario.Count > 0)
            {
                return Forbid();
            }

            var tiposCuentasOrdenados = ids.Select( (valor, indice) => new TipoCuenta() { Id = valor, Orden = indice + 1 }).AsEnumerable();

            await repositorioTiposCuentas.Ordenar(tiposCuentasOrdenados);
            

            return Ok();
        }
    
    }
}
