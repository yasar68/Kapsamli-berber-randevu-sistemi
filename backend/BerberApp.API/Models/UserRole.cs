namespace BerberApp.API.Models
{
    public class UserRole
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int RoleId { get; set; }  // Rolün foreign key'i
        public Role? Role { get; set; }  // Navigation property
    }

}