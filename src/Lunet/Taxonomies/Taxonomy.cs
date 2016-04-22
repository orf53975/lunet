// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Lunet.Core;

namespace Lunet.Taxonomies
{
    [DebuggerDisplay("{Name} => {Singular} Terms: [{Terms.Count}]")]
    public class Taxonomy : DynamicObject<TaxonomyManager>
    {
        private readonly List<TaxonomyTerm> byName;
        private readonly List<TaxonomyTerm> byCount;

        public Taxonomy(TaxonomyManager parent, string name, string singular) : base(parent)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (singular == null) throw new ArgumentNullException(nameof(singular));
            Name = name;
            Singular = singular;
            Terms = new DynamicObject<Taxonomy>(this);
            byName = new List<TaxonomyTerm>();
            byCount = new List<TaxonomyTerm>();
            SetValue("name", Name, true);
            SetValue("terms", Terms, true);
            SetValue("by_alphabetical", ByName, true);
            SetValue("by_count", ByCount, true);
        }

        public string Name { get; }

        public string Singular { get; }

        public DynamicObject Terms { get; }

        public IEnumerable<TaxonomyTerm> ByName => byName;

        public IEnumerable<TaxonomyTerm> ByCount => byCount;

        public void AddTerm(TaxonomyTerm term)
        {
            Terms.SetValue(term.Name, term, true);
        }

        internal void Update()
        {
            byName.Clear();
            byCount.Clear();
            foreach (var termObj in Terms.Values)
            {
                var term = termObj as TaxonomyTerm;
                if (term == null)
                {
                    continue;
                }

                // Update the TaxonomyTerm
                term.Update();

                byName.Add(term);
                byCount.Add(term);
            }
            byName.Sort((left, right) => string.Compare(left.Name, right.Name, StringComparison.Ordinal));
            byCount.Sort((left, right) => left.PageCount.CompareTo(right.PageCount));
        }
    }
}