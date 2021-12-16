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

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Debe ingresar un Nick")]
        [Display(Name = "Nick")]
        [StringLength(10)]
        public string Nick { get; set; }

        //[Required]
        [Display(Name = "Nombre completo")]
        public string Nombre { get; set; }

        [Display(Name = "R.U.N:")]
        public long Run { get; set; }

        [Display(Name = "DV:")]
        public string Dv { get; set; }

        //[Required]
        [Display(Name = "Apellido paterno")]
        public string Apellido_Paterno { get; set; }

        //[Required]
        [Display(Name = "Apellido materno")]
        public string Apellido_Materno { get; set; }

        //[Required]
        [Display(Name = "Telefono 912345678")]
        [Phone]
        public string Telefono { get; set; }

        //[Required]

        [Display(Name = "Correo")]
        [EmailAddress]
        public string Correo { get; set; }

        [Display(Name = "Comuna")]
        public int IdComuna { get; set; }
        public string Comuna { get; set; }

        //[Required]
        [Display(Name = "Region")]
        public string Region { get; set; }
        public long IdRegion { get; set; }


        [Display(Name = "Experiencia:")]
        public int Experiencia { get; set; }

        [Display(Name = "Rango:")]
        public string Rango { get; set; }

        public int IdRango { get; set; }

        public List<TB_POSICION> ListPosPer { get; set; }

        public List<TB_JUEGO> ListJuegos { get; set; }

        public int IdUsuario { get; set; }

        [Display(Name = "Foto de perfil")]

        public byte[] PerfilPersona { get; set; }

        #region[cboEstado]
        //public List<SelectListItem> cboEstado()
        //{

        //    List<Modelnew.ViewModel.PersonaViewModel> estadolst = null;
        //    using (Modelnew.AirSoftAppEntities1 db = new Modelnew.AirSoftAppEntities1())
        //    {

        //        estadolst = (from e in db.ESTADO
        //                     select new PersonaViewModel
        //                     {
        //                         idEstado = e.IDESTTEAM,
        //                         Estado = e.DESCESTTEAM
        //                     }).ToList();
        //    }

        //    List<SelectListItem> items = estadolst.ConvertAll(e =>
        //    {

        //        return new SelectListItem()
        //        {
        //            Text = e.Estado.ToString(),
        //            Value = e.idEstado.ToString(),
        //            Selected = false
        //        };
        //    });

        //    return items;
        //}
        #endregion

        //Carga el listado de regiones


        #region[infoRegion]
        public string infoRegion(int idComuna)
        {
            using (airSoftAppEntities db = new airSoftAppEntities())
            {
                var descregion = (from a in db.TB_COMUNA
                                  join b in db.TB_REGION on a.IDREGION equals b.IDREGION
                                  where a.IDCOMUNA == idComuna
                                  select b.DESCREGION);

                return descregion.FirstOrDefault();
            }
        }
        #endregion

        #region[infoComuna]
        public string infoComuna(long idComuna)
        {
            string comuna;
            using (airSoftAppEntities db = new airSoftAppEntities())
            {
                var Comuna = db.TB_COMUNA.Find(idComuna);

                comuna = Comuna.DESCCOMUNA;
            }
            return (comuna);

        }
        #endregion



        public string infoRango(int experiencia)
        {
            airSoftAppEntities db = new airSoftAppEntities();
            {
                var valRango1 = (from a in db.TB_RANGO
                                 where a.VALORRANGO > experiencia
                                 select a.DESCRANGO
                                 ).FirstOrDefault();
                if (valRango1 == null)
                {
                    valRango1 = (from a in db.TB_RANGO
                                 orderby a.VALORRANGO descending
                                 select a.DESCRANGO
                                     ).FirstOrDefault();
                    return valRango1;
                }
                return valRango1;
            }
        }

    }
}