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

    [Authorize]
    public class EscuadronController : Controller
    {
        // GET: Escuadron


        airSoftAppEntities db = null;

        public ActionResult IndexEscuadron()
        {
            long Rut = this.ObtenerPersona(User.Identity.GetUserName()).RUTPERSONA;

            db = new airSoftAppEntities();
            {
                var Escuadron = db.TB_INTEGRANTE.Where(x => x.RUTPERSONA == Rut).ToList();
                return View(Escuadron);
            }
        }
        public ActionResult NuevoEscuadron()
        {
            EscuadronViewModel model = new EscuadronViewModel();

            model.CapEscuadron = f.Name(User.Identity.GetUserName());

            model.CodEscuadron = f.GetCode(1).ToUpper();

            return View(model);
        }

        [HttpPost]
        public ActionResult NuevoEscuadron(EscuadronViewModel model, HttpPostedFileBase imgPerfil)
        {

            db = new airSoftAppEntities();
            {
                try
                {
                    model.ImgEscuadron = f.obtByteEscuadron(imgPerfil, model.CodEscuadron);

                    if (ModelState.IsValid)
                    {
                        db = new airSoftAppEntities();
                        {
                            TB_ESCUADRON oEsc = new TB_ESCUADRON();

                            oEsc.NOMESCUADRON = model.NomEscuadron.ToUpper();
                            oEsc.IMGESCUADRON = model.ImgEscuadron;
                            oEsc.CODESCUADRON = model.CodEscuadron;
                            oEsc.ESTESCUADRON = true;
                            //oEsc.RUTPERSONA = f.oPer(User.Identity.GetUserName()).RUTPERSONA;


                            db.TB_ESCUADRON.Add(oEsc);
                            db.SaveChanges();

                            TB_INTEGRANTE oInt = new TB_INTEGRANTE();

                            oInt.RUTPERSONA = f.oPer(User.Identity.GetUserName()).RUTPERSONA;
                            oInt.CODESCUADRON = model.CodEscuadron;
                            oInt.ESTINTEGRANTE = true;
                            oInt.CAPINTEGRANTE = true;

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

        public ActionResult EditarEscuadron(string id)
        {
            if (id == null || id == "")
            {
                return Redirect("~/Escuadron/IndexEscuadron");
            }

            EscuadronViewModel model = new EscuadronViewModel();
            db = new airSoftAppEntities();
            {
                var escuadron = db.TB_ESCUADRON.Where(e => e.CODESCUADRON == id).FirstOrDefault();

                model.NomEscuadron = escuadron.NOMESCUADRON;
                model.CapEscuadron = escuadron.TB_PERSONA.NOMPERSONA + " " + escuadron.TB_PERSONA.APATERNOPER + " " + escuadron.TB_PERSONA.AMATERNOPER;
                model.EstEscuadron = (bool)escuadron.ESTESCUADRON;
                model.CodEscuadron = escuadron.CODESCUADRON;
            }

            return View(model);

        }

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
                    model.ImgEscuadron = f.obtByteEscuadron(imgPerfil, model.CodEscuadron);

                    if (ModelState.IsValid)
                    {
                        db = new airSoftAppEntities();
                        {
                            var oEsc = db.TB_ESCUADRON.Where(x => x.CODESCUADRON == model.CodEscuadron).FirstOrDefault();

                            oEsc.NOMESCUADRON = model.NomEscuadron.ToUpper();
                            oEsc.IMGESCUADRON = model.ImgEscuadron;
                            oEsc.ESTESCUADRON = model.EstEscuadron;
                            oEsc.RUTPERSONA = f.oPer(User.Identity.GetUserName()).RUTPERSONA;

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

        public ActionResult EliminarEscuadron(string id)
        {

            if (id == null || id == "")
            {
                return Redirect("~/Escuadron/Index");
            }

            db = new airSoftAppEntities();
            {
                db.TB_INTEGRANTE.Remove(db.TB_INTEGRANTE.Where(x => x.CODESCUADRON == id).FirstOrDefault());
                db.TB_ESCUADRON.Remove(db.TB_ESCUADRON.Where(X => X.CODESCUADRON == id).FirstOrDefault());
                //Eventos
                //partidas

                db.SaveChanges();
            }


            return Redirect("~/Escuadron/IndexEscuadron");
        }

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

    } 
}