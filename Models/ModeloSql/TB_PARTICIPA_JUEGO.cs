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
    
    public partial class TB_PARTICIPA_JUEGO
    {
        public int IDPARTICIPANTE { get; set; }
        public Nullable<int> IDPERSONA { get; set; }
        public Nullable<int> IDJUEGO { get; set; }
        public Nullable<int> IDESCUADRON { get; set; }
        public Nullable<int> ESTPARTJUEGO { get; set; }
    
        public virtual TB_ESCUADRON TB_ESCUADRON { get; set; }
        public virtual TB_JUEGO TB_JUEGO { get; set; }
        public virtual TB_PERSONA TB_PERSONA { get; set; }
    }
}
