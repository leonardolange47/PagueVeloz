namespace PagueVeloz.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        public Customer(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }
}
