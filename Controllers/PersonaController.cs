using AirsoftApp.Models;
using AirsoftApp.Models.ModeloLocal;
using AirsoftApp.Models.ModeloSql;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
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
            ViewData["idComuna"] = CboComuna();

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

        public ActionResult NuevaPersona(PersonaViewModel model )
        {
            HttpPostedFileBase imagenSubida = Request.Files[0];

            
            if (imagenSubida.ContentLength == 0)
            {
              
                    ModelState.AddModelError("PerfilPersona", "Debe establecer una imagen de perfil");

            }
            else
            {
                if (imagenSubida.FileName.EndsWith(".jpg"))
                {
                    model.PerfilPersona = ObtenerByte(imagenSubida);
                }
                else
                {
                    ModelState.AddModelError("PerfilPersona", "Solo se aceptan imagenes formato .JPG");
                }
            }

            if (model.Run != null)
            { 
            model.Run = model.Run.Replace(".", "").ToUpper();
            }
           

            if (ValidaRut(model.Run, model.Dv) == false)
            {
                model.Run = null;
                ViewBag.Mensaje = "El RUN ingresado es invalido o ya existe";
            }
            
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
                            oPer.NICKPERSONA = model.Nick.ToUpper();
                            oPer.NOMPERSONA = model.Nombre.ToUpper();
                            oPer.APATERNOPER = model.Apellido_Paterno.ToUpper();
                            oPer.AMATERNOPER = model.Apellido_Materno.ToUpper();
                            oPer.TELPERSONA = model.Telefono;
                            oPer.CORREOPER = model.Correo.ToUpper();
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

                            //AccountController salida = new AccountController();

                            Request.GetOwinContext().Authentication.SignOut();
                            Session.Abandon();
                            Session.Clear();

                            return Redirect("~/Account/Login");
                        }
                    }
                    else
                    {
                        var selectRegion = new SelectList(CboRegion(), "Value", "Text", (int)model.IdRegion);
                        var selectComuna = new SelectList(CboComuna((int)model.IdRegion), "Value", "Text", (int)model.IdComuna);
                        model.Rango = InfoRango(model.Experiencia);

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
            model.Rango = InfoRango((int)odatos.EXPERIENCIAPER);
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

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult EditarPersona(PersonaViewModel model)
        {
            HttpPostedFileBase imagenSubida = Request.Files[0];

            if (imagenSubida.ContentLength == 0)
            {
                model.PerfilPersona = ObtenerByteGuardado(model.Run);
            }
            else
            {
                if (imagenSubida.FileName.EndsWith(".jpg"))
                {
                    model.PerfilPersona = ObtenerByte(imagenSubida);
                }
                else
                {
                    ModelState.AddModelError("PerfilPersona", "Solo se aceptan imagenes formato .JPG");
                }
            }


            try
            {
                if (ModelState.IsValid)
                {

                    db = new airSoftAppEntities();
                    {

                        int IdPersona = this.ObtenerPersona(User.Identity.GetUserName()).IDPERSONA;//Obtiene i persona

                        int h = 0;
                        var ObtenerSeleccion = new TB_SELECCION();

                        var del = db.TB_SELECCION.Where(d => d.IDPERSONA == IdPersona).ToList();//Busca los datos de la seleccion

                        while (del != null && h < del.Count)//Elimina la seleccion
                        {
                            db.TB_SELECCION.Remove(del[h]);
                            db.SaveChanges();
                            h++;
                        }

                        for (int i = 0; i < model.ListPosPer.Count; i++)//Ingresa los nuevos datos de la seleccion
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

                        TB_PERSONA obtPersona = new TB_PERSONA();

                        obtPersona = db.TB_PERSONA.Where(x => x.IDPERSONA == IdPersona).FirstOrDefault();//Busca a la persona y reemplaza los datos cambiados
                        


                        obtPersona.NICKPERSONA = model.Nick.ToUpper();
                        obtPersona.NOMPERSONA = model.Nombre.ToUpper();
                        obtPersona.APATERNOPER = model.Apellido_Paterno.ToUpper();
                        obtPersona.AMATERNOPER = model.Apellido_Materno.ToUpper();
                        obtPersona.TELPERSONA = model.Telefono;
                        obtPersona.PERFILPERSONA = model.PerfilPersona;
                        obtPersona.CORREOPER = model.Correo.ToUpper();
                        obtPersona.IDCOMUNA = model.IdComuna;

                        db.Entry(obtPersona).State = EntityState.Modified;

                        db.SaveChanges();

                        UpdateCorreo(model.Correo, GetId(User.Identity.GetUserName()));

                    }

                    return Redirect("~/Persona/EditarPersona");
                }
                else
                {


                    var selectRegion = new SelectList(CboRegion(), "Value", "Text", (int)model.IdRegion);
                    var selectComuna = new SelectList(CboComuna((int)model.IdRegion), "Value", "Text", (int)model.IdComuna);
                    model.Rango = InfoRango(model.Experiencia);

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
        public ActionResult EliminarPersona(int id)
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

        public PosicionModel ObtenerPosiconesCheck(string Run)
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

        public ActionResult ConvertirImagen(string run)//obtiene los byte de la imagen guardada y los trasforma a un archivo de imagen 
        {
            db = new airSoftAppEntities();
            {
                var imagen = (from a in db.TB_PERSONA
                              where a.RUTPERSONA == run
                              select a.PERFILPERSONA).FirstOrDefault();

                return File(imagen, "imagenes/jpg");
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

        public byte[] ObtenerByteGuardado(string run) // consulta una imagen guardada
        {
            byte[] imagenData = null; //Convierte la imagen a byte

            db = new airSoftAppEntities();
            { 
                imagenData = db.TB_PERSONA.Where(d => d.RUTPERSONA == run).FirstOrDefault().PERFILPERSONA;
            }
            return imagenData;
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

        public List<SelectListItem> CboComuna()
        {

            List<SelectListItem> comunalst = new List<SelectListItem>();

            using (airSoftAppEntities db = new airSoftAppEntities())
            {
                comunalst = (from e in db.TB_COMUNA
                             select new SelectListItem
                             {
                                 Value = e.IDCOMUNA.ToString(),
                                 Text = e.DESCCOMUNA.ToString()
                             }).ToList();
            }
            return (comunalst);
        }

        public string InfoRango(int experiencia)
        {
            airSoftAppEntities db = new airSoftAppEntities();
            {
                var valRango1 = (from a in db.TB_RANGO
                                 where a.VALORRANGO >= experiencia
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

        public bool ValidaRut(string rut)
        {

            rut = rut.Replace(".", "").ToUpper();
            Regex expresion = new Regex("^([0-9]+-[0-9K])$");
            string dv = rut.Substring(rut.Length - 1, 1);
            if (!expresion.IsMatch(rut))
            {
                return false;
            }
            char[] charCorte = { '-' };
            string[] rutTemp = rut.Split(charCorte);
            if (dv != Digito(int.Parse(rutTemp[0])))
            {
                return false;
            }

            return true;
        }

        public bool ValidaRut(string rut, string dv)
        {
            db = new airSoftAppEntities();
            {
                if (ValidaRut(rut + "-" + dv) == true)
                {
                    var _run = db.TB_PERSONA.Where(a => a.RUTPERSONA == rut).FirstOrDefault();
                    if (_run != null)
                    {
                        return false;
                    }
                    else
                    { 
                        return true;
                    }
                }
                else 
                {
                    return false;
                }
            }            
        }

        public static string Digito(int rut)
        {
            int suma = 0;
            int multiplicador = 1;
            while (rut != 0)
            {
                multiplicador++;
                if (multiplicador == 8)
                    multiplicador = 2;
                suma += (rut % 10) * multiplicador;
                rut = rut / 10;
            }
            suma = 11 - (suma % 11);
            if (suma == 11)
            {
                return "0";
            }
            else if (suma == 10)
            {
                return "K";
            }
            else
            {
                return suma.ToString();
            }
        }

        #endregion
    }
}