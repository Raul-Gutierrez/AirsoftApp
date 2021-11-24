using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AirsoftApp.Models
{
    public class AsistenciaViewModel
    {

            public int idPersona { get; set; }

            public int idJuego { get; set; }

            public int idIntegrante { get; set; }

            public string nomJuego { get; set; }

        [Required(ErrorMessage = "Debe ingresar una fecha valida")]
            [Display(Name = "Fecha del juego")]
            [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
            [DataType(DataType.Date)]
            public DateTime FechJuego { get; set; }

        [Display(Name = "Avatar")]
        public byte[] AvatarJuego { get; set; }

        public string CodJuego { get; set; }

    }
}