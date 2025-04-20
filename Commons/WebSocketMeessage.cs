using System;
using System.Collections.Generic;

namespace Commons
{
    public class BaseMessage
    {
        public string Action { get; set; }
    }

    public class ConnectionMessage : BaseMessage
    {
        public string ElectionName { get; set; }
    }

    public class CandidatesMessage : BaseMessage
    {
        public List<CandidateDTO> Candidates { get; set; }
    }

    public class VoteRequestMessage : BaseMessage
    {
        public Guid CandidateId { get; set; }
        public string AuthCode { get; set; }
    }

}
