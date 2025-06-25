using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace TaskManager.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreUsuario { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime FechaRegistro { get; set; }

        public virtual ICollection<Tarea> Tareas { get; set; }
    }
}
