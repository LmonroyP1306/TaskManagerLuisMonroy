using System;
using System.Data.Entity;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Data
{
    public class DbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            // Usuarios iniciales
            var usuarios = new[]
            {
            new Usuario
            {
                NombreUsuario = "admin",
                PasswordHash = "hashed_password_aqui", // Pon aquí el hash real o en claro para pruebas
                FechaRegistro = DateTime.Now
            },
            new Usuario
            {
                NombreUsuario = "usuario1",
                PasswordHash = "hashed_password_aqui",
                FechaRegistro = DateTime.Now
            }
        };

            context.Usuarios.AddRange(usuarios);
            context.SaveChanges();

            // Crear tareas iniciales asociadas al primer usuario (admin)
            var tareas = new[]
            {
            new Tarea
            {
                Titulo = "Tarea de ejemplo",
                Descripcion = "Esta es una tarea creada automáticamente al iniciar.",
                Estado = "Pendiente",
                FechaAsignacion = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddDays(7),
                UsuarioId = usuarios[0].UsuarioId
            }
        };

            context.Tareas.AddRange(tareas);
            context.SaveChanges();

            base.Seed(context);
        }
    }
}
