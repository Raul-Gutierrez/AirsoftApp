using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AirsoftApp.Models
{
    public class HomeViewModels
    {
        [Display(Name = "Avatar")]
        public byte[] ImgJuego { get; set; }
        [Display(Name = "Descripcion")]
        public string DescJuego { get; set; }

        public int IdJuego { get; set; }


    }
}