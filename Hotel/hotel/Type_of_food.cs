namespace Hotel.hotel
{
    internal class Type_of_food
    {
        public int id { get; set; }
        public string status { get; set; }



        public Type_of_food() { }

        public Type_of_food(string status, string description)
        {
            this.status = status;
        }

    }
}
