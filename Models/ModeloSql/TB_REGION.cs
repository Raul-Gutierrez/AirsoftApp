//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AirsoftApp.Models.ModeloSql
{
    using System;
    using System.Collections.Generic;
    
    public partial class TB_REGION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_REGION()
        {
            this.TB_COMUNA = new HashSet<TB_COMUNA>();
        }
    
        public int IDREGION { get; set; }
        public string DESCREGION { get; set; }
        public string ABREREG { get; set; }
        public string CAPREG { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_COMUNA> TB_COMUNA { get; set; }
    }
}