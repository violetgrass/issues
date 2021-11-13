using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Violet.WorkItems.Service.Models;

namespace Violet.WorkItems.Service.Controllers;

[ApiController]
[Authorize("WorkItemPolicy")]
public class WorkItemDescriptorController : ControllerBase
{
    private readonly WorkItemManager _workItemManager;

    public WorkItemDescriptorController(WorkItemManager workItemManager)
    {
        _workItemManager = workItemManager;
    }

    [HttpGet("api/v1/projects/{projectCode}/workitems/{workItemId}/descriptor")]
    [ProducesResponseType(typeof(WorkItemDescriptorApiResponse), 200)]
    public async Task<ActionResult> GetDescriptorForWorkItem(string projectCode, string workItemId)
        => InternalGetDescriptorForWorkItem(await _workItemManager.GetAsync(projectCode, workItemId));

    [HttpGet("api/v1/projects/{projectCode}/types/{workItemType}/descriptor")]
    [ProducesResponseType(typeof(WorkItemDescriptorApiResponse), 200)]
    public async Task<ActionResult> GetDescriptorForTemplate(string projectCode, string workItemType)
        => InternalGetDescriptorForWorkItem(await _workItemManager.CreateTemplateAsync(projectCode, workItemType));

    private ActionResult InternalGetDescriptorForWorkItem(WorkItem item)
    {
        var properties = _workItemManager.DescriptorManager.GetCurrentPropertyDescriptors(item);
        var commands = _workItemManager.DescriptorManager.GetCurrentCommands(item);

        return Ok(new WorkItemDescriptorApiResponse()
        {
            Success = true,
            ProjectCode = item.ProjectCode,
            WorkItemId = item.Id,
            Properties = properties,
            Commands = commands,
        });
    }
}
