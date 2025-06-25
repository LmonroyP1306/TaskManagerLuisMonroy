using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [Authorize]
    public class TareasController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tareas
        public ActionResult Index(string estado, DateTime? fecha, DateTime? fechaFiltro, string orden = "asc")
        {
            string usuarioNombre = User.Identity.Name;

            var usuario = db.Usuarios.FirstOrDefault(u => u.NombreUsuario == usuarioNombre);
            if (usuario == null)
                return RedirectToAction("Login", "Account");

            var tareas = db.Tareas.Where(t => t.UsuarioId == usuario.UsuarioId);

            if (!string.IsNullOrEmpty(estado))
                tareas = tareas.Where(t => t.Estado == estado);

            if (fecha.HasValue)
            {
                var fechaSoloFecha = fecha.Value.Date;
                tareas = tareas.Where(t => DbFunctions.TruncateTime(t.FechaVencimiento) == fechaSoloFecha);
            }

            if (fechaFiltro.HasValue)
            {
                var fechaFiltroSoloFecha = fechaFiltro.Value.Date;
                tareas = tareas.Where(t => DbFunctions.TruncateTime(t.FechaVencimiento) == fechaFiltroSoloFecha);
            }

            tareas = orden == "desc"
                ? tareas.OrderByDescending(t => t.FechaVencimiento)
                : tareas.OrderBy(t => t.FechaVencimiento);

            return View(tareas.ToList());
        }



        [HttpGet]
        public ActionResult Create()
        {
            var model = new Tarea
            {
                FechaVencimiento = DateTime.Now.AddDays(1)
            };
            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tarea model)
        {
            if (ModelState.IsValid)
            {
                string nombreUsuario = User.Identity.Name;
                var usuario = db.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);

                if (usuario == null)
                    return Json(new { success = false, redirectUrl = Url.Action("Login", "Account") });

                if (model.FechaVencimiento.Date < DateTime.Today.AddDays(1))
                {
                    ModelState.AddModelError("FechaVencimiento", "La fecha de vencimiento debe ser al menos mañana.");
                    return PartialView("_Create", model);
                }

                model.UsuarioId = usuario.UsuarioId;
                model.FechaAsignacion = DateTime.Now;

                db.Tareas.Add(model);
                db.SaveChanges();

                return Json(new { success = true });
            }

            return PartialView("_Create", model);
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var tarea = db.Tareas.Find(id);
            if (tarea == null) return HttpNotFound();
            return PartialView("_Edit", tarea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tarea model)
        {
            if (ModelState.IsValid)
            {
                // Verificar que la tarea pertenezca al usuario actual
                string nombreUsuario = User.Identity.Name;
                var usuario = db.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);

                if (usuario == null)
                    return Json(new { success = false, redirectUrl = Url.Action("Login", "Account") });

                var tareaExistente = db.Tareas.FirstOrDefault(t => t.TareaId == model.TareaId && t.UsuarioId == usuario.UsuarioId);

                if (tareaExistente == null)
                    return Json(new { success = false, message = "No tienes permiso para editar esta tarea" });

                // Validación de fecha
                if (model.FechaVencimiento <= model.FechaAsignacion)
                {
                    ModelState.AddModelError("FechaVencimiento", "La fecha de vencimiento debe ser mayor a la fecha de asignación.");
                    return PartialView("_Edit", model);
                }

                // Actualizar solo los campos permitidos
                tareaExistente.Titulo = model.Titulo;
                tareaExistente.Descripcion = model.Descripcion;
                tareaExistente.FechaVencimiento = model.FechaVencimiento;
                tareaExistente.Estado = model.Estado;

                db.Entry(tareaExistente).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true });
            }

            return PartialView("_Edit", model);
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            var tarea = db.Tareas.Find(id);
            if (tarea == null) return HttpNotFound();
            return PartialView("_Delete", tarea);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Verificar que la tarea pertenezca al usuario actual
            string nombreUsuario = User.Identity.Name;
            var usuario = db.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);

            if (usuario == null)
                return Json(new { success = false, redirectUrl = Url.Action("Login", "Account") });

            var tarea = db.Tareas.FirstOrDefault(t => t.TareaId == id && t.UsuarioId == usuario.UsuarioId);

            if (tarea == null)
                return Json(new { success = false, message = "No tienes permiso para eliminar esta tarea o no existe" });

            db.Tareas.Remove(tarea);
            db.SaveChanges();

            return Json(new { success = true });
        }


        [HttpPost]
        public JsonResult UpdateStatus(int id, string newStatus)
        {
            try
            {
                var tarea = db.Tareas.Find(id);
                if (tarea == null)
                {
                    return Json(new { success = false, message = "Tarea no encontrada" });
                }

                tarea.Estado = newStatus;
                db.Entry(tarea).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
