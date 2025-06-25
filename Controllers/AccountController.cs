using System.Web.Mvc;
using TaskManager.Data;
using TaskManager.Models;
using System.Linq;
using System.Web.Security;

namespace TaskManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string nombreUsuario, string password)
        {
            var usuario = db.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario && u.PasswordHash == password);

            if (usuario != null)
            {
                FormsAuthentication.SetAuthCookie(nombreUsuario, false);
                return RedirectToAction("Index", "Tareas");
            }

            ViewBag.MensajeError = "Usuario o contraseña incorrectos.";
            return View();
        }


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}
