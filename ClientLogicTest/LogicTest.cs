﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logic;
using Logic.Interfaces;

namespace LogicTest
{
    [TestClass]
    public class LogicTest
    {
        private IElectionSystem electionSystem = null!;

        [TestInitialize]
        public void Setup()
        {
            electionSystem = LogicAbstractAPI.Create().GetElectionSystem();
        }

        [TestMethod]
        public void GetElectionTitle_ShouldReturnCorrectTitle()
        {
            Assert.AreEqual("Ładowanie...", electionSystem.GetElectionTitle());
        }

        [TestMethod]
        public void GetCandidatesShouldReturnEmptyList()
        {
            var candidates = electionSystem.GetCandidates();
            Assert.IsNotNull(candidates);
            Assert.AreEqual(0, candidates.Count);
        }

        [TestMethod]
        public void CastValidVoteShouldNotThrowAndNotChangeCandidates()
        {
            var candidateId = Guid.NewGuid();
            electionSystem.CastVote(candidateId, "123456");
            var candidatesAfter = electionSystem.GetCandidates();
            Assert.AreEqual(0, candidatesAfter.Count);
        }

        [TestMethod]
        public void CastInvalidVoteShouldNotThrowAndNotChangeCandidates()
        {
            var candidateId = Guid.NewGuid();
            electionSystem.CastVote(candidateId, "999999");
            var candidatesAfter = electionSystem.GetCandidates();
            Assert.AreEqual(0, candidatesAfter.Count);
        }
    }
}
