using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.Dtos
{
	public class LoginDto
	{
        [Required] // Each Validation Attribute Has Default Error Msg, Here I Using Default Error Message
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        /*We Don't Need Here To Use DataType(DataType.Password) {*****} Because This Handled In Front-end*/
        public string Password { get; set; }
    }
}
