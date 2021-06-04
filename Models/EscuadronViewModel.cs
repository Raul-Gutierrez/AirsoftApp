using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AirsoftApp.Models
{
    public class EscuadronViewModel
    {

        [Display(Name = "Nombre del Escuadron")]
        public string NomEscuadron { get; set; }
        [Display(Name = "Nombre del capitan")]
        public string CapEscuadron { get; set; }

        [Display(Name = "Su codigo de escuadron")]
        public string CodEscuadron { get; set; }

        [Display(Name = "Imagen Escuadron")]
        public byte[] ImgEscuadron { get; set; }

        [Display(Name = "¿Aceptar vacantes?")]
        public bool EstEscuadron { get; set; }



        #region[CBOESTADO]
        //public List<SelectListItem> cboEstado()
        //{
        //    List<SelectListItem> estadoList = new List<SelectListItem>();

        //    using (airSoftAppEntities db = new airSoftAppEntities())
        //    {
        //        estadoList = (from a in db.ESTADOESCUADRON
        //                      select new SelectListItem
        //                      {
        //                          Value = a.IDESTESC.ToString(),
        //                          Text = a.DESCESTESC
        //                      }).ToList();
        //    }
        //    return (estadoList);
        //}
        #endregion







    }
}