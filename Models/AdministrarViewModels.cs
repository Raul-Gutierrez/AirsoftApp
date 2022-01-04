using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AirsoftApp.Models
{
    public class AdministrarViewModels
    {
        
        [Required(ErrorMessage = "Debe ingresar un RUN valido")]
        [Display(Name = "RUN")]
        [StringLength(7)]
        public string Run { get; set; }

        [Required(ErrorMessage = "Debe seleecionar una opción")]
        [Display(Name = "Asignar rol")]
        public string IdRol { get; set; }





    }
}