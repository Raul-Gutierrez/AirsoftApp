using AirsoftApp.Models;
using AirsoftApp.Models.ModeloSql;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;

namespace AirsoftApp.Controllers
{
    public class HomeController : Controller
    {
        airSoftAppEntities db = null;
        public ActionResult Index()
        {
            string usuario = User.Identity.GetUserName();
            if (usuario != "")
            {
                db = new airSoftAppEntities();
                {
                    var model = (from a in db.TB_JUEGO
                                 select new HomeViewModels
                                 {
                                     IdJuego = a.IDJUEGO,
                                     DescJuego = a.DESCJUEGO,
                                     ImgJuego = a.IMGJUEGO
                                 });

                    int idPersona = IdPersona(usuario);
                

                    ViewBag.escuadrones = (from b in db.TB_ESCUADRON 
                                           join c in db.TB_INTEGRANTE on b.IDESCUADRON equals c.IDESCUADRON 
                                           join d in db.TB_PERSONA on c.IDPERSONA equals d.IDPERSONA 
                                           where b.ESTESCUADRON == true && c.IDPERSONA != idPersona && c.CAPINTEGRANTE == true
                                           select new EscuadronViewModel
                                           {
                                               IdEscuadron = b.IDESCUADRON,
                                               ImgEscuadron = b.IMGESCUADRON,
                                               NomEscuadron = b.NOMESCUADRON,
                                               CodEscuadron = b.CODESCUADRON
                                           }
                                       ).ToList();
                    return View(model);
                }
                
            }
            else
            {
                return Redirect("~/Home/Inicio");
            }  
        }

        public ActionResult Inicio()
        {
            return View();
        }

        public ActionResult VerEscuadron(string codEscuadron)
        {
            db = new airSoftAppEntities();
            {
               var model = db.TB_ESCUADRON.Where(a => a.CODESCUADRON == codEscuadron).FirstOrDefault();

                ViewBag.lista = (from a in db.TB_INTEGRANTE
                                 where a.TB_ESCUADRON.CODESCUADRON == codEscuadron
                                 select new PersonaViewModel{
                                     
                                     Nick = a.TB_PERSONA.NICKPERSONA
                }).ToList();

               
                return View(model);
            }
        }

        public ActionResult UnirEscuadron(int idEscuadron)
        {
            PersonaController persona = new PersonaController();
            int IdPersona = persona.ObtenerPersona(User.Identity.GetUserName()).IDPERSONA;

            db = new airSoftAppEntities();
            {

                var integrante = new TB_INTEGRANTE();

                var valida = (from a in db.TB_INTEGRANTE
                              where a.IDPERSONA == IdPersona && a.IDESCUADRON == idEscuadron
                              select new
                              {
                                  idIntegrante = a.IDINTEGRANTES
                              }).ToList();
                if (valida.Count() == 0)
                {
                    integrante.IDESCUADRON = idEscuadron;
                    integrante.IDPERSONA = IdPersona;
                    integrante.ESTINTEGRANTE = true;
                    integrante.CAPINTEGRANTE = false;

                    db.TB_INTEGRANTE.Add(integrante);
                    db.SaveChanges();
                    return Redirect("~/Home/index");

                }
                else
                {
                   
                    return Redirect("~/Home/index");
                }


            }

        }

        public ActionResult VerJuego(int idJuego)
        {  
            db = new airSoftAppEntities();
            {
                
                var model = db.TB_JUEGO.Where(a=> a.IDJUEGO == idJuego).FirstOrDefault();

                return View(model);
            };
           
        }


        public ActionResult ConvertirImagenHome(int IdJuego)
        {
            db = new airSoftAppEntities();
            {
                var imagen = (from a in db.TB_JUEGO
                              where a.IDJUEGO == IdJuego
                              select a.IMGJUEGO).FirstOrDefault();

                return File(imagen, "imagenes/jpg");
            }

        }

        public int IdPersona(string usuario)
        {
            
            db = new airSoftAppEntities();
            {
                var user = db.TB_PERSONA.Where(a => a.CORREOPER == usuario).FirstOrDefault().IDPERSONA;
                return user;
            };        
        }


    }
}