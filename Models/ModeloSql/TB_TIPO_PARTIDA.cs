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
    
    public partial class TB_TIPO_PARTIDA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_TIPO_PARTIDA()
        {
            this.TB_JUEGO = new HashSet<TB_JUEGO>();
        }
    
        public int IDTIPOPARTIDA { get; set; }
        public string DESCTIPOPARTIDA { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_JUEGO> TB_JUEGO { get; set; }
    }
}