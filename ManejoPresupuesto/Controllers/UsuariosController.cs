using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers
{
    public class UsuariosController: Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;

        public UsuariosController(UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager) 
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [AllowAnonymous]
        public IActionResult Registro() 
        { 
        
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Registro(RegistroViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = new Usuario() { Email = model.Email};

            var resultado = await userManager.CreateAsync(usuario,password: model.Password);

            if (resultado.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Transacciones");
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    
                }

                return View(model);
            }
          

        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            return RedirectToAction("Index", "Transacciones");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {


            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
           if (!ModelState.IsValid)
            {
                return View(model);

            }

            var resultado = await signInManager.PasswordSignInAsync(model.Email,model.Password, model.Recuerdame, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Transacciones");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o password incorrecto");
                return View(model);
            }
        }
    }
}
