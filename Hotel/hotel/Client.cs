namespace Hotel
{
    internal class Client
    {
        public int id { get; set; }
        public string name { get; set; }
        public int status { get; set; }
        public string pasport { get; set; }
        public string gender { get; set; }
        public string birthday { get; set; }
        public string address { get; set; }
        public string description { get; set; }



        public Client() { }

        public Client(string name, int status, string pasport, string gender, string birthday, string address, string description)
        {
            this.name = name;
            this.status = status;
            this.pasport = pasport;
            this.address = address;
            this.description = description;
            this.gender = gender;
            this.birthday = birthday;
        }

    }
}
