using AirsoftApp.Models;
using AirsoftApp.Models.ModeloSql;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirsoftApp.Controllers
{
    public class AsistenciaController : Controller
    { 
        airSoftAppEntities db = null;
        // GET: Asistencia
        public ActionResult IndexAsistencia()
        {
            string usuario = User.Identity.GetUserName();
            int idUsuario = ObtenerIdUsuario(usuario);

            db = new airSoftAppEntities();
            {

                var model = (from a in db.TB_JUEGO join b in db.TB_PARTICIPA_JUEGO on a.IDJUEGO equals b.IDJUEGO
                             where b.IDPERSONA == idUsuario
                             select new AsistenciaViewModel
                                 {
                                     CodJuego = a.CODJUEGO,
                                     idJuego = a.IDJUEGO,
                                     nomJuego = a.NOMJUEGO,
                                     FechJuego = (DateTime)a.FECHJUEGO,
                                     AvatarJuego = a.IMGJUEGO
                                 }).ToList();
                return View(model);
            };
      
        }

        public ActionResult AceptarJuego(int IdEscuadron,int IdJuego )
        {

            int IdPersona = ObtenerIdUsuario(User.Identity.GetUserName());

            db = new airSoftAppEntities();
            {
                TB_PARTICIPA_JUEGO Juego = new TB_PARTICIPA_JUEGO
                {
                    IDESCUADRON = IdEscuadron,
                    IDJUEGO = IdJuego,
                    IDPERSONA = IdPersona
            };

            db.TB_PARTICIPA_JUEGO.Add(Juego);
            db.SaveChanges();

                return Redirect("~/Asistencia/IndexAsistencia");
            }

            
        }

        public int ObtenerIdUsuario(string Correo)
        {
            db = new airSoftAppEntities();
            {
                var idUsuario = db.TB_PERSONA.Where(a => a.CORREOPER == Correo).FirstOrDefault().IDPERSONA;
                return (idUsuario);
            };       
        }
    }
}
