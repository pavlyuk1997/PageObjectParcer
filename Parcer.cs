using Atata;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace ParcerTry
{
    using _ = OrdinaryPage;

    public class Parcer
    {
        [Test]
        public void SampleTest()
        {
            string path = @"c:\temp\MyTest.txt";

            List<string> urls = new List<string> { "https://atata.io/getting-started/", "https://atata.io/components/", "https://atata.io/control-search/" };
            Dictionary<string, List<PageOblectElement>> openWith = new Dictionary<string, List<PageOblectElement>>();
            List<AtataContext> contexts = new List<AtataContext>();

            try
            {
                Parallel.ForEach(urls, url =>
                {
                    var currentContext = AtataContext.Configure().Build();
                    contexts.Add(currentContext);

                    List<PageOblectElement> pageObjectElements = new List<PageOblectElement>();
                    var pageObject = Go.To<_>(url: url);
                    var links = pageObject.FindAll<Link<_>>();

                    Parallel.ForEach(links, link =>
                    {
                        var findStrategy = GetFindStrategy(pageObject, link);
                        if (findStrategy == null)
                        {
                            return;
                        }

                        var propertyAsString = PropertyStrategy.GetElementPropertyInStringForm(pageObject, link);
                        PageOblectElement pageOblectElement = new PageOblectElement
                        {
                            Strategy = findStrategy,
                            Element = propertyAsString
                        };

                        pageObjectElements.Add(pageOblectElement);
                    });

                    openWith.Add(url, pageObjectElements);
                });
            }
            catch
            {
                throw;
            }
            finally
            {
                foreach(var context in contexts)
                {
                    context.CleanUp();
                }    
            }
        }

        private string GetFindStrategy(_ page, Control<_> element)
        {
            string findStrategy;

            if (!string.IsNullOrEmpty(element.Attributes.Id))
            {
                var elements = page.Scope.FindElements(By.XPath($"//a[@id = {element.Attributes.Id}]"));

                if (elements.Count() == 1)
                {
                    return $" [FindById(\"{element.Attributes.Id})\"]";
                }
            }

            if (!string.IsNullOrEmpty(element.Attributes.TextContent.Value))
            {
                var elements = page.Scope.FindElements(By.XPath($"//a[normalize-space(.) = '{element.Attributes.TextContent.Value.Trim()}']"))
                    .Select(x=>x.Displayed && x.Enabled);

                if (elements.Count() == 1)
                {
                    return $" [FindByContent(\"{element.Attributes.TextContent.Value}\")]";
                }
            }

            foreach (var classAttribute in element.Attributes.Class.Value)
            {
                if (!string.IsNullOrEmpty(classAttribute))
                {
                    var elements = page.Scope.FindElements(OpenQA.Selenium.By.ClassName(classAttribute));
                    if (elements.Count() == 1)
                    {
                        findStrategy = $" [FindByClass(\"{classAttribute})\"]";
                    }
                    else
                    {
                        continue;
                    }
                    return findStrategy;
                }
            }

            string xpath = GetXpathOfElement(page, element);

            var parentElement = (UIComponent<_>)element;

            while (page.Scope.FindElements(By.XPath($"//{xpath}")).Count > 1)
            {
                parentElement = parentElement.Find<Control<_>>(new FindFirstAttribute { OuterXPath = "parent::" });
                try
                {
                    xpath = GetXpathOfElement(page, parentElement) + $"/{xpath}";
                }
                catch
                {
                    return null;
                }
            }

            findStrategy = $"[FindByXPath(\"//{xpath}\")]";

            return findStrategy;
        }

        private string GetXpathOfElement(_ page, UIComponent<_> element)
        {
            string xpath = element.Scope.TagName;

            if(!string.IsNullOrEmpty(element.Attributes.Id))
            {
                xpath += $"[@id = {element.Attributes.Id}]";
            }

            foreach (var classAttribute in element.Attributes.Class.Value)
            {
                if (!string.IsNullOrEmpty(classAttribute))
                {
                    var elements = page.Scope.FindElements(OpenQA.Selenium.By.ClassName(classAttribute));
                    if (elements.Count() == 1)
                    {
                        xpath += $"[contains(@class , \"{classAttribute}\")]";
                    }
                    else
                    {
                        continue;
                    }
                    return xpath;
                }
            }

            if(!string.IsNullOrEmpty(element.Attributes.TextContent) && element.Attributes.TextContent.Value.Trim().Length < 30)
            {
                xpath += $"[normalize-space(.) = '{element.Attributes.TextContent.Value.Trim()}']";
            }

            return xpath;
        }

    }
}
