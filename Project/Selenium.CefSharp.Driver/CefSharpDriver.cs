﻿using Codeer.Friendly;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using OpenQA.Selenium;
using System;
using System.Drawing;
using Codeer.TestAssistant.GeneratorToolKit;
using OpenQA.Selenium.Html5;
using OpenQA.Selenium.Internal;
using System.Collections.ObjectModel;
using Codeer.Friendly.Dynamic;
using System.Linq;
using Selenium.CefSharp.Driver.InTarget;
using OpenQA.Selenium.Interactions.Internal;
using System.Collections.Generic;
using OpenQA.Selenium.Interactions;

namespace Selenium.CefSharp.Driver
{
    [ControlDriver(TypeFullName = "CefSharp.Wpf.ChromiumWebBrowser|CefSharp.WinForms.ChromiumWebBrowser")]
    public class CefSharpDriver :
        IWebDriver,
        IJavaScriptExecutor,
        IFindsById,
        IFindsByClassName,
        IFindsByLinkText,
        IFindsByName,
        IFindsByTagName,
        IFindsByXPath,
        IFindsByPartialLinkText,
        IFindsByCssSelector,
        IHasApplicationCache,
        IHasWebStorage,
        ITakesScreenshot,
#pragma warning disable CS0618
        IHasInputDevices,
#pragma warning restore CS0618
        IAllowsFileDetection,
        IActionExecutor,
        IAppVarOwner,
        IUIObject
    {
        CefSharpFrameDriver _currentFrame;
        CefSharpFrameDriver _mainFrame;

        internal ChromiumWebBrowserDriver ChromiumWebBrowser { get; }

        public WindowsAppFriend App => ChromiumWebBrowser.App;

        public AppVar AppVar => ChromiumWebBrowser.AppVar;

        public Size Size => ChromiumWebBrowser.Size;

#pragma warning disable CS0618
        public IKeyboard Keyboard => new ObsoleteKeyboard();
#pragma warning restore CS0618

#pragma warning disable CS0618
        public IMouse Mouse => new ObsoleteMouse();
#pragma warning restore CS0618

        public bool IsActionExecutor => true;

        public IFileDetector FileDetector { get; set; } = new DefaultFileDetector();

        public string Url
        {
            get => _mainFrame.Url;
            set
            {
                _mainFrame.Url = value;
                _currentFrame = _mainFrame;
            }
        }

        public CefSharpDriver(AppVar appVar)
        {
            ChromiumWebBrowser = new ChromiumWebBrowserDriver(appVar);
            _mainFrame =  _currentFrame = new CefSharpFrameDriver(this, null, () => (AppVar)ChromiumWebBrowser.GetMainFrame(), new IWebElement[0]);
            ChromiumWebBrowser.WaitForLoading();
        }

        public void Dispose() => AppVar.Dispose();

        public ITargetLocator SwitchTo() => new TargetLocator(this);

        public INavigation Navigate() => new Navigation(this);

        public Point PointToScreen(Point clientPoint) => _currentFrame.PointToScreen(clientPoint);

        public void ShowDevTools() => ChromiumWebBrowser.ShowDevTools();

        public void Activate() => _currentFrame.Activate();

        public IWebElement CreateWebElement(int id) => _currentFrame.CreateWebElement(id);

        public Screenshot GetScreenshot() => _currentFrame.GetScreenshot();

        public string Title => _currentFrame.Title;

        public bool HasApplicationCache => true;

        public IApplicationCache ApplicationCache => new CefSharpApplicationCache(this);

        public bool HasWebStorage => true;

        public IWebStorage WebStorage => new CefSharpWebStorage(this);

        public string PageSource => (string)ExecuteScript("return document.documentElement.outerHTML;");

        public IWebElement FindElement(By by) => ElementFinder.FindElementFromDocument(this, by);

        public ReadOnlyCollection<IWebElement> FindElements(By by) => ElementFinder.FindElementsFromDocument(this, by);

        public object ExecuteScript(string script, params object[] args) => _currentFrame.ExecuteScript(script, args);

        public object ExecuteAsyncScript(string script, params object[] args) => _currentFrame.ExecuteAsyncScript(script, args);

        public IWebElement FindElementById(string id) => FindElement(By.Id(id));

        public ReadOnlyCollection<IWebElement> FindElementsById(string id) => FindElements(By.Id(id));

        public IWebElement FindElementByClassName(string className) => FindElement(By.ClassName(className));

        public ReadOnlyCollection<IWebElement> FindElementsByClassName(string className) => FindElements(By.ClassName(className));

        public IWebElement FindElementByName(string name) => FindElement(By.Name(name));

        public ReadOnlyCollection<IWebElement> FindElementsByName(string name) => FindElements(By.Name(name));

        public IWebElement FindElementByTagName(string tagName) => FindElement(By.TagName(tagName));

        public ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName) => FindElements(By.TagName(tagName));

        public IWebElement FindElementByXPath(string xpath) => FindElement(By.XPath(xpath));

        public ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath) => FindElements(By.XPath(xpath));

        public IWebElement FindElementByCssSelector(string cssSelector) => FindElement(By.CssSelector(cssSelector));

        public ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector) => FindElements(By.CssSelector(cssSelector));

        public IWebElement FindElementByLinkText(string linkText) => FindElement(By.LinkText(linkText));

        public ReadOnlyCollection<IWebElement> FindElementsByLinkText(string linkText) => FindElements(By.LinkText(linkText));

        public IWebElement FindElementByPartialLinkText(string partialLinkText) => FindElement(By.PartialLinkText(partialLinkText));

        public ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText) => FindElements(By.PartialLinkText(partialLinkText));

        public void PerformActions(IList<ActionSequence> actionSequenceList) => ActionsAnalyzer.PerformActions(this, actionSequenceList);

        public void ResetInputState() { }

        class Navigation : INavigation
        {
            CefSharpDriver _this;

            public Navigation(CefSharpDriver driver) => _this = driver;

            public void Back()
            {
                _this._mainFrame.ExecuteScript("window.history.back();");
                _this._currentFrame = _this._mainFrame;
                _this.ChromiumWebBrowser.WaitForLoading();
            }

            public void Forward()
            {
                _this._mainFrame.ExecuteScript("window.history.forward();");
                _this._currentFrame = _this._mainFrame;
                _this.ChromiumWebBrowser.WaitForLoading();
            }

            public void GoToUrl(string url) => _this.Url = url;

            public void GoToUrl(Uri url) => _this.Url = url.ToString();

            public void Refresh()
            {
                _this._mainFrame.ExecuteScript("window.location.reload();");
                _this._currentFrame = _this._mainFrame;
                _this.ChromiumWebBrowser.WaitForLoading();
            }
        }

        class TargetLocator : ITargetLocator
        {
            CefSharpDriver _this;

            public TargetLocator(CefSharpDriver driver) => _this = driver;

            public IWebDriver DefaultContent()
            {
                _this._currentFrame = _this._mainFrame;
                return _this;
            }

            public IWebElement ActiveElement() => _this.ExecuteScript("return document.activeElement;") as IWebElement;

            public IAlert Alert() => new CefSharpAlert(_this.App, _this.Url);

            public IWebDriver Frame(int frameIndex)
            {
                var frameElements = _this.FindElementsByTagName("iframe");
                var frameNames = frameElements.Select(e => e.GetAttribute("name")).ToList();
                var frameElement = frameElements[frameIndex];
                var frame = _this.App.Type<FrameFinder>().FindFrame(_this, _this._currentFrame, frameNames, frameIndex);
                if (((AppVar)frame).IsNull)
                {
                    throw new NotFoundException("Frame was not found.");
                }
                _this._currentFrame = new CefSharpFrameDriver(_this, _this._currentFrame, 
                    () => (AppVar)frame, 
                    _this._currentFrame.FrameElements.Concat(new []{ frameElement }).ToArray());
                return _this;
            }

            public IWebDriver Frame(string frameName)
            {
                var frameElements = _this.FindElementsByTagName("iframe");
                for (int i = 0; i < frameElements.Count; i++)
                {
                    var e = frameElements[i];
                    if (e.GetAttribute("id") == frameName || e.GetAttribute("name") == frameName)
                    {
                        return Frame(i);
                    }
                }
                throw new NotFoundException("Frame was not found.");
            }

            public IWebDriver Frame(IWebElement frameElementSrc)
            {
                var frameElement = (CefSharpWebElement)frameElementSrc;
                var frameElements = _this.FindElementsByTagName("iframe").Cast<CefSharpWebElement>().ToList();
                for (int i = 0; i < frameElements.Count; i++)
                {
                    var e = frameElements[i];
                    if (e.Id == frameElement.Id)
                    {
                        return Frame(i);
                    }
                }
                throw new NotFoundException("Frame was not found.");
            }

            public IWebDriver ParentFrame()
            {
                if (_this._currentFrame.ParentFrame == null) throw new NotFoundException("Frame was not found.");
                _this._currentFrame = _this._currentFrame.ParentFrame;
                return _this;
            }

            //don't support.
            public IWebDriver Window(string windowName) => throw new NotSupportedException();
        }

#pragma warning disable CS0618
        class ObsoleteKeyboard : IKeyboard
#pragma warning restore CS0618
        {
            public void PressKey(string keyToPress) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
            public void ReleaseKey(string keyToRelease) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
            public void SendKeys(string keySequence) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
        }

#pragma warning disable CS0618
        class ObsoleteMouse : IMouse
#pragma warning restore CS0618
        {
            public void Click(ICoordinates where) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
            public void ContextClick(ICoordinates where) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
            public void DoubleClick(ICoordinates where) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
            public void MouseDown(ICoordinates where) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
            public void MouseMove(ICoordinates where) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
            public void MouseMove(ICoordinates where, int offsetX, int offsetY) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
            public void MouseUp(ICoordinates where) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
        }

        //don't support.
        public string CurrentWindowHandle => throw new NotSupportedException();
        public ReadOnlyCollection<string> WindowHandles => throw new NotSupportedException();
        public void Close() => throw new NotSupportedException();
        public void Quit() => throw new NotSupportedException();
        public IOptions Manage() => throw new NotSupportedException();
    }
}
