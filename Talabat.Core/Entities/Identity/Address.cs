namespace Talabat.Core.Entities.Identity
{
    public class Address /*Belongs To Another DB*/
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public AppUser User { get; set; } // Nav Prop [ONE]
        public string AppUserId { get; set; } // FK (This Address Belongs To Which User?!)
    }
}

/*
 - One User Can Has More Than One Address/Branch
 - So, In Each Address There Is a Person Has (FirstName, LastName) That Will Receive The Order
 # In My Business The User Has One Address, however I Keep FirstName, LastName For Any Feature Change Request
 */