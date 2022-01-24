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



        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult IndexAdministrar(AdministrarViewModels model)
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

                    //Roles Actual es

                    var roles = userManager.GetRoles(Usuario);

                    /*Verificar si esta en el rol */
                    var usuario = userManager.IsInRole(Usuario, "USER");

                    var administrador = userManager.IsInRole(Usuario, "ADMIN");


                    if (usuario == true && Rol == "ADMIN") 
                    {
                        //Remover Rol
                        for (int i = 0; i < roles.Count; i++)
                        {
                            userManager.RemoveFromRole(Usuario, roles[i]);
                        }
                        //Agregar usuario a rol
                        if (userManager.AddToRole(Usuario, "" + Rol + "").Succeeded)
                        {
                            ViewData["MensajeVal"] = "Usuario enrrolado correctamente";
                            ViewBag.Mensaje = "ok";
                            return View();
                        }  
                    }

                    if (usuario == true && Rol == "USER")
                    {
    
                            ViewData["MensajeError"] = "El usuario no es un administrador";
                            ViewBag.Mensaje = "error";
                            return View();
         
                    }


                    if (administrador == true && Rol == "ADMIN")
                    {


                        ViewData["MensajeError"] = "El usuario ya está en el rol";
                        ViewBag.Mensaje = "error";
                        return View();
                    }

                    if (administrador == true && Rol == "USER")
                    {
                        //Remover Rol
                        for (int i = 0; i < roles.Count; i++)
                        {
                            userManager.RemoveFromRole(Usuario, roles[i]);
                        }
                        //Agregar usuario a rol
                        if (userManager.AddToRole(Usuario, "" + Rol + "").Succeeded)
                        {
                            ViewData["MensajeVal"] = "Usuario Desenrrolado correctamente";
                            ViewBag.Mensaje = "ok";
                            return View();
                        }
                    }
                    
                };

            }

            return View();
        }

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
