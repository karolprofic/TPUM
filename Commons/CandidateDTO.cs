﻿using System;
using System.Collections.Generic;

namespace Commons
{
    public class CandidateDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Votes { get; set; }
    }
}
