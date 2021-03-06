using System.Data.Entity;
using VotingApplication.Data.Model;

namespace VotingApplication.Web.Tests.E2E.Helpers.Clearers
{
    public class DBClearer : Clearer
    {
        public DBClearer(ITestVotingContext context) : base(context) { }

        public void ClearAll()
        {
            ((DbSet<Choice>)_context.Choices).RemoveRange(_context.Choices);
            ((DbSet<Vote>)_context.Votes).RemoveRange(_context.Votes);
            ((DbSet<Poll>)_context.Polls).RemoveRange(_context.Polls);
            ((DbSet<Ballot>)_context.Ballots).RemoveRange(_context.Ballots);

            _context.SaveChanges();
        }
    }
}
