using AirsoftApp.Models.ModeloSql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirsoftApp.Models
{
    public class JuegoViewModel
    {

        public int idJuego { get; set; }

        [Display(Name = "Codigo del juego")]
        [StringLength(10)]
        public string CodJuego { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Debe ingresar un Nombre")]
        [Display(Name = "Nombre juego")]
        [StringLength(10)]
        public string NomJuego { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Debe ingresar una descripcion del juego")]
        [Display(Name = "Descripcion del juego")]
        [StringLength(250)]
        public string DescJuego { get; set; }

        [Required(ErrorMessage = "Debe ingresar una fecha valida")]
        [Display(Name = "Fecha del juego")]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FechJuego { get; set; }

        //[System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Debe agregar una imagen")]
        [Display(Name = "Avatar")]
        public byte[] AvatarJuego { get; set; }

        //El estado cambia mientras esta en curso
        public bool EstJuego { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Debe elegir un escuadron")]
        [Display(Name = "Escuadron anfitrión")]
        public int IdEscuadronJuego { get; set; }
        public string DescEscuadronJuego { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Debe elegir un modo de juego")]
        [Display(Name = "Modo de juego")]
        public int IdModoJuego { get; set; }
        public string DescModoJuego { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Debe elegir un tipo de juego")]
        [Display(Name = "Tipo Juego")]
        public int IdTipoJuego { get; set; }
        public string DescTipoJuego { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Debe elegir un tipo de Partida")]
        [Display(Name = "Tipo Partida")]
        public int IdTipoPartida { get; set; }
        public string DescTipoPartida { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Debe elegir una ciudad")]
        [Display(Name = "Region")]
        public int IdRegion { get; set; }
        public string Region { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Debe elegir una comuna")]
        [Display(Name = "Comuna")]
        public int IdComuna { get; set; }
        public string Comuna { get; set; }

        
        [Display(Name = "Creador")]
        public int idPersonaJuego { get; set; }
        public string NombrePersonaJuego { get; set; }

    }
}