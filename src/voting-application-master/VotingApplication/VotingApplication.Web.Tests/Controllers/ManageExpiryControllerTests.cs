using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VotingApplication.Data.Context;
using VotingApplication.Data.Model;
using VotingApplication.Web.Api.Controllers;
using VotingApplication.Web.Api.Metrics;
using VotingApplication.Web.Api.Models.DBViewModels;
using VotingApplication.Web.Tests.TestHelpers;

namespace VotingApplication.Web.Tests.Controllers
{
    [TestClass]
    public class ManageExpiryControllerTests
    {
        public readonly Guid PollManageGuid = new Guid("961efb70-6767-4658-a95d-fea312c802ec");

        [TestClass]
        public class PutTests : ManageExpiryControllerTests
        {
            [TestMethod]
            [ExpectedHttpResponseException(HttpStatusCode.NotFound)]
            public void UnknownManageId_ReturnsNotFound()
            {
                IDbSet<Poll> existingPolls = DbSetTestHelper.CreateMockDbSet<Poll>();
                IContextFactory contextFactory = ContextFactoryTestHelper.CreateContextFactory(existingPolls);
                ManagePollExpiryRequest request = new ManagePollExpiryRequest { };

                ManageExpiryController controller = CreateManageExpiryController(contextFactory, new Mock<IMetricHandler>().Object);

                controller.Put(Guid.NewGuid(), request);
            }

            [TestMethod]
            public void NullExpiry_SetsExpiryToNull()
            {
                // Arrange
                IDbSet<Poll> existingPolls = DbSetTestHelper.CreateMockDbSet<Poll>();
                Poll existingPoll = CreatePoll();
                existingPolls.Add(existingPoll);

                IContextFactory contextFactory = ContextFactoryTestHelper.CreateContextFactory(existingPolls);
                ManagePollExpiryRequest request = new ManagePollExpiryRequest { };

                ManageExpiryController controller = CreateManageExpiryController(contextFactory, new Mock<IMetricHandler>().Object);
                controller.Validate(request);

                // Act
                controller.Put(PollManageGuid, request);

                // Assert
                Assert.IsNull(existingPoll.ExpiryDateUtc);
            }

            [TestMethod]
            public void SetExpiry_SetsExpiry()
            {
                IDbSet<Poll> existingPolls = DbSetTestHelper.CreateMockDbSet<Poll>();
                Poll existingPoll = CreatePoll();
                existingPolls.Add(existingPoll);

                DateTime expiry = DateTime.UtcNow.AddHours(1);

                IContextFactory contextFactory = ContextFactoryTestHelper.CreateContextFactory(existingPolls);
                ManagePollExpiryRequest request = new ManagePollExpiryRequest { ExpiryDateUtc = expiry };

                ManageExpiryController controller = CreateManageExpiryController(contextFactory, new Mock<IMetricHandler>().Object);

                controller.Put(PollManageGuid, request);

                Assert.AreEqual(expiry, existingPoll.ExpiryDateUtc);
            }

            [TestMethod]
            [ExpectedHttpResponseException(HttpStatusCode.BadRequest)]
            public void ExpiryInThePast_IsRejected()
            {
                IDbSet<Poll> existingPolls = DbSetTestHelper.CreateMockDbSet<Poll>();
                Poll existingPoll = CreatePoll();
                existingPolls.Add(existingPoll);

                DateTime past = DateTime.UtcNow.AddHours(-1);

                IContextFactory contextFactory = ContextFactoryTestHelper.CreateContextFactory(existingPolls);
                ManagePollExpiryRequest request = new ManagePollExpiryRequest { ExpiryDateUtc = past };

                ManageExpiryController controller = CreateManageExpiryController(contextFactory, new Mock<IMetricHandler>().Object);

                controller.Put(PollManageGuid, request);
            }

            [TestMethod]
            public void ChangedExpiryDateGeneratesMetric()
            {
                // Arrange
                IDbSet<Poll> existingPolls = DbSetTestHelper.CreateMockDbSet<Poll>();
                Poll existingPoll = CreatePoll();
                existingPolls.Add(existingPoll);

                DateTime future = DateTime.UtcNow.AddHours(1);

                IContextFactory contextFactory = ContextFactoryTestHelper.CreateContextFactory(existingPolls);
                ManagePollExpiryRequest request = new ManagePollExpiryRequest { ExpiryDateUtc = future };

                Mock<IMetricHandler> metricHandler = new Mock<IMetricHandler>();
                ManageExpiryController controller = CreateManageExpiryController(contextFactory, metricHandler.Object);

                // Act
                controller.Put(PollManageGuid, request);

                // Assert
                metricHandler.Verify(m => m.HandleExpiryChangedEvent(future, existingPoll.UUID), Times.Once());
            }

            [TestMethod]
            public void UnchangedExpiryDateDoesNotGenerateMetric()
            {
                // Arrange
                IDbSet<Poll> existingPolls = DbSetTestHelper.CreateMockDbSet<Poll>();
                Poll existingPoll = CreatePoll();
                DateTime currentExpiry = DateTime.UtcNow.AddHours(1);
                existingPoll.ExpiryDateUtc = currentExpiry;
                existingPolls.Add(existingPoll);


                IContextFactory contextFactory = ContextFactoryTestHelper.CreateContextFactory(existingPolls);
                ManagePollExpiryRequest request = new ManagePollExpiryRequest { ExpiryDateUtc = currentExpiry };

                Mock<IMetricHandler> metricHandler = new Mock<IMetricHandler>();
                ManageExpiryController controller = CreateManageExpiryController(contextFactory, metricHandler.Object);

                // Act
                controller.Put(PollManageGuid, request);

                // Assert
                metricHandler.Verify(m => m.HandleExpiryChangedEvent(It.IsAny<DateTimeOffset?>(), It.IsAny<Guid>()), Times.Never());
            }
        }


        public static ManageExpiryController CreateManageExpiryController(IContextFactory contextFactory, IMetricHandler metricHandler)
        {
            return new ManageExpiryController(contextFactory, metricHandler)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
        }

        private Poll CreatePoll()
        {
            return new Poll()
            {
                ManageId = PollManageGuid
            };
        }
    }
}
