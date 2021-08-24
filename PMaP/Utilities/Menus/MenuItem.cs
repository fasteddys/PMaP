using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMaP.Utilities.Menus {
    /// <summary>
    /// Clase con la estructura base de un nodo del menú
    /// </summary>
    public class MenuItem {
        /// <summary>
        /// Nombre del elemento del menú
        /// </summary>
        public String Text { get; set; }
        /// <summary>
        /// Imagen representativa del elemento del menú
        /// </summary>
        public String Icon { get; set; }
        /// <summary>
        /// Enlace del elemento del menú
        /// </summary>
        public String Url { get; set; }
        /// <summary>
        /// Valor booleano para indicar si el elemento del menú es el que se está visitando
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Lista de elementos de menú que constituyen el submenú
        /// </summary>
        public List<MenuItem> Submenu { get; set; }
        /// <summary>
        /// Valor booleano para indicar si el enlace hay que abrirlo en una ventana nueva
        /// </summary>
        public bool TargetBlank { get; set; }
        /// <summary>
        /// Clase css a aplicar al enlace
        /// </summary>
        public String StyleCss { get; set; }
        /// <summary>
        /// Indica si se puede o no utilizar la url para enlazar
        /// </summary>
        public Boolean WithLink { get; set; }
        /// <summary>
        /// Indica si es una opción de menú a mostrar (true) o no (false)
        /// </summary>
        public Boolean IsVisible { get; set; }

        /// <summary>
        /// Constructor vacío
        /// </summary>
        public MenuItem() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text">Nomre</param>
        /// <param name="icon">Imagen representativa</param>
        /// <param name="url">Enlace</param>
        /// <param name="isActive">Es o no el elemento visitado</param>
        /// <param name="submenu">Lista de elementos que forman el submenú hijo</param>
        /// <param name="styleCss">Clase css a aplicar</param>
        /// <param name="targetBlank">Indica si se abre en una ventana nueva</param>
        /// <param name="withLink">Indica si se la url se puede (valor a true) o no (valor a false) utilizar en un enlace. Opcional</param>
        /// <param name="isVisible">Indica si se ve o no la opción (1==Se ve; 0==No se ve). Si no llega valor se asigna 1</param>
        public MenuItem(string text, string icon, string url, bool isActive, List<MenuItem> submenu, string styleCss = "", bool targetBlank = false, bool withLink = true, string isVisible = "1")
        {
            Text = text;
            Icon = icon;
            Url = url;
            IsActive = isActive;
            Submenu = submenu;
            TargetBlank = targetBlank;
            StyleCss = styleCss;
            WithLink = withLink;
            IsVisible = Convert.ToBoolean(String.IsNullOrWhiteSpace(isVisible) ? 1 : Convert.ToInt32(isVisible));
        }
    }
}