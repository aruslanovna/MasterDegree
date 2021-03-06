using System;
using System.Collections.Generic;

namespace VotingApplication.Web.Api.Models.DBViewModels
{
    public class ResultsRequestResponseModel
    {
        public string PollName { get; set; }
        public List<string> Winners { get; set; }
        public List<ResultModel> Results { get; set; }
        public bool NamedVoting { get; set; }
        public DateTime? ExpiryDateUtc { get; set; }
        public string PollType { get; set; }
    }
}