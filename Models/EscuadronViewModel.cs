using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AirsoftApp.Models
{
    public class EscuadronViewModel
    {
        public int IdEscuadron { get; set; }



        [Required(ErrorMessage = "Debe ingresar un nombre")]
        [MinLength(3, ErrorMessage = "{0} debe tener un minimo de 3 caracteres")]
        [MaxLength(10, ErrorMessage = "{0} debe tener un maximo de 10 caracteres")]
        [Display(Name = "Nombre del Escuadron*")]
        public string NomEscuadron { get; set; }

        [Display(Name = "Su codigo de escuadron: ")]
        public string CodEscuadron { get; set; }

        [Display(Name = "Imagen de escuadron*")]
        public byte[] ImgEscuadron { get; set; }

        [Required(ErrorMessage = "Debe escoger una opción")]

        [Display(Name = "¿Aceptar vacantes?*")]
        public bool EstEscuadron { get; set; }



    }
}
