using AirsoftApp.Models;
using AirsoftApp.Models.ModeloSql;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace AirsoftApp.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        airSoftAppEntities db = null;
        public ActionResult Index()
        {

            string usuario = User.Identity.GetUserName();
            int idPersona = IdPersona(usuario);
            if (usuario != "" && idPersona != 0)
            {
                db = new airSoftAppEntities();
                {

                    var model = (from a in db.TB_JUEGO
                                 where a.TB_PERSONA.CORREOPER != usuario && a.ESTJUEGO != false
                                 select new HomeViewModels
                                 {
                                     IdJuego = a.IDJUEGO,
                                     DescJuego = a.DESCJUEGO,
                                     ImgJuego = a.IMGJUEGO
                                 });

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
                using (ApplicationDbContext db = new ApplicationDbContext())
                {


                    var idUsarioActual = User.Identity.GetUserId();
                    var userManager = new UserManager<ApplicationUser>
                        (new UserStore<ApplicationUser>(db));

                    //var roleManager = new RoleManager<IdentityRole>
                    //    (new RoleStore<IdentityRole>(db));
                    ////Crear Rol
                    //var resultado = roleManager.Create(new IdentityRole("UNREGISTERED"));

                    //Agregar usuario a rol

                    if (userManager.IsInRole(idUsarioActual, "UNREGISTERED") == false)
                    {
                        var resultado = userManager.AddToRole(idUsarioActual, "UNREGISTERED");
                    }
                };

                return Redirect("~/Home/Inicio");
            }
        }
        [AllowAnonymous]
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
                                 where a.TB_ESCUADRON.CODESCUADRON == codEscuadron && a.ESTINTEGRANTE == true
                                 select new PersonaViewModel {

                                     Nick = a.TB_PERSONA.NICKPERSONA
                                 }).ToList();

                return View(model);
            }
        }


        public ActionResult SolicitudEscuadron(int idEscuadron)
        {
            PersonaController persona = new PersonaController();
            int IdPersona = persona.ObtenerPersona(User.Identity.GetUserName()).IDPERSONA;

            db = new airSoftAppEntities();
            {             
                var valida = db.TB_INTEGRANTE.Where(a => a.IDPERSONA == IdPersona && a.IDESCUADRON == idEscuadron).FirstOrDefault();

                if (valida != null)
                {
                    return Redirect("~/Home/index");
                }

                var creador = db.TB_INTEGRANTE.Find(valida.IDCREADOR).IDCREADOR;

                TB_INTEGRANTE integrante = new TB_INTEGRANTE();
                
                integrante.IDESCUADRON = idEscuadron;
                integrante.IDPERSONA = IdPersona;
                integrante.ESTINTEGRANTE = false;
                integrante.CAPINTEGRANTE = false;
                integrante.IDCREADOR = creador;

                db.TB_INTEGRANTE.Add(integrante);
                db.SaveChanges();

                return Redirect("~/Home/index");
            }
        }

        public ActionResult UnirEscuadron(int idInt)
        {
            PersonaController persona = new PersonaController();
            int IdPersona = persona.ObtenerPersona(User.Identity.GetUserName()).IDPERSONA;

            db = new airSoftAppEntities();
            {
                var Objeto = db.TB_INTEGRANTE.Find(idInt);

                if (Objeto == null)
                {
                    return Redirect("~/Home/index");
                }

                Objeto.ESTINTEGRANTE = true;

                db.Entry(Objeto).State = EntityState.Modified;
                db.SaveChanges();
                return Redirect("~/Home/index");
            }
        }

        [HttpGet]
        public ActionResult RetiraEscuadron(string escuadron)
        {

            db = new airSoftAppEntities();
            {
                int idEscuadron = db.TB_ESCUADRON.Where(a => a.NOMESCUADRON == escuadron).FirstOrDefault().IDESCUADRON;

                PersonaController persona = new PersonaController();
                int IdPersona = persona.ObtenerPersona(User.Identity.GetUserName()).IDPERSONA;

                int idIntegrante = db.TB_INTEGRANTE.Where(b => b.IDESCUADRON == idEscuadron && b.IDPERSONA == IdPersona).FirstOrDefault().IDINTEGRANTES;

                db.TB_INTEGRANTE.Remove(db.TB_INTEGRANTE.Where(a=> a.IDINTEGRANTES == idIntegrante).FirstOrDefault());
                db.SaveChanges();
            }
            return Redirect("~/Home/Index");
        }

        public ActionResult VerJuego(int idJuego)
        {  
            db = new airSoftAppEntities();
            {
                var model = db.TB_JUEGO.Where(a => a.IDJUEGO == idJuego).FirstOrDefault();

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
                var user = (from a in db.TB_PERSONA
                            where a.CORREOPER == usuario
                            select a.IDPERSONA ).FirstOrDefault();
                if (user == 0)
                {
                    return 0;
                }
                else
                {
                    return user;
                } 
            }      
        }
    }
}