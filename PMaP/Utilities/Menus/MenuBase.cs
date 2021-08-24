using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using PMaP.Common.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace PMaP.Utilities.Menus {
    public abstract class MenuBase {
        /// <summary>
        /// Variable con la ruta al fichero
        /// </summary>
        public String Path { get; set; }

        private readonly IActionContextAccessor _actionContextAccessor;

        protected MenuBase(IActionContextAccessor actionContextAccessor)
        {
            _actionContextAccessor = actionContextAccessor;
        }

        /// <summary>
        /// Construye una url a partir de un controlador y una acción
        /// </summary>
        /// <param name="controller">Nombre de controlador</param>
        /// <param name="action">Nombre de la acción</param>
        /// <param name="section">Nombre de la sección</param>
        /// <param name="area">Nombre de la parte del proyecto a la que pertenece</param>
        /// <returns>La url destino con el controlador, la acción y la sección</returns>
        protected String MakeUrl(String controller, String action, String section, String area = null) {
            String urlItem = "#";

            if (!string.IsNullOrEmpty(controller)) {
                urlItem = "~";
                if (!string.IsNullOrEmpty(area)) {
                    urlItem += "/" + area;
                }
                urlItem += "/" + controller;
                if (!string.IsNullOrEmpty(section)) {
                    urlItem += "/" + section;
                }
                if (!string.IsNullOrEmpty(action)) {
                    urlItem += "/" + action;
                }
            }

            return urlItem;
        }

        /// <summary>
        /// Determina si el controlador y la acción corresponden a la de la página (modulo) visitada (activo)
        /// </summary>
        /// <param name="controller">Nombre del controlador</param>
        /// <param name="action">Nombre de la acción</param>
        /// <param name="section">Nombre de la sección</param>
        /// <param name="area">Nombre de la parte del proyecto a la que pertenece</param>
        /// <returns>True si se corresponde con el módulo activo; False en otro caso</returns>
        public bool IsItemActive(String controller, String action, String section, String area = null) {
            //var pathActive = HttpContext.Current.Request.RequestContext.RouteData.Values;
            var pathActive = _actionContextAccessor.ActionContext.RouteData.Values;

            return (pathActive["controller"].ToString().ToUpper() == controller.ToUpper()) && (pathActive["action"].ToString().ToUpper() == action.ToUpper()
                && ((!pathActive.ContainsKey("section") && String.IsNullOrEmpty(section))
                    || (pathActive.ContainsKey("section") && !String.IsNullOrEmpty(section) && pathActive["section"].ToString().ToUpper() == section.ToUpper()))
                    && ((!pathActive.ContainsKey("area") && String.IsNullOrEmpty(area))
                    || (pathActive.ContainsKey("area") && !String.IsNullOrEmpty(area) && pathActive["area"].ToString().ToUpper() == area.ToUpper())));
        }

        /// <summary>
        /// Migas de pan de la página actual
        /// </summary>
        /// <returns>Retonar una lista de nombres que forman la ruta de la página actual</returns>
        public List<MenuItem> GetBreadcrumb() {
            var pathActive = _actionContextAccessor.ActionContext.RouteData.Values;
            String controller = pathActive["controller"].ToString().ToUpper();
            String action = pathActive["action"].ToString().ToUpper();
            String section = pathActive.ContainsKey("section") ? pathActive["section"].ToString().ToUpper() : null;
            String area = pathActive.ContainsKey("area") ? pathActive["area"].ToString().ToUpper() : null;

            //XDocument xmlMenu = XDocument.Load(HttpContext.Current.Server.MapPath(Path));
            XDocument xmlMenu = XDocument.Load(Path);

            // Se busca si entre los nodos de nivel 1 está la página actual
            var nodes = ((from item in xmlMenu.Descendants("Item")
                    .Where(o => (o.Attribute("Controller").Value.ToUpper().Equals(controller)
                                 && o.Attribute("Action").Value.ToUpper().Equals(action))
                                && (String.IsNullOrEmpty(section) || o.Attribute("Section").Value.ToUpper().Equals(section))
                                && (String.IsNullOrEmpty(area) || o.Attribute("Area").Value.ToUpper().Equals(area)))
                          select new {
                              Ancestors = item.Ancestors().Reverse(),
                              Text = XmlHelper.GetValueNode(item.Attribute("Text")),
                              Attributes = item.Attributes()
                          }).FirstOrDefault()
                ??
                    (from item in xmlMenu.Descendants("SubItem")
                        .Where(o => (o.Attribute("Controller").Value.ToUpper().Equals(controller)
                                     && o.Attribute("Action").Value.ToUpper().Equals(action))
                                    && (String.IsNullOrEmpty(section) || o.Attribute("Section").Value.ToUpper().Equals(section))
                                    && (String.IsNullOrEmpty(area) || o.Attribute("Area").Value.ToUpper().Equals(area)))
                     select new {
                         Ancestors = item.Ancestors().Reverse(),
                         Text = XmlHelper.GetValueNode(item.Attribute("Text")),
                         Attributes = item.Attributes()
                     }).FirstOrDefault())
                ??
                    (from item in xmlMenu.Descendants("Section")
                        .Where(o => (o.Attribute("Controller").Value.ToUpper().Equals(controller)
                                     && o.Attribute("Action").Value.ToUpper().Equals(action))
                                    && (String.IsNullOrEmpty(section) || o.Attribute("Section").Value.ToUpper().Equals(section))
                                    && (String.IsNullOrEmpty(area) || o.Attribute("Area").Value.ToUpper().Equals(area)))
                     let parent = item.Ancestors().FirstOrDefault()
                     where parent != null
                     select new {
                         Ancestors = item.Ancestors().Reverse(),
                         Text = XmlHelper.GetValueNode(item.Attribute("Text")),
                         Attributes = item.Attributes()
                     }).FirstOrDefault();

            if (nodes != null) {
                var nodesList = new List<MenuItem>();
                var withLink = "true";
                foreach (var ancestor in nodes.Ancestors) {
                    var attribuites = new List<String>();
                    ancestor.Attributes().ToList().ForEach(a => attribuites.Add(a.Name.LocalName));
                    if (attribuites.Contains("Controller")) {
                        withLink = XmlHelper.GetValueNode(ancestor.Attribute("WithLink"));
                        withLink = String.IsNullOrEmpty(withLink) ? "true" : "false";
                        nodesList.Add(new MenuItem(
                            TextMenus.ResourceManager.GetString(XmlHelper.GetValueNode(ancestor.Attribute("Text"))), "",
                            MakeUrl(XmlHelper.GetValueNode(ancestor.Attribute("Controller")),
                                XmlHelper.GetValueNode(ancestor.Attribute("Action")),
                                XmlHelper.GetValueNode(ancestor.Attribute("Section")),
                                XmlHelper.GetValueNode(ancestor.Attribute("Area"))), false, null,
                                String.Empty, false, Boolean.Parse(withLink)
                                ));
                    }
                }
                withLink = XmlHelper.GetValueNode(nodes.Attributes.FirstOrDefault(o => o.Name.LocalName == "WithLink"));
                withLink = String.IsNullOrEmpty(withLink) ? "true" : "false";
                nodesList.Add(new MenuItem(TextMenus.ResourceManager.GetString(nodes.Text), "",
                    MakeUrl(XmlHelper.GetValueNode(nodes.Attributes.FirstOrDefault(o => o.Name.LocalName == "Controller")),
                                XmlHelper.GetValueNode(nodes.Attributes.FirstOrDefault(o => o.Name.LocalName == "Action")),
                                XmlHelper.GetValueNode(nodes.Attributes.FirstOrDefault(o => o.Name.LocalName == "Section")),
                                XmlHelper.GetValueNode(nodes.Attributes.FirstOrDefault(o => o.Name.LocalName == "Area"))), true, null,
                                String.Empty, false, Boolean.Parse(withLink)));
                return nodesList;
            }

            return null;
        }

        /// <summary>
        /// Comprueba si en un árbol de nodos hay alguno activo
        /// </summary>
        /// <param name="items">Árbol de nodos a comprobar</param>
        /// <returns>Devuelve cierto si algún nodo del árbol está activo y falso en otro caso</returns>
        public Boolean SubmenuIsActive(List<MenuItem> items) {
            if (items == null) return false; // Si lleva vacío el nodo no es activo.
            foreach (var item in items) {
                if (item.IsActive) return true; // Si es un nodo activo se devuelve true.
                if (item.Submenu == null) continue; // Si no tiene hijos (submenu o sections) se pasa a la siguiente iteración.
                if (SubmenuIsActive(item.Submenu)) return true; // Si algún hijo (submenu o section) es activo entonces el nodo actual se considera activo y se devuelve true.
            }
            return false; // Si no se encontró ningún nodo de su árbol activo se devuelve falso ya que se considera que no está activo.
        }

        /// <summary>
        /// Construye de forma recursiva los elementos del menú a partir del nodo raíz
        /// </summary>
        /// <param name="rol">Perfil de usuario para el que se buscan los elementos del menú</param>
        /// <param name="items">Nodo a partir del que se buscan los elementos del menú</param>
        /// <returns>Devuelve la lista de nodos del nivel del menú</returns>
        public List<MenuItem> LoadMenu(String rol, IEnumerable<XElement> items) {
            List<MenuItem> list = new List<MenuItem>();
            
            string singleRol = rol;
            string managerRol = string.Empty;
            if (rol.Contains("Manager"))
            {
                var listRols = rol.Split(',');
                singleRol = listRols[1];
                managerRol = listRols[0];
            }

            var itemsList = items.Select(x => new {
                Text = XmlHelper.GetValueNode(x.Attribute("Text")),
                Icon = XmlHelper.GetValueNode(x.Attribute("Icon")),
                Area = XmlHelper.GetValueNode(x.Attribute("Area")),
                Controller = XmlHelper.GetValueNode(x.Attribute("Controller")),
                Action = XmlHelper.GetValueNode(x.Attribute("Action")),
                Section = XmlHelper.GetValueNode(x.Attribute("Section")),
                Target = XmlHelper.GetValueNode(x.Attribute("Target")),
                IsVisible = XmlHelper.GetValueNode(x.Attribute("Visible")),
                Submenu = x.Elements("Submenu").Elements("SubItem")
                                            .Where(p => (p.Attribute("Roles").Value.ToUpper().Equals("ALL") ||
                                                        ("," + p.Attribute("Roles").Value.Replace(" ", "") + ",").ToUpper().Contains("," + singleRol.ToUpper() + ",") ||
                                                        ("," + p.Attribute("Roles").Value.Replace(" ", "") + ",").ToUpper().Contains("," + managerRol.ToUpper() + ","))),
                Sections = x.Descendants("Sections").Elements("Section")
                                            .Where(p => (p.Attribute("Roles").Value.ToUpper().Equals("ALL") ||
                                                        ("," + p.Attribute("Roles").Value.Replace(" ", "") + ",").ToUpper().Contains("," + singleRol.ToUpper() + ",") ||
                                                        ("," + p.Attribute("Roles").Value.Replace(" ", "") + ",").ToUpper().Contains("," + managerRol.ToUpper() + ",")))
            }).ToList();

            foreach (var item in itemsList)
            {
                if (item.Target == "_blank")
                {

                }
            }

            foreach (var item in itemsList.Where(x => x.IsVisible != "0"))
            {
                List<MenuItem> submenusItems = null;
                if (item.Submenu != null) submenusItems = LoadMenu(rol, item.Submenu);

                // Nodos Sections. Son las acciones (añadir, editar, borrar...). Se utilizan para saber si el nodo padre está activo por estar en una acción
                Boolean sectionItemIsActive = false;
                if (item.Sections != null) {
                    var sections = item.Sections.Select(x => new {
                        Text = XmlHelper.GetValueNode(x.Attribute("Text")),
                        Icon = XmlHelper.GetValueNode(x.Attribute("Icon")),
                        Area = XmlHelper.GetValueNode(x.Attribute("Area")),
                        Controller = XmlHelper.GetValueNode(x.Attribute("Controller")),
                        Action = XmlHelper.GetValueNode(x.Attribute("Action")),
                        Section = XmlHelper.GetValueNode(x.Attribute("Section")),
                        IsVisible = XmlHelper.GetValueNode(x.Attribute("Visible")),
                    });
                    foreach (var section in sections) {
                        bool isSectionActive = IsItemActive(section.Controller, section.Action, section.Section, section.Area);
                        sectionItemIsActive = sectionItemIsActive || isSectionActive;
                    }
                }

                bool isTargetBlank = item.Target == "_blank";
                bool isSubmenuActive = IsItemActive(item.Controller, item.Action, item.Section, item.Area);
                list.Add(new MenuItem(TextMenus.ResourceManager.GetString(item.Text), item.Icon, MakeUrl(item.Controller, item.Action, item.Section, item.Area),
                   SubmenuIsActive(submenusItems) || isSubmenuActive || sectionItemIsActive, submenusItems, String.Empty, isTargetBlank, false, item.IsVisible));
            }
            return list.Count > 0 ? list : null;
        }

        /// <summary>
        /// Devuelve los elementos que forman el menú según el perfil (rol) de usuario
        /// </summary>
        /// <param name="rol">Perfil de usuario para el que se buscan los elementos del menú</param>
        public virtual List<MenuItem> GetMenuByRol(String rol) {
            try {
                //XDocument xmlMenu = XDocument.Load(HttpContext.Current.Server.MapPath(Path));
                XDocument xmlMenu = XDocument.Load(Path);

                string singleRol = rol;
                string managerRol = string.Empty;
                if (rol.Contains("Manager"))
                {
                    var listRols = rol.Split(',');
                    singleRol = listRols[1];
                    managerRol = listRols[0];
                }
                
                IEnumerable<XElement> items =
                    xmlMenu.Descendants("Items")
                        .Elements("Item")
                        .Where(
                            o =>
                                (o.Attribute("Roles").Value.ToUpper().Equals("ALL") ||
                                ("," + o.Attribute("Roles").Value.Replace(" ", "") + ",").ToUpper().Contains("," + singleRol.ToUpper() + ",") ||
                                ("," + o.Attribute("Roles").Value.Replace(" ", "") + ",").ToUpper().Contains("," + managerRol.ToUpper() + ",")));

                List<MenuItem> menuItems = LoadMenu(rol, items);
                return menuItems;
            }
            catch (Exception) {
                return new List<MenuItem>();
            }
        }

    }
}