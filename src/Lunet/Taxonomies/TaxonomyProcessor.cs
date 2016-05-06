﻿// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System.Linq;
using Lunet.Core;
using Lunet.Plugins;
using Scriban.Runtime;

namespace Lunet.Taxonomies
{
    public class TaxonomyProcessor : ProcessorBase
    {
        public override string Name => "taxonomies";

        public override void BeginProcess()
        {
            foreach (var page in Site.Pages)
            {
                var dyn = (DynamicObject)page;
                foreach (var tax in Site.Taxonomies.List)
                {
                    var termsObj = dyn[tax.Name];
                    var terms = termsObj as ScriptArray;
                    if (termsObj == null)
                    {
                        continue;
                    }
                    if (terms == null)
                    {
                        Site.Error("Invalid type");
                        continue;
                    }

                    foreach (var termNameObj in terms)
                    {
                        var termName = termNameObj as string;
                        if (termName == null)
                        {
                            Site.Error("// TODO ERROR ON TERM");
                            continue;
                        }

                        object termObj;
                        TaxonomyTerm term;
                        if (!tax.Terms.TryGetValue(termName, out termObj))
                        {
                            termObj = term = new TaxonomyTerm(tax, termName);
                            tax.Terms[termName] = termObj;
                        }
                        else
                        {
                            term = (TaxonomyTerm)termObj;
                        }

                        term.Pages.Add(page);
                    }
                }
            }

            // Update taxonomy computed
            foreach (var tax in Site.Taxonomies.List)
            {
                tax.Update();
            }


            foreach (var tax in Site.Taxonomies.List)
            {
                foreach (var term in tax.Terms.Values.OfType<TaxonomyTerm>())
                {
                    // term.Url
                    var content = new ContentObject(Site, Site.BaseDirectory, tax.Name)
                    {
                        ScriptObjectLocal = new DynamicObject<TaxonomyTerm>(term),
                        Url = term.Url,
                        Layout = tax.Name,
                        LayoutType = "term",
                        ContentType = ContentType.Html
                    };

                    content.ScriptObjectLocal.SetValue("pages", term.Pages.AsReadOnly(), true);

                    Site.Pages.Add(content);
                }
            }

            //TODO GENERATE PAGE OBJECT WITH LAYOUT AND CONTENT FOR TAXONOMIES
        }
    }
}