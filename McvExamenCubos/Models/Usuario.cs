using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McvExamenCubos.Models
{
    [Table("USUARIOSCUBO")]
    public class Usuario
    {
        [Key]
        [Column("ID_USUARIO")]
        public int IdUsuario { get; set; }

        [Column("NOMBRE")]
        public string Nombre { get; set; }

        [Column("EMAIL")]
        public string Email { get; set; }


        [Column("PASS")]
        public string Password { get; set; }

        [Column("IMAGEN")]
        public string ImagenPerfil { get; set; }


    }
}
