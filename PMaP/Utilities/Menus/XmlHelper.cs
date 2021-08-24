using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PMaP.Utilities.Menus
{
    public class XmlHelper
    {
        public static string GetValueNode(XAttribute node)
        {
            if (node == null)
            {
                return string.Empty;
            }
            else
            {
                return node.Value;
            }
        }
    }
}
