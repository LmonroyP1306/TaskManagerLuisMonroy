using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    public class Tarea
    {
        public int TareaId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(100, ErrorMessage = "El título no puede exceder 100 caracteres")]
        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public DateTime FechaAsignacion { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria")]
        [Display(Name = "Fecha de Vencimiento")]
        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        public string Estado { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }
    }

    public enum EstadoTarea
    {
        [Display(Name = "Pendiente")]
        Pendiente,

        [Display(Name = "En Progreso")]
        EnProgreso,

        [Display(Name = "Completa")]
        Completa
    }
}
