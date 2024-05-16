namespace Api.Repository
{
    public class ContactUsRepository : IContactUsRepository
    {
        private readonly Context _context;

        public ContactUsRepository(Context context)
        {
            _context = context;
        }

    }
}
