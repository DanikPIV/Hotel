namespace Hotel
{
    internal class User
    {

        public int id { get; set; }
        public string root { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public int current { get; set; }



        public User() { }

        public User(string root, string login, string password)
        {
            this.root = root;
            this.login = login;
            this.password = password;
        }
        public User(string login, string password)
        {
            this.login = login;
            this.password = password;
        }

    }
}
