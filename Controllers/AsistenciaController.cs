using AirsoftApp.Models;
using AirsoftApp.Models.ModeloSql;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                             where b.IDPERSONA == idUsuario && b.ESTPARTJUEGO == 0
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

        public ActionResult AceptarJuego(int IdEscuadron, int IdJuego)
        {
            int IdPersona = ObtenerIdUsuario(User.Identity.GetUserName());

            db = new airSoftAppEntities();
            {
                TB_PARTICIPA_JUEGO Juego = new TB_PARTICIPA_JUEGO
                {
                    IDESCUADRON = IdEscuadron,
                    IDJUEGO = IdJuego,
                    IDPERSONA = IdPersona,
                    ESTPARTJUEGO = 1
                };

                db.TB_PARTICIPA_JUEGO.Add(Juego);
                db.SaveChanges();

                return Redirect("~/Asistencia/IndexAsistencia");
            }


        }

        public ActionResult VerJuegoAsistido(int IdJuego)
        {

            db = new airSoftAppEntities();
            {
                var Juego = db.TB_JUEGO.Find(IdJuego);

                TB_JUEGO Juegos = new TB_JUEGO
                {
                    IMGJUEGO = Juego.IMGJUEGO,
                    NOMJUEGO = Juego.NOMJUEGO,
                    DESCJUEGO = Juego.DESCJUEGO,
                    FECHJUEGO = Juego.FECHJUEGO
                };


                return View(Juegos);
            }
        }
        [HttpPost]
        public ActionResult IniciarJuego(string CodJuego, int IdJuego)
        {

            int IdPersona = ObtenerIdUsuario(User.Identity.GetUserName());

            if (VerificarCodigoEscuadron(CodJuego) == 1)
            {
                db = new airSoftAppEntities();
                {
                    var IdPartJuego = (from a in db.TB_PARTICIPA_JUEGO
                                 where a.IDJUEGO == IdJuego && a.IDPERSONA == IdPersona
                                 select a.IDPARTICIPANTE).FirstOrDefault();

                    TB_PARTICIPA_JUEGO ObtJuego = db.TB_PARTICIPA_JUEGO.Find(IdPartJuego);
                    TB_PERSONA ObtPer = db.TB_PERSONA.Find(IdPersona);

                    ObtJuego.ESTPARTJUEGO = 1;
                    ObtPer.EXPERIENCIAPER = 500;

                    db.Entry(ObtJuego).State = EntityState.Modified;
                    db.SaveChanges();
                    db.Entry(ObtPer).State = EntityState.Modified;
                    db.SaveChanges();

                }
                return Redirect("~/Asistencia/IndexAsistencia");
            }
            else 
            { 
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

        public int VerificarCodigoEscuadron(string CodJuego)
        {

            db = new airSoftAppEntities();
            {
                var Juego = db.TB_JUEGO.Where(a => a.CODJUEGO == CodJuego).FirstOrDefault();
                if (Juego.CODJUEGO == CodJuego)
                {
                    return 1;
                }
                else 
                { 
                    return 0; 
                }
            
            }

        }

    }
}
