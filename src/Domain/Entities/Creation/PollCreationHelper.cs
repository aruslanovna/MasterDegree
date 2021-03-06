
using System;
using System.Collections.Generic;

namespace VotingApplication.Data.Model.Creation
{
    public static class PollCreationHelper
    {
        public static Poll Create()
        {
            return new Poll()
            {
                UUID = Guid.NewGuid(),
                ManageId = Guid.NewGuid(),

                PollType = PollType.Basic,
                Choices = new List<Choice>(),
                MaxPoints = null,
                MaxPerVote = null,
                InviteOnly = false,
                NamedVoting = false,
                ExpiryDateUtc = null,
                ChoiceAdding = false,

                CreatedDateUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow
            };
        }

    }
}
