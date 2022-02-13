namespace UserApi.Properties
{

    public class UserDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Surname {get; set;}
        public string? Gender {get; set;}
        public string? BirthDate {get; set;}
        public string? CreatedAt {get; set;}

        //create userDto model.

        public UserDto() {} //requirement for the api input parameter.
        public UserDto(User user) =>
        (Id, Name, Surname, Gender, BirthDate, CreatedAt) = 
        (user.Id, user.Name, user.Surname, user.Gender, user.BirthDate, user.CreatedAt); 
    }

}

