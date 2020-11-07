﻿using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acidui
{
    public class ExtentFactory
    {
        HashSet<CMRelationEnd> path = new HashSet<CMRelationEnd>();

        public Extent CreateRootExtent(CMTable table, ExtentFlavorType type, String[] order, String[] values)
        {
            if (table == table.Root.RootTable)
            {
                return CreateExtentForRoot(table);
            }
            else
            {
                var root = table.Root.RootTable;

                var rootRelation = root.Relations[table.Name.Simple];

                return new Extent
                {
                    Flavor = (type, 2),
                    Children = new[] { CreateExtent(rootRelation, (type, 2), order, values) }
                };
            }
        }

        public Extent CreateExtentForRoot(CMTable rootTable)
        {
            return new Extent
            {
                Flavor = (ExtentFlavorType.Page, 1),
                Children = CreateSubExtents(rootTable, (ExtentFlavorType.Page, 1)).ToArray()
            };
        }

        IEnumerable<Extent> CreateSubExtents(CMTable parentTable, ExtentFlavor parentFlavor)
        {
            return parentTable.Relations.Values
                .Where(e => !path.Contains(e))
                .Select(e => CreateExtent(e, parentFlavor))
                .Where(e => e != null)
                ;
        }

        Extent CreateExtent(CMRelationEnd end, ExtentFlavor parentFlavor, String[] order = null, String[] values = null)
        {
            if (order != null)
            {
                foreach (var column in order) column.Assert(o => end.Table.Columns.ContainsKey(o),
                    $"Extent order column '{column}' is not in table '{end.Table.Name.LastPart}'");
            }

            var flavor = ReduceFlavor(parentFlavor, end);

            if (flavor.type == ExtentFlavorType.None || flavor.depth < 0) return null;

            path.Add(end);

            try
            {
                return new Extent
                {
                    Flavor = flavor,
                    RelationName = end.OtherEnd.Name,
                    Children = CreateSubExtents(end.Table, flavor).ToArray(),
                    Columns = SelectColumns(flavor, end.Table).Select(c => c.Name).ToArray(),
                    Order = order,
                    Values = values
                };
            }
            finally
            {
                path.Remove(end);
            }
        }

        IEnumerable<CMColumn> SelectColumns(ExtentFlavor flavor, CMTable table)
        {
            // We need to request all key columns in any case, as we need those
            // to form hyperlinks to the entity

            switch (flavor.type)
            {
                default:
                    return table.ColumnsInOrder;
            }
        }

        ExtentFlavor ReduceFlavor(ExtentFlavor flavor, CMRelationEnd end)
        {
            switch (flavor.type)
            {
                case ExtentFlavorType.PageList:
                    return (ExtentFlavorType.Page, 1);
                case ExtentFlavorType.BlockList:
                    return (ExtentFlavorType.Block, 1);
                case ExtentFlavorType.Page:
                    return end.IsMany ? (ExtentFlavorType.Inline, 1) : (ExtentFlavorType.Inline, 1);
                case ExtentFlavorType.Block:
                    return (ExtentFlavorType.Inline, 1);
                case ExtentFlavorType.Inline:
                    return end.IsMany ? (ExtentFlavorType.None, 0) : (ExtentFlavorType.Inline, flavor.depth - 1);
                default:
                    return (ExtentFlavorType.None, 0);
            }
        }
    }
}