using AirsoftApp.Models;
using AirsoftApp.Models.ModeloLocal;
using AirsoftApp.Models.ModeloSql;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirsoftApp.Controllers
{
    [Authorize]
    public class PersonaController : Controller
    {
        airSoftAppEntities db = null;
        EntitiesLocal db1 = null;

        public ActionResult IndexPersona()
        {

            string user = User.Identity.GetUserName();
            TB_PERSONA USER = this.ObtenerPersona(user);
            if (USER == null)
            {
                return Redirect("~/Persona/NuevaPersona");
            }
            else
            {
                return Redirect("~/Persona/EditarPersona");

            }

        }

        #region[cboComuna]
        [HttpGet]
        public JsonResult Comuna(int idRegion)
        {
            List<ElementJsonIntKey> list = new List<ElementJsonIntKey>();
            db = new airSoftAppEntities();
            {
                list = (from a in db.TB_COMUNA
                        where a.IDREGION == idRegion
                        select new ElementJsonIntKey
                        {
                            Value = a.IDCOMUNA,
                            Text = a.DESCCOMUNA
                        }).ToList();
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region[class elemntJsonIntKey]
        public class ElementJsonIntKey
        {
            public int Value { get; set; }
            public string Text { get; set; }

        }
        #endregion

        #region[Nuevo]
        public ActionResult NuevaPersona()
        {
            string user = User.Identity.GetUserName();
            TB_PERSONA USER = this.ObtenerPersona(user);
            PersonaViewModel model = new PersonaViewModel();

            if (USER != null)
                {
                return Redirect("~/Persona/EditarPersona");
                }
            model.Correo = user;
            ViewData["idRegion"] = model.CboRegion();

            PosicionModel pos = new PosicionModel();

            db = new airSoftAppEntities();
            {
                pos.ListPosiciones = db.TB_POSICION.ToList<TB_POSICION>();
            }

            model.ListPosPer = pos.ListPosiciones;
            return View(model);

        }

        #endregion

        #region[Editar leer]

        public ActionResult EditarPersona()
        {
            string user = User.Identity.GetUserName();
            PersonaViewModel model = new PersonaViewModel();
            TB_PERSONA odatos = this.ObtenerPersona(user);

            model.Run = odatos.RUTPERSONA;
            model.Dv = odatos.DVPER;
            model.Nick = odatos.NICKPERSONA;
            model.Nombre = odatos.NOMPERSONA;
            model.Apellido_Paterno = odatos.APATERNOPER;
            model.Apellido_Materno = odatos.AMATERNOPER;
            model.Telefono = odatos.TELPERSONA;
            model.Correo = odatos.CORREOPER;
            model.Rango = model.infoRango(model.Experiencia);

            var selectRegion = new SelectList(model.CboRegion(), "Value", "Text", (int)odatos.TB_COMUNA.IDREGION);
            var selectComuna = new SelectList(model.cboComuna((int)odatos.TB_COMUNA.IDREGION), "Value", "Text", (int)odatos.IDCOMUNA);

            ViewData["idRegion"] = selectRegion;
            ViewData["idComuna"] = selectComuna;

            model.ListPosPer = this.ObtenerPosiconesCheck(model.Run).ListPosiciones;

            return View(model);
        }
        #endregion

        #region[Nuevo guardar]

        [HttpPost]
        public ActionResult NuevaPersonaGuardar(PersonaViewModel model, HttpPostedFileBase imgPerfil)
        {
            model.PerfilPersona = this.ObtenerByte(imgPerfil, model.Run);

            db = new airSoftAppEntities();
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        db = new airSoftAppEntities();
                        {
                            var oPer = new TB_PERSONA();
                            var oSel = new TB_SELECCION();

                            oPer.PERFILPERSONA = model.PerfilPersona;
                            oPer.RUTPERSONA = model.Run;
                            oPer.DVPER = model.Dv;
                            oPer.NICKPERSONA = model.Nick;
                            oPer.NOMPERSONA = model.Nombre;
                            oPer.APATERNOPER = model.Apellido_Paterno;
                            oPer.AMATERNOPER = model.Apellido_Materno;
                            oPer.TELPERSONA = model.Telefono;
                            oPer.CORREOPER = model.Correo;
                            oPer.EXPERIENCIAPER = 0;
                            oPer.IDRANGO = 1;
                            oPer.IDCOMUNA = model.IdComuna;

                            this.ModificaCorreoLocal(model.Correo, this.ObtenerId(User.Identity.GetUserName()));

                            db.TB_PERSONA.Add(oPer);
                            db.SaveChanges();


                            for (int i = 0; i < model.ListPosPer.Count; i++)
                            {
                                if (model.ListPosPer[i].IsChecked == true)
                                {
                                    oSel.RUTPERSONA = model.Run;
                                    oSel.IDPOSICION = model.ListPosPer[i].IDPOSICION;
                                    oSel.VALSELECCION = model.ListPosPer[i].IsChecked;

                                    db.TB_SELECCION.Add(oSel);
                                    db.SaveChanges();
                                }
                            }

                            return Redirect("~/Persona/EditarPersona");
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

        #region[Eliminar]
        public ActionResult EliminarPersona(long id)
        {
            using (EntitiesLocal dbl = new EntitiesLocal())
            {
                string user = User.Identity.GetUserName();
                dbl.AspNetUsers.Remove(dbl.AspNetUsers.Where(a => a.Email == user).FirstOrDefault());
                dbl.SaveChanges();

            }
            db = new airSoftAppEntities();
            {
                //db.TB_INTEGRANTE.Remove(db.TB_INTEGRANTE.Where(x => x.RUTPERSONA == id).FirstOrDefault());
                //db.ESCUADRON.Remove(db.ESCUADRON.Where(x => x.RUTPERSONA == id).FirstOrDefault());

                db.TB_SELECCION.RemoveRange(db.TB_SELECCION.Where(a => a.RUTPERSONA == id));
                //db.SaveChanges();
                
                db.TB_PERSONA.Remove(db.TB_PERSONA.Find(id));
                db.SaveChanges();

            }
            Request.GetOwinContext().Authentication.SignOut();
            //FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Clear();

            return Redirect("~/Account/Login ");
        }
        #endregion



        #region[FuncionesPersona]

        public TB_PERSONA ObtenerPersona(string correo)
        {
            db = new airSoftAppEntities();
            {
                var odatos = db.TB_PERSONA.Where(a => a.CORREOPER.Equals(correo)).FirstOrDefault();
                return odatos;
            }
        } //Obtiene datos de una persona existente 

        public PosicionModel ObtenerPosiconesCheck(long Run)
        {
            PosicionModel pos = new PosicionModel();
            db = new airSoftAppEntities();
            {
                int j = 0;
                pos.ListPosiciones = db.TB_POSICION.ToList<TB_POSICION>();
                var oSel = db.TB_SELECCION.Where(d => d.RUTPERSONA == Run).ToList();

                for (int i = 0; i < pos.ListPosiciones.Count; i++)
                {
                    if (j < oSel.Count)
                    {
                        if (oSel[j].IDPOSICION == pos.ListPosiciones[i].IDPOSICION)
                        {
                            pos.ListPosiciones[i].IsChecked = (bool)oSel[j].VALSELECCION;
                            j++;
                        }
                    }
                }
            }
            return pos;
        } //Obtiene las posiciones 

        public ActionResult ConvertirImagen(long run)
        {
            db = new airSoftAppEntities();
            {
                var imagen = (from a in db.TB_PERSONA
                              where a.RUTPERSONA == run
                              select a.PERFILPERSONA).FirstOrDefault();

                return File(imagen, "imagenes/jpg");
            }

        } //Decodifica los bytes de LA IMAGEN

        public byte[] ObtenerByte(HttpPostedFileBase imgPerfil, long run) // Obtiene los bytes de las imagenes 
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

                var Oper = db.TB_PERSONA.Find(run);

                imagenData = Oper.PERFILPERSONA;

                return imagenData;

            }

        }

        public void ModificaCorreoLocal(string correo, string id)
        {
            db1 = new EntitiesLocal();
            {
                var oNet = db1.AspNetUsers.Find(id);

                oNet.Email = correo;
                oNet.UserName = correo;

                db1.Entry(oNet).State = System.Data.Entity.EntityState.Modified;
                db1.SaveChanges();
            }
        } //Modifica el correo local

        public string ObtenerId(string correo)
        {
            db1 = new EntitiesLocal();
            {
                AspNetUsers cor = new AspNetUsers();

                var id = db1.AspNetUsers.Where(d => d.Email.Contains(correo)).FirstOrDefault();
                string ID = id.Id;

                return ID;
            }
        }

        #endregion

    }
}