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
    
    public partial class TB_JUEGO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_JUEGO()
        {
            this.TB_PARTICIPA_JUEGO = new HashSet<TB_PARTICIPA_JUEGO>();
        }
    
        public int IDJUEGO { get; set; }
        public Nullable<int> IDPERSONA { get; set; }
        public Nullable<int> IDESCUADRON { get; set; }
        public string CODJUEGO { get; set; }
        public Nullable<int> IDTIPOJUEGO { get; set; }
        public Nullable<int> IDMODOJUEGO { get; set; }
        public Nullable<int> IDCOMUNA { get; set; }
        public Nullable<int> IDTIPOPARTIDA { get; set; }
        public string NOMJUEGO { get; set; }
        public string DESCJUEGO { get; set; }
        public Nullable<System.DateTime> FECHJUEGO { get; set; }
        public byte[] IMGJUEGO { get; set; }
        public Nullable<bool> ESTJUEGO { get; set; }
    
        public virtual TB_COMUNA TB_COMUNA { get; set; }
        public virtual TB_ESCUADRON TB_ESCUADRON { get; set; }
        public virtual TB_MODO_JUEGO TB_MODO_JUEGO { get; set; }
        public virtual TB_PERSONA TB_PERSONA { get; set; }
        public virtual TB_TIPO_JUEGO TB_TIPO_JUEGO { get; set; }
        public virtual TB_TIPO_PARTIDA TB_TIPO_PARTIDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_PARTICIPA_JUEGO> TB_PARTICIPA_JUEGO { get; set; }
    }
}
