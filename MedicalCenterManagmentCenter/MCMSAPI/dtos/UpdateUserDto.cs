namespace MCMSAPI.dtos
{
    public class UpdateUserDto
    {


        public string Email { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public bool is2FAEnable { get;set;}
    }
}
