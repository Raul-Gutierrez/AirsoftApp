using AirsoftApp.Models;
using AirsoftApp.Models.ModeloLocal;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirsoftApp.Controllers
{

[Authorize(Roles = "ADMIN")]
    public class AdministrarController : Controller
    {
        // GET: Administrar
        public ActionResult IndexAdministrar()
        {
            ViewData["Roles"] = Roles();


            return View();
        }

        // GET: Administrar/Details/5
        public ActionResult Enrolar()
        {
            if (User.Identity.IsAuthenticated)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var idUsarioActual = User.Identity.GetUserId();

                    var roleManager = new RoleManager<IdentityRole>
                        (new RoleStore<IdentityRole>(db));
                    //Crear Rol
                    var resultado = roleManager.Create(new IdentityRole("ADMIN"));

                    var userManager = new UserManager<ApplicationUser>
                        (new UserStore<ApplicationUser>(db));
                    //Agregar usuario a rol
                    resultado = userManager.AddToRole(idUsarioActual, "ADMIN");
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
                                 Value = a.Id,
                                 Text = a.Name
                                        
                             }).ToList();
            }
              return (roles);
        }



    }
}
