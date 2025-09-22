using System.Collections.Generic;

namespace BerberApp.API.Models
{
    public class Role
    {
        public int Id { get; set; }  // Birincil anahtar

        public string Name { get; set; } = string.Empty;  // Rol adı, örn: "Admin", "Customer", "Barber"

        // Eğer kullanıcılarla çoktan çoğa ilişkisi olacaksa
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
