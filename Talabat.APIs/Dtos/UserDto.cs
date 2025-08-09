namespace Talabat.APIs.Dtos
{
	/* DTO : Data Transfer Object, Obj Created From DTO Class Is The Obj That Responsable For Carrier يشيل
	 Of Data Transfered Between Processes [From Frontend -> Backend [POST/PUT/Delete] or From Backend -> Frontend [GET]]
	*/
	public class UserDto
	{
        public string DisplayName { get; set; }
		public string Email { get; set; }
		public string Token { get; set; }
    }
}
