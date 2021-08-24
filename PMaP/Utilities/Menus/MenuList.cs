using Microsoft.AspNetCore.Mvc.Infrastructure;
using PMaP.Utilities.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMaP.Utilities.Menus {
    public class MenuList : MenuBase {
        private String _path = "Utilities/Menus/MenuList.xml";

        /// <summary>
        /// Constructor
        /// </summary>
        public MenuList(IActionContextAccessor actionContextAccessor)
            : base(actionContextAccessor) {
            this.Path = _path;
        }

    }
}