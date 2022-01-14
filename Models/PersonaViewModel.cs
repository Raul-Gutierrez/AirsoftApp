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

        [Required(ErrorMessage = "Debe ingresar un Nick, máximo 10 caracteres")]
        [Display(Name = "Nick")]
        [StringLength(10)]
        public string Nick { get; set; }

        [Required(ErrorMessage = "Debe ingresar al menos un nombre")]
        [Display(Name = "Nombre completo")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debe ingresar un RUN")]
        [Display(Name = "R.U.N:")]
        public string Run { get; set; }

        [Required(ErrorMessage = "Debe ingresar un digito verificador")]
        [Display(Name = "DV:")]
        [StringLength(1)]
        public string Dv { get; set; }


        [Required(ErrorMessage = "Debe ingresar un apellido")]
        [Display(Name = "Apellido paterno")]
        public string Apellido_Paterno { get; set; }

        [Required(ErrorMessage = "Debe ingresar un apellido")]
        [Display(Name = "Apellido materno")]
        public string Apellido_Materno { get; set; }

        [Required(ErrorMessage = "Debe ingresar un número")]
        [Display(Name = "Telefono 912345678")]
        [StringLength(10)]
        [Phone]
        public string Telefono { get; set; }

        //[Required]
        
        [Display(Name = "Correo")]
        [Required(ErrorMessage = "Debe ingresar un correo")]
        [EmailAddress]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Debe sellecionar una comuna")]
        [Display(Name = "Comuna")]
        public int IdComuna { get; set; }

        //public string Comuna { get; set; }



        [Required(ErrorMessage = "Debe seleccionar una región")]
        [Display(Name = "Region")]
        public long IdRegion { get; set; }

        //public string Region { get; set; }
       


        [Display(Name = "Experiencia:")]
        public int Experiencia { get; set; }

        [Display(Name = "Rango:")]
        public string Rango { get; set; }

        //public int IdRango { get; set; }

        public List<TB_POSICION> ListPosPer { get; set; }

        //public List<TB_JUEGO> ListJuegos { get; set; }

        //public int IdUsuario { get; set; }

        [Display(Name = "Foto de perfil")]
        //[Required(ErrorMessage = "Debe ingresar una foto de perfil")]
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

    }
}