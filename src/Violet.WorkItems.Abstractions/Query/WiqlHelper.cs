using System;
using System.Linq;

namespace Violet.WorkItems.Query;

public static class WiqlHelper
{
    public static Func<WorkItem, bool> ConvertQueryClauseToPredicate(QueryClause clause)
        => clause switch
        {
            ProjectClause c => (WorkItem wi) => wi.ProjectCode == c.ProjectCode,
            WorkItemTypeClause c => (WorkItem wi) => wi.WorkItemType == c.WorkItemType,
            WorkItemIdClause c => (WorkItem wi) => wi.Id == c.WorkItemId,

            StringMatchClause c => (WorkItem wi) => wi[c.PropertyName]?.Value?.Contains(c.Match) ?? false,
            ValueMatchClause c => (WorkItem wi) => c.Values.Any(v => wi[c.PropertyName]?.Value == v),

            AndClause c => (WorkItem wi) => c.SubClauses.All(sc => ConvertQueryClauseToPredicate(sc)(wi)),
            OrClause c => (WorkItem wi) => c.SubClauses.Any(sc => ConvertQueryClauseToPredicate(sc)(wi)),
            NotClause c => (WorkItem wi) => !ConvertQueryClauseToPredicate(c.SubClause)(wi),

            JoinClause => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };
}