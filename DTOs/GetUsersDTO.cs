namespace E_CommerceApp.DTOs
{
    public class GetUsersDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CityName { get; set; }
        public List<string>? Roles { get; set; }
    }
}
