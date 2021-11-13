﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Violet.WorkItems;

public class WorkItem
{
    public string ProjectCode { get; set; }
    public string Id { get; set; }
    public string WorkItemType { get; set; }
    public IEnumerable<Property> Properties { get; set; }
    public IEnumerable<LogEntry> Log { get; set; }

    public WorkItem()
    {
        ProjectCode = string.Empty;
        Id = string.Empty;
        WorkItemType = string.Empty;
        Properties = new List<Property>();
        Log = new List<LogEntry>();
    }
    public WorkItem(string projectCode, string id, string workItemType, IEnumerable<Property> properties, IEnumerable<LogEntry> log)
    {
        if (string.IsNullOrWhiteSpace(projectCode))
        {
            throw new ArgumentException("null or empty", nameof(projectCode));
        }

        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("null or empty", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(workItemType))
        {
            throw new ArgumentException("null or empty", nameof(workItemType));
        }

        ProjectCode = projectCode;
        Id = id;
        WorkItemType = workItemType;
        Properties = properties ?? throw new ArgumentNullException(nameof(properties));
        Log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public Property? this[string propertyName]
        => Properties.FirstOrDefault(property => property.Name == propertyName);
}
