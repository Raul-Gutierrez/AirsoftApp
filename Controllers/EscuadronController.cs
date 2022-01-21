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
using System.Web.Helpers;
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
                                         IdIngreso = d.IDINTEGRANTES,
                                         Run = e.RUTPERSONA.ToString(),
                                         Nombre = e.NOMPERSONA,
                                         Escuadron = f.NOMESCUADRON,
                                         IdInt = d.IDINTEGRANTES
                                     }).ToList();

                return View(model);
            }

        }

        public class Solicitud
        {
            public int IdIngreso { get; set; }
            public string Run { get; set; }
            public string Nombre { get; set; }
            public string Escuadron { get; set; }
            public int IdInt { get; set; }

        }


        public JsonResult ListaNick(string CodEscuadron)
        {
            List<ElementJsonIntKey> list = new List<ElementJsonIntKey>();

            db = new airSoftAppEntities();
            {

               list = (from a in db.TB_INTEGRANTE
                                 where a.TB_ESCUADRON.CODESCUADRON == CodEscuadron && a.ESTINTEGRANTE == true
                                 select new ElementJsonIntKey
                                 {

                                     Nick = a.TB_PERSONA.NICKPERSONA
                                 }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        public class ElementJsonIntKey
        {
            public string Nick { get; set; }
        }


        public ActionResult NuevoEscuadron()
        {
            EscuadronViewModel model = new EscuadronViewModel
            {
                CodEscuadron = this.Token().ToUpper(),
                ImgEscuadron = null,
                NomEscuadron = "".ToUpper()
            };
            return View(model);
        }


        [HttpPost]
        public ActionResult NuevoEscuadron(EscuadronViewModel model)
        {
            HttpPostedFileBase imgPerfil = Request.Files[0];

            if (ValidarEscuadronUnico(model.NomEscuadron) == true)
            {
                ModelState.AddModelError("NomEscuadron", "El nombre ya existe");
            }

            if (imgPerfil.ContentLength == 0)
            {
                ModelState.AddModelError("ImgEscuadron", "Debe establecer una imagen de perfil");     
            }
            else
            {
                if (imgPerfil.FileName.EndsWith(".jpg"))
                {
                    model.ImgEscuadron = ObtenerByte(imgPerfil);
                }
                else
                {
                    ModelState.AddModelError("ImgEscuadron", "Solo se aceptan imagenes formato .JPG");
                }
            }

            db = new airSoftAppEntities();
            {
                try
                {

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
                                CAPINTEGRANTE = true,
                                IDCREADOR = IdPersona
                                
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
                        IdEscuadron = escuadron.IDESCUADRON,
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
        public ActionResult EditarEscuadron(EscuadronViewModel model)
        {
            if (model.CodEscuadron == null || model.CodEscuadron == "")
            {
                return Redirect("~/Escuadron/IndexEscuadron");
            }

            HttpPostedFileBase imgPerfil = Request.Files[0];

            if (ValidarEscuadronUnico(model.NomEscuadron) == true)
            {
                if (ObtenerNombreEscuadron(model.IdEscuadron) != model.NomEscuadron.ToUpper())
                {
                    ModelState.AddModelError("NomEscuadron", "El nombre ya existe");
                }
            }

            if (imgPerfil.ContentLength == 0)
            {
                model.ImgEscuadron = ObtenerByteGuardado(model.CodEscuadron);
            }
            else
            {
                if (imgPerfil.FileName.EndsWith(".jpg"))
                {
                    model.ImgEscuadron = ObtenerByte(imgPerfil);
                }
                else
                {
                    ModelState.AddModelError("ImgEscuadron", "Solo se aceptan imagenes formato .JPG");
                }
            }

            db = new airSoftAppEntities();
            {
                try
                {

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

        
        public ActionResult EliminarEscuadron(int id)//Pendiente de edicion
        {

            if (id == 0)
            {
                return Redirect("~/Escuadron/Index");
            }
            else
            {
                db = new airSoftAppEntities();
                {


                    List<TB_INTEGRANTE> Lista = new List<TB_INTEGRANTE>();

                    Lista = db.TB_INTEGRANTE.Where(a => a.IDESCUADRON == id).ToList();

                    if(Lista != null);
                    {
                        db.TB_INTEGRANTE.RemoveRange(Lista);
                        db.SaveChanges();
                    } 

                    // la lista de jeugos debe eliminar tanto la lista de juegos como los integrantes del juego 



                    db.TB_ESCUADRON.Remove(db.TB_ESCUADRON.Find(id));
                    db.SaveChanges();
                    //db.TB_JUEGO.RemoveRange(db.TB_JUEGO.Where(e=>e.IDESCUADRON == id));
                    //db.SaveChanges();
                    //db.TB_PARTICIPA_JUEGO.RemoveRange(db.TB_JUEGO.Where(e => e.IDESCUADRON == id));
                    //db.SaveChanges();
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

        public bool ValidarEscuadronUnico(string escuadron)
        {
            if(escuadron != null)
            { 
                db = new airSoftAppEntities();
                {
                    //var _escuadron = db.TB_ESCUADRON.Where(x => x.NOMESCUADRON == escuadron).FirstOrDefault();

                    if (db.TB_ESCUADRON.Where(x => x.NOMESCUADRON == escuadron).ToString() == null)
                    {
                        return true;
                    }
                    else
                    {
                    return false;
                    }
                }
            }
            else 
            {
                return false;
            }
        }

        public byte[] ObtenerByte(HttpPostedFileBase imgPerfil) // Obtiene los byte de la imagen 
        {

            byte[] imagenData = null;

            if (imgPerfil != null && imgPerfil.ContentLength > 0)
            {

                using (var imagenBinaria = new BinaryReader(imgPerfil.InputStream))
                {
                    imagenData = imagenBinaria.ReadBytes(imgPerfil.ContentLength);
                }
            }
            return imagenData;
        }

        public byte[] ObtenerByteGuardado(string codEscuadron) // consulta una imagen guardada
        {
            byte[] imagenData = null; //Convierte la imagen a byte

            db = new airSoftAppEntities();
            {
                imagenData = db.TB_ESCUADRON.Where(d => d.CODESCUADRON == codEscuadron).FirstOrDefault().IMGESCUADRON;
            }
            return imagenData;
        }

        public string ObtenerNombreEscuadron(int id)
        {
            db = new airSoftAppEntities();
            {
                string Nombre = db.TB_ESCUADRON.Find(id).NOMESCUADRON;
                return Nombre;
            }
        }



        #endregion
    }
}