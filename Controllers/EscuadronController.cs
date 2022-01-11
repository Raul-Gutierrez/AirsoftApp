using AirsoftApp.Models;
using AirsoftApp.Models.ModeloSql;
using Microsoft.AspNet.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirsoftApp.Controllers
{

    [Authorize]
    public class EscuadronController : Controller
    {
        // GET: Escuadron


        airSoftAppEntities db = null;

        public ActionResult IndexEscuadron()
        {
            int IdPersona = ObtenerPersona(User.Identity.GetUserName()).IDPERSONA;

            db = new airSoftAppEntities();
            {
                var model = (from a in db.TB_INTEGRANTE
                             join b in db.TB_ESCUADRON on a.IDESCUADRON equals b.IDESCUADRON
                             join c in db.TB_PERSONA on a.IDPERSONA equals c.IDPERSONA
                             where c.IDPERSONA == IdPersona && a.CAPINTEGRANTE == true
                             select new EscuadronViewModel
                             {
                                 IdEscuadron = b.IDESCUADRON,
                                 NomEscuadron = b.NOMESCUADRON,
                                 CodEscuadron = b.CODESCUADRON,
                                 ImgEscuadron = b.IMGESCUADRON,
                                 EstEscuadron = (bool)b.ESTESCUADRON
                             });
                HomeController Home = new HomeController();
                int idPersona = Home.IdPersona(User.Identity.GetUserName());

                ViewBag.Pertenencia = (from d in db.TB_INTEGRANTE join e in db.TB_ESCUADRON on d.IDESCUADRON equals e.IDESCUADRON
                                       where d.ESTINTEGRANTE == true && d.IDPERSONA == idPersona && d.CAPINTEGRANTE == false 
                                       select new EscuadronViewModel
                                       {
                                           NomEscuadron = e.NOMESCUADRON,
                                           CodEscuadron = e.CODESCUADRON
                                       }).ToList();




                ViewBag.Solicitud = (from d in db.TB_INTEGRANTE
                                     join e in db.TB_PERSONA on d.IDPERSONA equals e.IDPERSONA
                                     join f in db.TB_ESCUADRON on d.IDESCUADRON equals f.IDESCUADRON
                                     where d.ESTINTEGRANTE == false && d.IDCREADOR == idPersona && d.CAPINTEGRANTE == false
                                     select new Solicitud
                                     {
                                         Run = (long)e.RUTPERSONA,
                                         Nombre = e.NOMPERSONA,
                                         Escuadron = f.NOMESCUADRON,
                                         idInt = d.IDINTEGRANTES
                                     }).ToList();

                return View(model);
            }

        }

        public class Solicitud
        {
            public long Run { get; set; }
            public string Nombre { get; set; }
            public string Escuadron { get; set; }
            public int idInt { get; set; }

        }

        public ActionResult NuevoEscuadron()
        {
            EscuadronViewModel model = new EscuadronViewModel
            {
                CapEscuadron = "",
                CodEscuadron = this.Token().ToUpper(),
                ImgEscuadron = null,
                NomEscuadron = ""
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult NuevoEscuadron(EscuadronViewModel model, HttpPostedFileBase imgPerfil)
        {

            db = new airSoftAppEntities();
            {
                try
                {
                    model.ImgEscuadron = ObtByteEscuadron(imgPerfil, model.CodEscuadron);

                    if (ModelState.IsValid)
                    {
                        db = new airSoftAppEntities();
                        {
                            TB_ESCUADRON oEsc = new TB_ESCUADRON
                            {
                                NOMESCUADRON = model.NomEscuadron.ToUpper(),
                                IMGESCUADRON = model.ImgEscuadron,
                                CODESCUADRON = model.CodEscuadron,
                                ESTESCUADRON = model.EstEscuadron,
                                FECHACREACION = Convert.ToDateTime(DateTime.Now.Date.ToString("dd-MM-yyyy"))

                            };

                            db.TB_ESCUADRON.Add(oEsc);
                            db.SaveChanges();

                            int IdEscuadron = this.ObtIdEscuadron(model.CodEscuadron);
                            int IdPersona = this.ObtenerPersona(User.Identity.GetUserName()).IDPERSONA;

                            TB_INTEGRANTE oInt = new TB_INTEGRANTE
                            {
                                IDPERSONA = IdPersona,
                                IDESCUADRON = IdEscuadron,
                                ESTINTEGRANTE = true,
                                CAPINTEGRANTE = true
                            };

                            db.TB_INTEGRANTE.Add(oInt);
                            db.SaveChanges();

                            return Redirect("~/Escuadron/IndexEscuadron");
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

        }

        public ActionResult EditarEscuadron(int IdEscuadron)
        {
            if (IdEscuadron == 0)
            {
                return Redirect("~/Escuadron/IndexEscuadron");
            }
            else
            {
                db = new airSoftAppEntities();
                {

                    var escuadron = db.TB_ESCUADRON.Where(e => e.IDESCUADRON == IdEscuadron).FirstOrDefault();
                    EscuadronViewModel model = new EscuadronViewModel
                    {
                        NomEscuadron = escuadron.NOMESCUADRON,
                        EstEscuadron = (bool)escuadron.ESTESCUADRON,
                        CodEscuadron = escuadron.CODESCUADRON
                    };
                    return View(model);
                }
            }


        }
        #region[GuardarEscuadron]

        [HttpPost]
        public ActionResult GuardarEscuadron(EscuadronViewModel model, HttpPostedFileBase imgPerfil)
        {
            if (model.CodEscuadron == null || model.CodEscuadron == "")
            {
                return Redirect("~/Escuadron/IndexEscuadron");
            }

            db = new airSoftAppEntities();
            {
                try
                {
                    model.ImgEscuadron = ObtByteEscuadron(imgPerfil, model.CodEscuadron);

                    if (ModelState.IsValid)
                    {
                        db = new airSoftAppEntities();
                        {
                            var oEsc = db.TB_ESCUADRON.Where(x => x.CODESCUADRON == model.CodEscuadron).FirstOrDefault();

                            oEsc.NOMESCUADRON = model.NomEscuadron.ToUpper();
                            oEsc.IMGESCUADRON = model.ImgEscuadron;
                            oEsc.ESTESCUADRON = model.EstEscuadron;
                            db.Entry(oEsc).State = EntityState.Modified;
                            db.SaveChanges();

                            return Redirect("~/Escuadron/IndexEscuadron");
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

        }
        #endregion

        
        public ActionResult EliminarEscuadron(int id)
        {

            if (id == 0)
            {
                return Redirect("~/Escuadron/Index");
            }
            else
            {
                db = new airSoftAppEntities();
                {
                    db.TB_INTEGRANTE.Remove(db.TB_INTEGRANTE.Where(x => x.IDESCUADRON == id).FirstOrDefault());
                    db.TB_ESCUADRON.Remove(db.TB_ESCUADRON.Where(X => X.IDESCUADRON == id).FirstOrDefault());
                    //Eventos
                    //partidas

                    db.SaveChanges();
                }
            }
            


            return Redirect("~/Escuadron/IndexEscuadron");
        }


        #region[Funciones escuadron]
        public ActionResult ConvertirImagen(string CodEscuadron)
        {
            db = new airSoftAppEntities();
            {
                var imagen = (from a in db.TB_ESCUADRON
                              where a.CODESCUADRON == CodEscuadron
                              select a.IMGESCUADRON).FirstOrDefault();

                return File(imagen, "imagenes/jpg");
            }

        }

        public TB_PERSONA ObtenerPersona(string correo) //Obtiene datos de una persona existente
        {
            db = new airSoftAppEntities();
            {
                var odatos = db.TB_PERSONA.Where(a => a.CORREOPER.Equals(correo)).FirstOrDefault();
                return odatos;
            }
        }

        public byte[] ObtByteEscuadron(HttpPostedFileBase imgPerfil, string CodEscuadron) // Obtiene los bytes de las imagenes 
        {

            byte[] imagenData = null;

            if (imgPerfil != null && imgPerfil.ContentLength > 0)
            {

                using (var imagenBinaria = new BinaryReader(imgPerfil.InputStream))
                {
                    imagenData = imagenBinaria.ReadBytes(imgPerfil.ContentLength);
                }

                return imagenData;
            }
            else
            {
                db = new airSoftAppEntities();

                var Esc = db.TB_ESCUADRON.Find(CodEscuadron);

                imagenData = Esc.IMGESCUADRON;

                return imagenData;

            }

        }

        public string Token()
        { 
            db = new airSoftAppEntities();
                {
                    string codEscuadron = "";
                    int longitud = 5;
                    Guid miGuid = Guid.NewGuid();
                    string token = miGuid.ToString().Replace("-", string.Empty).Substring(0, longitud);

                    var cod = db.TB_ESCUADRON.Where(b => b.CODESCUADRON == token).ToString();

                if (cod == token)
                {
                    codEscuadron = this.Token();
                }
                else 
                {
                    codEscuadron = token;
                }
                return codEscuadron;
                }       
        }

        public int ObtIdEscuadron(string CodEscuadron)
        {
            db = new airSoftAppEntities();
            {
                int Id = db.TB_ESCUADRON.Where(a => a.CODESCUADRON == CodEscuadron).FirstOrDefault().IDESCUADRON;
                return Id;            
            }

        }
        #endregion
    }
}