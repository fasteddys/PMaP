namespace PMaP.Helpers
{
    public class LanguageItem
    {
        /// <summary>
        /// Valor identificativo del idioma.
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Configuración regional que se utilizará por ejemplo para el formateo.
        /// </summary>
        public string Culture { get; set; }
        /// <summary>
        /// Nombre del idioma. Se pone el nombre de la variable en el fichero de recursos de internacionalización.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Nombre de la imagen que será su icono (bandera).
        /// </summary>
        public string Flag { get; set; }
        /// <summary>
        /// Indica si el idioma se puede utilizar en la web. 
        /// Valores posibles: true (activo) o false (inactivo).
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// Indica que el idioma se utilizará como idioma principal de la web.
        /// Valores posibles: true (activo) o false (inactivo).
        /// Sólo debe estar a true en un idioma.
        /// </summary>
        public bool Default { get; set; }
        /// <summary>
        /// Configuración de los valores numéricos
        /// </summary>
        //public NumbersLanguageItem NumbersLanguage { get; set; }
        /// <summary>
        /// Configuración de la moneda
        /// </summary>
        //public CurrencyLanguageItem CurrencyLanguage { get; set; }
    }
}
