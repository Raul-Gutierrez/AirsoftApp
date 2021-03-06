using AirsoftApp.Models;
using AirsoftApp.Models.ModeloSql;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirsoftApp.Controllers
{

    [Authorize]
    public class JuegoController : Controller
    {
        airSoftAppEntities db = null;
        // GET: Juego
        public ActionResult IndexJuego()
        {
            if (ValidaCantidadEscuadrones() == true)
            {
                TempData["Mensaje"] = "Debe Crear primero un escudron";
                return Redirect("~/Escuadron/NuevoEscuadron");
            }
            int IdPersona = ObtenerPersona(User.Identity.GetUserName()).IDPERSONA;

            db = new airSoftAppEntities();
            {

                var model = (from a in db.TB_JUEGO
                             join b in db.TB_ESCUADRON on a.IDESCUADRON equals b.IDESCUADRON
                             join c in db.TB_MODO_JUEGO on a.IDMODOJUEGO equals c.IDMODOJUEGO
                             join d in db.TB_TIPO_JUEGO on a.IDTIPOJUEGO equals d.IDTIPOJUEGO
                             join e in db.TB_TIPO_PARTIDA on a.IDTIPOPARTIDA equals e.IDTIPOPARTIDA
                             join f in db.TB_COMUNA on a.IDCOMUNA equals f.IDCOMUNA
                             join g in db.TB_PERSONA on a.IDPERSONA equals g.IDPERSONA
                             where a.IDPERSONA == IdPersona
                             select new JuegoViewModel
                             {
                                 IdJuego = a.IDJUEGO,
                                 CodJuego = a.CODJUEGO,
                                 AvatarJuego = a.IMGJUEGO,
                                 DescEscuadronJuego = b.NOMESCUADRON,
                                 DescJuego = a.DESCJUEGO
                             }).ToList();

                return View(model);
            }
        }


        // GET: Juego/Create
        public ActionResult NuevoJuego()
        {
            JuegoViewModel Model = new JuegoViewModel();
            PersonaController p = new PersonaController();
            int IdPersona = ObtenerPersona(User.Identity.GetUserName()).IDPERSONA;

            Model.CodJuego = Token();
            Model.NomJuego = "";
            Model.DescJuego = "";
            Model.FechJuego = DateTime.Now;
            Model.AvatarJuego = null;
            Model.EstJuego = false;
            Model.IdPersonaJuego = IdPersona;
            //Model.DescModoJuego = "";
            ViewData["idRegion"] = p.CboRegion();
            ViewData["IdModoJuego"] = ModoJuegoList();
            ViewData["IdTipoPartida"] = TipoPartidaList();
            ViewData["IdTipoJuego"] = TipoJuegoList();
            ViewData["IdEscuadronJuego"] = EscuadronesList();
            ViewData["IdComuna"] = p.CboComuna();


            return View(Model);
        }

        // POST: Juego/Create
        [HttpPost]
        public ActionResult NuevoJuego(JuegoViewModel model)
        {
            HttpPostedFileBase avatar = Request.Files[0];



            if (avatar.ContentLength == 0)
            {
                ModelState.AddModelError("AvatarJuego", "Debe establecer una imagen de perfil");
            }
            else
            {
                if (avatar.FileName.EndsWith(".jpg"))
                {
                    model.AvatarJuego = ObtenerByteJuego(avatar);
                }
                else
                {
                    ModelState.AddModelError("ImgEscuadron", "Solo se aceptan imagenes formato .JPG");
                }
            }

            if (model.FechJuego <= DateTime.Today.AddDays(2))
            {
                ModelState.AddModelError("FechJuego", "La fecha ingresada no puede ser menor a 2 días contando hoy");
            }



            try
            {
                if (ModelState.IsValid)
                {
                    db = new airSoftAppEntities();
                    {
                        TB_JUEGO ObjJuego = new TB_JUEGO 
                        {
                            CODJUEGO = model.CodJuego,
                            NOMJUEGO = model.NomJuego,
                            DESCJUEGO = model.DescJuego,
                            FECHJUEGO = model.FechJuego,
                            IMGJUEGO = model.AvatarJuego,
                            ESTJUEGO = true,
                            IDESCUADRON = model.IdEscuadronJuego,
                            IDMODOJUEGO = model.IdModoJuego,
                            IDTIPOJUEGO = model.IdTipoJuego,
                            IDTIPOPARTIDA = model.IdTipoPartida,
                            IDCOMUNA = model.IdComuna,
                            IDPERSONA = model.IdPersonaJuego
                        };

                        db.TB_JUEGO.Add(ObjJuego);
                        db.SaveChanges();

                        TB_PARTICIPA_JUEGO ObjAsistencia = new TB_PARTICIPA_JUEGO
                        {
                            ESTPARTJUEGO = 0,
                            IDPERSONA = model.IdPersonaJuego,
                            IDESCUADRON = model.IdEscuadronJuego,
                            IDJUEGO = db.TB_JUEGO.Where(a => a.CODJUEGO == model.CodJuego).FirstOrDefault().IDJUEGO
                                                       
                        };

                        db.TB_PARTICIPA_JUEGO.Add(ObjAsistencia);
                        db.SaveChanges();
                    }
                    return Redirect("~/Juego/IndexJuego");
                }
                else 
                {
                    PersonaController ObtFunc = new PersonaController();
                    //int IdPersona = ObtenerPersona(User.Identity.GetUserName()).IDPERSONA;

                    ViewData["idRegion"] = ObtFunc.CboRegion();
                    ViewData["IdModoJuego"] = ModoJuegoList();
                    ViewData["IdTipoPartida"] = TipoPartidaList();
                    ViewData["IdTipoJuego"] = TipoJuegoList();
                    ViewData["IdEscuadronJuego"] = EscuadronesList();
                    ViewData["IdComuna"] = ObtFunc.CboComuna();

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // GET: Juego/Edit/5
        public ActionResult EditarJuego(int idJuego)
        {

            if (idJuego == 0)
            {
                return Redirect("~/Juego/IndexJuego");
            }
            else 
            {
                db = new airSoftAppEntities();
                var juego = db.TB_JUEGO.Where(e => e.IDJUEGO == idJuego).FirstOrDefault();

                JuegoViewModel model = new JuegoViewModel();
                PersonaController persona = new PersonaController();


                model.IdJuego = juego.IDJUEGO;
                model.CodJuego = juego.CODJUEGO;
                model.NomJuego = juego.NOMJUEGO;
                model.DescJuego = juego.DESCJUEGO;
                model.FechJuego = (DateTime)juego.FECHJUEGO;
                model.AvatarJuego = juego.IMGJUEGO;

                var selectEscuadrones = (from a in db.TB_ESCUADRON join
                                        b in db.TB_INTEGRANTE on a.IDESCUADRON equals b.IDESCUADRON
                                         where b.IDPERSONA == juego.IDPERSONA
                                         select new
                                         {
                                             Value = a.IDESCUADRON,
                                             Text = a.NOMESCUADRON
                                         });

                ViewData["idEscuadronJuego"] = new SelectList(selectEscuadrones, "Value", "Text", (int)juego.IDESCUADRON);

                var selectModoJuego = (from a in db.TB_MODO_JUEGO
                                         select new
                                         {
                                             Value = a.IDMODOJUEGO,
                                             Text = a.DESCMODOJUEGO
                                         });

                ViewData["IdModoJuego"] = new SelectList(selectModoJuego, "Value", "Text", (int)juego.IDMODOJUEGO);

                var selectTipoJuego = (from a in db.TB_TIPO_JUEGO
                                         select new
                                         {
                                             Value = a.IDTIPOJUEGO,
                                             Text = a.DESCTIPOJUEGO
                                         });

                ViewData["idTipoJuego"] = new SelectList(selectTipoJuego, "Value", "Text", (int)juego.IDTIPOJUEGO);

                var selectTipoPartida = (from a in db.TB_TIPO_PARTIDA
                                        select new
                                        {
                                            Value = a.IDTIPOPARTIDA,
                                            Text = a.DESCTIPOPARTIDA
                                        }).ToList();

                ViewData["idTipoPartida"] = new SelectList(selectTipoPartida, "Value", "Text", (int)juego.IDTIPOPARTIDA);

                ViewData["idRegion"] = new SelectList(persona.CboRegion(), "Value", "Text",(int)juego.TB_COMUNA.IDREGION);
                ViewData["idComuna"] = new SelectList(persona.CboComuna((int)juego.TB_COMUNA.IDREGION), "Value", "Text", (int)juego.IDCOMUNA);


                //ObjJuego.IDPERSONA = Model.idPersonaJuego;

                return View(model);

            }


        }

        // POST: Juego/Edit/5
        [HttpPost]
        public ActionResult EditarJuego(JuegoViewModel model)
        {
            HttpPostedFileBase avatar = Request.Files[0];

            if (avatar.ContentLength == 0)
            {
                model.AvatarJuego = ObtenerByteGuardado(model.CodJuego);
            }
            else
            {
                if (avatar.FileName.EndsWith(".jpg"))
                {
                    model.AvatarJuego = ObtenerByteJuego(avatar);
                }
                else
                {
                    ModelState.AddModelError("ImgEscuadron", "Solo se aceptan imagenes formato .JPG");
                }
            }

            if (model.FechJuego <= DateTime.Today.AddDays(2))
            {
                ModelState.AddModelError("FechJuego", "La fecha ingresada no puede ser menor a 2 días contando hoy");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    db = new airSoftAppEntities();
                    {
                       var juego = db.TB_JUEGO.Where(a => a.CODJUEGO == model.CodJuego).FirstOrDefault();

                        juego.IMGJUEGO = model.AvatarJuego;
                        juego.NOMJUEGO = model.NomJuego;
                        juego.DESCJUEGO = model.DescJuego;
                        juego.FECHJUEGO = model.FechJuego;
                        juego.IDESCUADRON = model.IdEscuadronJuego;
                        juego.IDMODOJUEGO = model.IdModoJuego;
                        juego.IDTIPOJUEGO = model.IdTipoJuego;
                        juego.IDTIPOPARTIDA = model.IdTipoPartida;
                        juego.IDCOMUNA = model.IdComuna;

                        db.Entry(juego).State = EntityState.Modified;

                        db.SaveChanges();


                        return Redirect("~/Juego/IndexJuego");
                    }
                   
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // GET: Juego/Delete/5
        public ActionResult EliminarJuego(int idJuego)
        {

            db = new airSoftAppEntities();
            {

                List<TB_PARTICIPA_JUEGO> Lista = new List<TB_PARTICIPA_JUEGO>();

                Lista = db.TB_PARTICIPA_JUEGO.Where(a => a.IDJUEGO == idJuego).ToList();
                if (Lista != null)
                {
                    db.TB_PARTICIPA_JUEGO.RemoveRange(Lista);
                    db.SaveChanges();
                }

                db.TB_JUEGO.Remove(db.TB_JUEGO.Where(a => a.IDJUEGO == idJuego).FirstOrDefault());
                db.SaveChanges();

                return Redirect("~/Juego/IndexJuego");

            }
        }




        //----------------Funciones para Controlador de juego---------------------------------------


        public TB_PERSONA ObtenerPersona(string correo) //Obtiene datos de una persona existente
        {
            db = new airSoftAppEntities();
            {
                var odatos = db.TB_PERSONA.Where(a => a.CORREOPER.Equals(correo)).FirstOrDefault();
                return odatos;
            }
        }



        public byte[] ObtenerByteJuego(HttpPostedFileBase avatar) // Obtiene los byte de la imagen 
        {

            byte[] imagenData = null;

            if (avatar != null && avatar.ContentLength > 0)
            {

                using (var imagenBinaria = new BinaryReader(avatar.InputStream))
                {
                    imagenData = imagenBinaria.ReadBytes(avatar.ContentLength);
                }
            }
            return imagenData;
        }

        public byte[] ObtenerByteGuardado(string codJuego) // consulta una imagen guardada
        {
            byte[] imagenData = null; //Convierte la imagen a byte

            db = new airSoftAppEntities();
            {
                imagenData = db.TB_JUEGO.Where(d => d.CODJUEGO == codJuego).FirstOrDefault().IMGJUEGO;
            }
            return imagenData;
        }


        public ActionResult ConvertirImagenJuego(string CodJuego)
        {
            db = new airSoftAppEntities();
            {
                var imagen = (from a in db.TB_JUEGO
                              where a.CODJUEGO == CodJuego
                              select a.IMGJUEGO).FirstOrDefault();

                return File(imagen, "imagenes/jpg");
            }
        }
        public string Token()
        {

            db = new airSoftAppEntities();
            {
                string CodJuego = "";
                int Longitud = 5;
                Guid MiGuid = Guid.NewGuid();
                string Token = MiGuid.ToString().Replace("-", string.Empty).Substring(0, Longitud);

                var Cod = db.TB_JUEGO.Where(b => b.CODJUEGO == Token).ToString();

                if (Cod == Token)
                {
                    CodJuego = this.Token();
                }
                else
                {
                    CodJuego = Token;
                }
                return CodJuego;
            }


        }


        public List<SelectListItem> ModoJuegoList()
        {
            List<SelectListItem> ModoJuegoLIst = new List<SelectListItem>();

            using (airSoftAppEntities db = new airSoftAppEntities())
            {
                ModoJuegoLIst = (from e in db.TB_MODO_JUEGO
                             select new SelectListItem
                             {
                                 Value = e.IDMODOJUEGO.ToString(),
                                 Text = e.DESCMODOJUEGO.ToString()
                             }).ToList();
            }
            return (ModoJuegoLIst);

        }

        public List<SelectListItem> TipoJuegoList()
        {
            List<SelectListItem> TipoJuegoLIst = new List<SelectListItem>();

            using (airSoftAppEntities db = new airSoftAppEntities())
            {
                TipoJuegoLIst = (from e in db.TB_TIPO_JUEGO
                                 select new SelectListItem
                                 {
                                     Value = e.IDTIPOJUEGO.ToString(),
                                     Text = e.DESCTIPOJUEGO.ToString()
                                 }).ToList();
            }
            return (TipoJuegoLIst);

        }

        public List<SelectListItem> TipoPartidaList()
        {
            List<SelectListItem> TipoPartidaList = new List<SelectListItem>();

            using (airSoftAppEntities db = new airSoftAppEntities())
            {
                TipoPartidaList = (from e in db.TB_TIPO_PARTIDA
                                 select new SelectListItem
                                 {
                                     Value = e.IDTIPOPARTIDA.ToString(),
                                     Text = e.DESCTIPOPARTIDA.ToString()
                                 }).ToList();
            }
            return (TipoPartidaList);

        }

        public List<SelectListItem> EscuadronesList()
        {
            int IdPersona = ObtenerPersona(User.Identity.GetUserName()).IDPERSONA;
            List<SelectListItem> EscuadronesList = new List<SelectListItem>();

            using (airSoftAppEntities db = new airSoftAppEntities())
            {
                EscuadronesList = (from e in db.TB_ESCUADRON
                               join a in db.TB_INTEGRANTE on  e.IDESCUADRON equals a.IDESCUADRON
                               where a.CAPINTEGRANTE == true && a.IDPERSONA == IdPersona
                                   select new SelectListItem
                                   {
                                       Value = e.IDESCUADRON.ToString(),
                                       Text = e.NOMESCUADRON.ToString()
                                   }).ToList();
            }
            return (EscuadronesList);

        }

        public bool ValidaCantidadEscuadrones()
        {
            int IdPersona = ObtenerPersona(User.Identity.GetUserName()).IDPERSONA;

            db = new airSoftAppEntities();
            {
                if (db.TB_INTEGRANTE.Where(a => a.IDCREADOR == IdPersona).ToList().Count() == 0 )
                {
                    return true;
                }
            }
            return false;
        }

    }

}
