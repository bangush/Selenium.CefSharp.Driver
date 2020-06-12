﻿using System.Diagnostics;
using System.IO;
using Codeer.Friendly.Windows;
using OpenQA.Selenium;
using Selenium.CefSharp.Driver;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using OpenQA.Selenium.Support.UI;

namespace Test
{
    public class BasicTestWinForms : BasicTest
    {
        WindowsAppFriend _app;
        CefSharpDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [SetUp]
        public void SetUp()
        {
            _driver.Url = this.GetHtmlUrl();
        }

        [OneTimeSetUp]
        public void ClassInit()
        {
            var appWithDriver = AppRunner.RunWinFormApp();
            _app = appWithDriver.App;
            _driver = appWithDriver.Driver;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Process.GetProcessById(_app.ProcessId).Kill();
        }
    }

    public class BasicTestWpf : BasicTest
    {
        WindowsAppFriend _app;
        CefSharpDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [SetUp]
        public void SetUp()
        {
            _driver.Url = this.GetHtmlUrl();
        }

        [OneTimeSetUp]
        public void ClassInit()
        {
            var appWithDriver = AppRunner.RunWpfApp();
            _app = appWithDriver.App;
            _driver = appWithDriver.Driver;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Process.GetProcessById(_app.ProcessId).Kill();
        }
    }

    public class BasicTestWeb : BasicTest
    {
        IWebDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [SetUp]
        public void initialize()
        {
            _driver.Url = "https://github.com/Codeer-Software/Selenium.CefSharp.Driver";
            _driver.Url = this.GetHtmlUrl();
        }

        [OneTimeSetUp]
        public void ClassInit()
        {
            _driver = new ChromeDriver();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _driver.Dispose();
        }
    }

    public abstract class BasicTest
    {
        public abstract IWebDriver GetDriver();

        protected string GetHtmlUrl()
        {
            var dir = GetType().Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);
            return Path.Combine(dir, @"Test\Controls.html");
        }

        [Test]
        public void TestTitle()
        {
            GetDriver().Title.Is("Controls for Test");
        }

        [Test]
        public void TestPageSource()
        {
            //check only not to throw exception.
            var pageSource = GetDriver().PageSource;
        }

        [Test]
        public void TestNavigation()
        {
            var navigate = GetDriver().Navigate();
            navigate.Back();
            GetDriver().Url.Contains("Selenium.CefSharp.Driver").IsTrue();
            navigate.Forward();
            GetDriver().Url.Contains("Controls").IsTrue();
            navigate.GoToUrl("https://github.com/Codeer-Software/Selenium.CefSharp.Driver");
            GetDriver().Url.Contains("Selenium.CefSharp.Driver").IsTrue();
            navigate.Refresh();
            GetDriver().Url.Contains("Selenium.CefSharp.Driver").IsTrue();
        }

        [Test]
        public void Alert()
        {
            GetDriver().FindElement(By.Id("alertJS")).Click();

            var wait = new WebDriverWait(GetDriver(), new System.TimeSpan(10000));

#pragma warning disable CS0618 // 型またはメンバーが旧型式です
            var alert = wait.Until(ExpectedConditions.AlertIsPresent());
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
            alert.Text.Is("test");
            alert.Accept();

            GetDriver().FindElement(By.Id("confirmJS")).Click();

#pragma warning disable CS0618 // 型またはメンバーが旧型式です
            var confirm = wait.Until(ExpectedConditions.AlertIsPresent());
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
            confirm.Text.Is("test");
            confirm.Dismiss();

            GetDriver().FindElement(By.Id("promptJS")).Click();

#pragma warning disable CS0618 // 型またはメンバーが旧型式です
            var prompt = wait.Until(ExpectedConditions.AlertIsPresent());
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
            prompt.SendKeys("abc");
            prompt.Accept();
        }
    }
}
