using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using VotingApplication.Data.Model;
using VotingApplication.Web.Tests.E2E.Helpers;

namespace VotingApplication.Web.Tests.E2E
{
    [TestClass]
    public class CreationPageTests : E2ETest
    {
        [Ignore]
        [TestMethod]
        [TestCategory("E2ERewrite")]
        public void QuickPoll_DisabledWhenQuestionHasNoText()
        {
            using (IWebDriver driver = Driver)
            {
                GoToBaseUri(driver);

                IWebElement questionBox = FindElementById(driver, "quickpoll-question");
                questionBox.Clear();

                IWebElement quickPollButton = FindElementById(driver, "quickpoll-button");

                Assert.IsFalse(quickPollButton.Enabled);
            }
        }

        [Ignore]
        [TestMethod]
        [TestCategory("E2ERewrite")]
        public void CustomPoll_DisabledWhenQuestionHasNoText()
        {
            using (IWebDriver driver = Driver)
            {
                GoToBaseUri(driver);

                IWebElement questionBox = FindElementById(driver, "custompoll-question");
                questionBox.Clear();

                IWebElement customPollButton = FindElementById(driver, "custompoll-button");

                Assert.IsFalse(customPollButton.Enabled);
            }
        }

        [Ignore]
        [TestMethod]
        [TestCategory("E2ERewrite")]
        public void QuickPoll_DisabledWhenOnlyQuestionHasText()
        {
            using (IWebDriver driver = Driver)
            {
                GoToBaseUri(driver);

                IWebElement questionBox = FindElementById(driver, "quickpoll-question");
                questionBox.SendKeys("?");

                IWebElement quickPollButton = FindElementById(driver, "quickpoll-button");

                Assert.IsFalse(quickPollButton.Enabled);
            }
        }

        [Ignore]
        [TestMethod]
        [TestCategory("E2ERewrite")]
        public void QuickPoll_EnabledWhenQuestionAndChoiceHasText()
        {
            using (IWebDriver driver = Driver)
            {
                GoToBaseUri(driver);

                IWebElement questionBox = FindElementById(driver, "quickpoll-question");
                questionBox.SendKeys("?");

                IReadOnlyCollection<IWebElement> choices = FindElementsById(driver, "quickpoll-choice");
                choices.First().SendKeys("Option text");

                IWebElement quickPollButton = FindElementById(driver, "quickpoll-button");

                Assert.IsTrue(quickPollButton.Enabled);
            }
        }

        [Ignore]
        [TestMethod]
        [TestCategory("E2ERewrite")]
        public void CustomPoll_EnabledWhenQuestionHasText()
        {
            using (IWebDriver driver = Driver)
            {
                GoToBaseUri(driver);

                IWebElement questionBox = FindElementById(driver, "custompoll-question");
                questionBox.SendKeys("?");

                IWebElement customPollButton = FindElementById(driver, "custompoll-button");

                Assert.IsTrue(customPollButton.Enabled);
            }
        }

        [Ignore]
        [TestMethod]
        [TestCategory("E2ERewrite")]
        public void QuickPoll_NavigatesToManagePollPage()
        {
            using (IWebDriver driver = Driver)
            {
                using (var context = new TestVotingContext())
                {
                    GoToBaseUri(driver);

                    IWebElement questionBox = FindElementById(driver, "quickpoll-question");
                    questionBox.SendKeys("Does this work?");

                    IReadOnlyCollection<IWebElement> choices = FindElementsById(driver, "quickpoll-choice");
                    choices.First().SendKeys("Option text");

                    IWebElement quickPollButton = FindElementById(driver, "quickpoll-button");
                    quickPollButton.Click();

                    WaitForPageChange();

                    Poll newPoll = context.Polls.Single();

                    string expectedUrl = SiteBaseUri + "Poll/#/" + newPoll.UUID + "/Vote";

                    Assert.AreEqual(expectedUrl, driver.Url);
                }
            }
        }

        [Ignore]
        [TestMethod]
        [TestCategory("E2ERewrite")]
        public void CustomPoll_NavigatesToManagePollPage()
        {
            using (IWebDriver driver = Driver)
            {
                using (var context = new TestVotingContext())
                {
                    GoToBaseUri(driver);

                    IWebElement questionBox = FindElementById(driver, "custompoll-question");
                    questionBox.SendKeys("Does this work?");

                    IWebElement customPollButton = FindElementById(driver, "custompoll-button");
                    customPollButton.Click();

                    WaitForPageChange();

                    Poll newPoll = context.Polls.Single();

                    string expectedUrl = SiteBaseUri + "Manage/#/Manage/" + newPoll.ManageId;

                    Assert.AreEqual(expectedUrl, driver.Url);
                }
            }
        }
    }
}
