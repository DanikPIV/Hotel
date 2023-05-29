namespace Hotel.hotel
{
    internal class Status_client
    {
        public int id { get; set; }
        public string status { get; set; }
        public string description { get; set; }



        public Status_client() { }

        public Status_client(string status, string description)
        {
            this.status = status;
            this.description = description;
        }

    }
}
