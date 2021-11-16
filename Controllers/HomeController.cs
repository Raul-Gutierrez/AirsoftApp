using AirsoftApp.Models;
using AirsoftApp.Models.ModeloSql;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AirsoftApp.Controllers
{
    public class HomeController : Controller
    {
        airSoftAppEntities db = null;
        public ActionResult Index()
        {
            string Usuario = User.Identity.GetUserName();
            if (Usuario != "")
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

        //#region[Menu]
        //[HttpGet]
        //public JsonResult MenuHome()
        //{
        //    List<ElementJsonIntKey> lst;
        //    db = new airSoftAppEntities();
        //    {
        //        lst = (from a in db.TB_PAGINA
        //               select new ElementJsonIntKey
        //               {
        //                   Text = HtmlHelper.GenerateLink(a.DESCPAGINA.ToString())
        //               }).ToList();
        //    }
        //    return Json(lst, JsonRequestBehavior.AllowGet);
        //}
        //#endregion
        //#region[class elemntJsonIntKey]
        //public class ElementJsonIntKey
        //{
        //    public HtmlHelper Text { get; set; }
        //}
        //#endregion
    }
}