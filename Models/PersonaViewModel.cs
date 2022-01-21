using AirsoftApp.Models.ModeloSql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirsoftApp.Models
{
    public class PersonaViewModel
    {

        [Required(ErrorMessage = "Debe ingresar un {0}")]
        [Display(Name = "Nick*")]
        [MinLength(5, ErrorMessage ="{0} debe tener un minimo de 5 caracteres")]
        [MaxLength(10, ErrorMessage = "{0} debe tener un máximo de 10 caracteres")]
        public string Nick { get; set; }

        [Required(ErrorMessage = "Debe ingresar al menos un nombre")]
        [Display(Name = "Nombre completo*")]
        [MinLength(5, ErrorMessage = "{0} debe tener un minimo de 5 caracteres")]
        [MaxLength(20, ErrorMessage = "{0} debe tener un maximo de 20 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debe ingresar un {0}")]
        [MinLength(7, ErrorMessage = "{0} debe tener un minimo de 7 caracteres")]
        [MaxLength(8, ErrorMessage = "{0} debe tener un maximo de 8 caracteres")]
        [Display(Name = "R.U.N:*")]
        //[RegularExpression("/^([0-9])*$/", ErrorMessage ="Solo deben ingresarse digitos del 0 al 9")]
        public string Run { get; set; }

        [Required(ErrorMessage = "Debe ingresar un digito verificador")]
        [Display(Name = "DV:*")]
        [StringLength(1)]
        public string Dv { get; set; }

        [Required(ErrorMessage = "Debe ingresar un apellido")]
        [Display(Name = "Apellido paterno*")]
        public string Apellido_Paterno { get; set; }

        [Required(ErrorMessage = "Debe ingresar un apellido")]
        [Display(Name = "Apellido materno*")]
        public string Apellido_Materno { get; set; }

        [Required(ErrorMessage = "Debe ingresar un número")]
        [Display(Name = "Telefono*")]
        [StringLength(10)]
        [Phone]
        public string Telefono { get; set; }
        
        [Display(Name = "Correo*")]
        [Required(ErrorMessage = "Debe ingresar un correo")]
        [EmailAddress]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Debe sellecionar una comuna")]
        [Display(Name = "Comuna*")]
        public int IdComuna { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una región")]
        [Display(Name = "Región*")]
        public long IdRegion { get; set; }

        [Display(Name = "Experiencia:")]
        public int Experiencia { get; set; }

        [Display(Name = "Rango*:")]
        public string Rango { get; set; }

        public List<TB_POSICION> ListPosPer { get; set; }

        public byte[] PerfilPersona { get; set; }










    }
}