using AirsoftApp.Models;
using AirsoftApp.Models.ModeloLocal;
using AirsoftApp.Models.ModeloSql;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirsoftApp.Controllers
{

[Authorize(Roles = "ADMIN")]
    public class AdministrarController : Controller
    {
        airSoftAppEntities db = null;
        EntitiesLocal db1 = null;
        // GET: Administrar
        public ActionResult IndexAdministrar()
        {
            return View();
        }

        // GET: Administrar/Details/5
        public ActionResult Enrolar(AdministrarViewModels model)
        {
            if (!ModelState.IsValid)
            {
                return View("IndexAdministrar",model);
            }
            string Usuario = ObtenerUsuario(model.Run);
            string Rol = ObtenerRol(model.IdRol);

            if (User.Identity.IsAuthenticated)
            {

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    //Obtener usuario actual
                    //var idUsarioActual = User.Identity.GetUserId();

                    var roleManager = new RoleManager<IdentityRole>
                        (new RoleStore<IdentityRole>(db));
                    //Crear Rol
                    //var resultado = roleManager.Create(new IdentityRole("ADMIN"));

                    var userManager = new UserManager<ApplicationUser>
                        (new UserStore<ApplicationUser>(db));

                    //Rol Actual 

                    var roles = userManager.GetRoles(Usuario);

                    //Remover Rol
                    for (int i = 0;i < roles.Count; i++ ) 
                    {
                        /*var resultado = */userManager.RemoveFromRole(Usuario, roles[i]);
                    }
                    

                    //Agregar usuario a rol

                    if (userManager.AddToRole(Usuario, "" + Rol + "").Succeeded)
                    {
                        ViewBag.Mensaje = "Se ingreso correctamente el usuario " + model.Run + "";
                        return View();
                    }
                    else
                    {
                        ViewBag.Mensaje = "Hubo un problema con el usuario " + model.Run + ",el usuario no es valido";
                        return View();
                    }
                };

            }

            return View();
        }

        // GET: Administrar/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Administrar/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Administrar/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: Administrar/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Administrar/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Administrar/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
        public List<SelectListItem> Roles()
        {
            List<SelectListItem> roles = new List<SelectListItem>();
            using (EntitiesLocal db = new EntitiesLocal())
            {
                roles = (from a in db.AspNetRoles
                             select new SelectListItem
                             {
                                 Value = a.Id.ToString(),
                                 Text = a.Name
                                        
                             }).ToList();
            }
              return (roles);
        }

        public string ObtenerUsuario(string Run)
        {

           

            db = new airSoftAppEntities();
            {
                var Persona = db.TB_PERSONA.Where(a => a.RUTPERSONA == Run).FirstOrDefault();

                db1 = new EntitiesLocal();
                {
                    string IdUser = db1.AspNetUsers.Where(a => a.Email == Persona.CORREOPER).FirstOrDefault().Id;
                 
                    return IdUser;
                }
            }
        }

        public string ObtenerRol(string IdRol)
        {
            db1 = new EntitiesLocal();
            {
                string Rol = db1.AspNetRoles.Find(IdRol).Name;

                return Rol;
            
            }
 
        }

        public JsonResult Estadistica01()
        {
            List<ElementJsonIntKey> list = new List<ElementJsonIntKey>();
            db = new airSoftAppEntities();
            {

                list = (from a in db.TB_ESTADISTICA
                        select new ElementJsonIntKey
                        {
                            Value = a.PERPORMES.ToString()
                        }).ToList();
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Estadistica02()
        {
            List<ElementJsonIntKey> list = new List<ElementJsonIntKey>();
            db = new airSoftAppEntities();
            {

                list = (from a in db.TB_ESTADISTICA
                        select new ElementJsonIntKey
                        {
                            Value = a.ESCUADRONPORMES.ToString()
                        }).ToList();
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public class ElementJsonIntKey
        {
            public string Value { get; set; }
           

        }
    }
}
