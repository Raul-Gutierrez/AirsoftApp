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
    
    public partial class TB_RANGO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_RANGO()
        {
            this.TB_PERSONA = new HashSet<TB_PERSONA>();
        }
    
        public int IDRANGO { get; set; }
        public string DESCRANGO { get; set; }
        public Nullable<int> VALORRANGO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PERSONA> TB_PERSONA { get; set; }
    }
}
