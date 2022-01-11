using AirsoftApp.Models;
using AirsoftApp.Models.ModeloLocal;
using AirsoftApp.Models.ModeloSql;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
            ViewData["idRegion"] = CboRegion();

            PosicionModel pos = new PosicionModel();

            db = new airSoftAppEntities();
            {
                pos.ListPosiciones = db.TB_POSICION.ToList<TB_POSICION>();
            }

            model.ListPosPer = pos.ListPosiciones;
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
                            oPer.FECHAINSCRIPCION = Convert.ToDateTime(DateTime.Now.Date.ToString("dd-MM-yyyy"));



                            this.ModificaCorreoLocal(model.Correo, this.ObtenerId(User.Identity.GetUserName()));

                            db.TB_PERSONA.Add(oPer);
                            db.SaveChanges();

                            int IdPersona = this.ObtenerPersona(User.Identity.GetUserName()).IDPERSONA;

                            for (int i = 0; i < model.ListPosPer.Count; i++)
                            {
                                if (model.ListPosPer[i].IsChecked == true)
                                {
                                    oSel.IDPERSONA = IdPersona;
                                    oSel.IDPOSICION = model.ListPosPer[i].IDPOSICION;
                                    oSel.VALSELECCION = model.ListPosPer[i].IsChecked;

                                    db.TB_SELECCION.Add(oSel);
                                    db.SaveChanges();
                                }
                            }

                            using (ApplicationDbContext db = new ApplicationDbContext())
                            {
                                //Obtener usuario actual
                                var idUsarioActual = User.Identity.GetUserId();

                                var roleManager = new RoleManager<IdentityRole>
                                    (new RoleStore<IdentityRole>(db));

                                var userManager = new UserManager<ApplicationUser>
                                    (new UserStore<ApplicationUser>(db));

                                //Rol Actual 

                                var roles = userManager.GetRoles(idUsarioActual);

                                //Remover Rol
                                for (int i = 0; i < roles.Count; i++)
                                {
                                    var resultado = userManager.RemoveFromRole(idUsarioActual, roles[i]);
                                }

                                //Agregar usuario a rol
                                userManager.AddToRole(idUsarioActual, "USER");
                                
                            };

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

        #region[Editar leer]

        public ActionResult EditarPersona()
        {
            string user = User.Identity.GetUserName();
            PersonaViewModel model = new PersonaViewModel();
            TB_PERSONA odatos = this.ObtenerPersona(user);

            model.Run = (long)odatos.RUTPERSONA;
            model.Dv = odatos.DVPER;
            model.Nick = odatos.NICKPERSONA;
            model.Nombre = odatos.NOMPERSONA;
            model.Apellido_Paterno = odatos.APATERNOPER;
            model.Apellido_Materno = odatos.AMATERNOPER;
            model.Telefono = odatos.TELPERSONA;
            model.Correo = odatos.CORREOPER;
            model.Rango = infoRango(model.Experiencia);
            model.Experiencia = (int)odatos.EXPERIENCIAPER;

            var selectRegion = new SelectList(CboRegion(), "Value", "Text", (int)odatos.TB_COMUNA.IDREGION);
            var selectComuna = new SelectList(CboComuna((int)odatos.TB_COMUNA.IDREGION), "Value", "Text", (int)odatos.IDCOMUNA);

            ViewData["idRegion"] = selectRegion;
            ViewData["idComuna"] = selectComuna;

            model.ListPosPer = this.ObtenerPosiconesCheck(model.Run).ListPosiciones;

            return View(model);
        }
        #endregion

        #region[Editar Guardar]

        [HttpPost]
        public ActionResult EditarPersona(PersonaViewModel model, HttpPostedFileBase imgPerfil)
        {

            model.PerfilPersona = ObtenerByte(imgPerfil, model.Run);

            try
            {
                if (ModelState.IsValid)
                {

                    db = new airSoftAppEntities();
                    {

                        int IdPersona = this.ObtenerPersona(User.Identity.GetUserName()).IDPERSONA;


                        int h = 0;
                        var ObtenerSeleccion = new TB_SELECCION();

                        var del = db.TB_SELECCION.Where(d => d.IDPERSONA == IdPersona).ToList();

                        while (del != null && h < del.Count)
                        {
                            db.TB_SELECCION.Remove(del[h]);
                            db.SaveChanges();
                            h++;
                        }

                        for (int i = 0; i < model.ListPosPer.Count; i++)
                        {
                            if (model.ListPosPer[i].IsChecked == true)
                            {
                                ObtenerSeleccion.IDPERSONA = IdPersona;
                                ObtenerSeleccion.IDPOSICION = model.ListPosPer[i].IDPOSICION;
                                ObtenerSeleccion.VALSELECCION = model.ListPosPer[i].IsChecked;

                                db.TB_SELECCION.Add(ObtenerSeleccion);
                                db.SaveChanges();
                            }
                        }

                        TB_SELECCION ObtPersona = db.TB_SELECCION.Where(x => x.IDPERSONA == IdPersona).FirstOrDefault();

                        ObtPersona.TB_PERSONA.NICKPERSONA = model.Nick;
                        ObtPersona.TB_PERSONA.NOMPERSONA = model.Nombre;
                        ObtPersona.TB_PERSONA.APATERNOPER = model.Apellido_Paterno;
                        ObtPersona.TB_PERSONA.AMATERNOPER = model.Apellido_Materno;
                        ObtPersona.TB_PERSONA.TELPERSONA = model.Telefono;
                        ObtPersona.TB_PERSONA.PERFILPERSONA = model.PerfilPersona;
                        ObtPersona.TB_PERSONA.CORREOPER = model.Correo;
                        ObtPersona.TB_PERSONA.IDCOMUNA = model.IdComuna;

                        db.Entry(ObtPersona).State = EntityState.Modified;

                        db.SaveChanges();

                        UpdateCorreo(model.Correo, GetId(User.Identity.GetUserName()));

                    }

                    return Redirect("~/Persona/EditarPersona");
                }
                else
                {


                    var selectRegion = new SelectList(CboRegion(), "Value", "Text", (int)model.IdRegion);
                    var selectComuna = new SelectList(CboComuna((int)model.IdRegion), "Value", "Text", (int)model.IdComuna);
                    model.Rango = infoRango(model.Experiencia);

                    ViewData["idRegion"] = selectRegion;
                    ViewData["idComuna"] = selectComuna;



                    return View(model);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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

                int IdPersona = ObtenerPersona(User.Identity.GetUserName()).IDPERSONA;

                db.TB_SELECCION.RemoveRange(db.TB_SELECCION.Where(a => a.IDPERSONA == IdPersona));
                db.SaveChanges();
                
                db.TB_PERSONA.Remove(db.TB_PERSONA.Find(IdPersona));
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
                int IdPersona = ObtenerPersona(User.Identity.GetUserName()).IDPERSONA;

                int j = 0;
                pos.ListPosiciones = db.TB_POSICION.ToList<TB_POSICION>();
                var oSel = db.TB_SELECCION.Where(d => d.IDPERSONA == IdPersona).ToList();

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

        } //Decodifica los bytes de la imagen

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
            else // Si ya existe, busca la imagen guardada
            {
                db = new airSoftAppEntities();

                var Oper = db.TB_PERSONA.Where(d => d.RUTPERSONA == run).FirstOrDefault();

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
                AspNetUsers Email = new AspNetUsers();

                var id = db1.AspNetUsers.Where(d => d.Email.Contains(correo)).FirstOrDefault();
                string ID = id.Id;

                return ID;
            }
        }

        public void UpdateCorreo(string correo, string id)
        {
            db1 = new EntitiesLocal();
            {
                var oNet = db1.AspNetUsers.Find(id);

                oNet.Email = correo;
                oNet.UserName = correo;

                db1.Entry(oNet).State = System.Data.Entity.EntityState.Modified;
                db1.SaveChanges();
            }
        }

        public string GetId(string correo)
        {
            db1 = new EntitiesLocal();
            {
                AspNetUsers cor = new AspNetUsers();

                var id = db1.AspNetUsers.Where(d => d.Email.Contains(correo)).FirstOrDefault();
                string ID = id.Id;

                return ID;
            }
        }

        #region[cboComuna]
        [HttpGet]
        public List<SelectListItem> CboComuna(long idRegion)
        {
            List<SelectListItem> comunalst = new List<SelectListItem>();
            using (airSoftAppEntities db = new airSoftAppEntities())
            {
                comunalst = (from a in db.TB_COMUNA
                             where a.IDREGION == idRegion
                             select new SelectListItem
                             {
                                 Value = a.IDCOMUNA.ToString(),
                                 Text = a.DESCCOMUNA
                             }).ToList();
            }
            return (comunalst);
        }
        #endregion


        #region[cboRegion]
        public List<SelectListItem> CboRegion()
        {

            List<SelectListItem> regionlst = new List<SelectListItem>();

            using (airSoftAppEntities db = new airSoftAppEntities())
            {
                regionlst = (from e in db.TB_REGION
                             select new SelectListItem
                             {
                                 Value = e.IDREGION.ToString(),
                                 Text = e.DESCREGION.ToString()
                             }).ToList();
            }
            return (regionlst);
        }
        #endregion

        #endregion

        public string infoRango(int experiencia)
        {
            airSoftAppEntities db = new airSoftAppEntities();
            {
                var valRango1 = (from a in db.TB_RANGO
                                 where a.VALORRANGO > experiencia
                                 select a.DESCRANGO
                                 ).FirstOrDefault();
                if (valRango1 == null)
                {
                    valRango1 = (from a in db.TB_RANGO
                                 orderby a.VALORRANGO descending
                                 select a.DESCRANGO
                                     ).FirstOrDefault();
                    return valRango1;
                }
                return valRango1;
            }
        }

    }
}