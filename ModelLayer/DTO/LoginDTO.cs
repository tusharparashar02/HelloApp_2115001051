  namespace ModelLayer.DTO
    {
        public class LoginDTO
        {
            //[RegularExpression(@"\w{4,5}", ErrorMessage = "Only letters, 4 to 5 characters.")]
            public string Email { get; set; }
            public string Password { get; set; }

            override
            public string ToString()
            {
            return $"Email: {Email}  Password: {Password}";
            ;
            }
        }
    }
