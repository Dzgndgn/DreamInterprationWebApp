using System.ComponentModel.DataAnnotations;

namespace DreamAI.Dtos
{
    public class UserDto
    {
        [Required(ErrorMessage ="username zorunludur")]
        
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage ="şifre zorunludur")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="şifreler uyuşmuyor")]
        [Required(ErrorMessage ="şifreyi tekrardan giriniz")]
        public string reTypepassword { get; set; }
    }
}
