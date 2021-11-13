using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Violet.WorkItems.Types;

public class DescriptorManager
{
    private readonly IDescriptorProvider _provider;
    private Dictionary<string, WorkItemDescriptor> _descriptors;

    public DescriptorManager(IDescriptorProvider provider)
    {
        _provider = provider;
        _descriptors = new Dictionary<string, WorkItemDescriptor>();
    }

    public async Task LoadAllAsync()
    {
        var list = await _provider.GetAllDescriptorsAsync();
        _descriptors = list.ToDictionary(item => item.Name);
    }

    public PropertyDescriptor? GetCurrentPropertyDescriptor(WorkItem workItem, string propertyName)
        => GetCurrentPropertyDescriptors(workItem).FirstOrDefault(p => p.Name == propertyName);

    public (bool success, IEnumerable<PropertyDescriptor> propertyDescriptors) GetAllPropertyDescriptors(string workItemType)
        => _descriptors.TryGetValue(workItemType, out var descriptor)
            ? (true, descriptor.Properties)
            : (false, Array.Empty<PropertyDescriptor>());

    public IEnumerable<PropertyDescriptor> GetCurrentPropertyDescriptors(WorkItem workItem)
    {
        if (_descriptors.TryGetValue(workItem.WorkItemType, out var descriptor))
        {
            var stages = EvaluateActiveStages(workItem, descriptor);

            foreach (var propertyDescriptor in descriptor.Properties)
            {
                bool isVisible = propertyDescriptor.IsVisible;
                bool isEditable = propertyDescriptor.IsEditable;
                var validators = new List<ValidatorDescriptor>(propertyDescriptor.Validators);

                foreach (var stage in stages)
                {
                    var stagePropertyDescriptor = stage.Properties.FirstOrDefault(spd => spd.Name == propertyDescriptor.Name);

                    if (stagePropertyDescriptor != null)
                    {
                        if (stagePropertyDescriptor.IsVisible.HasValue)
                        {
                            isVisible = isVisible && stagePropertyDescriptor.IsVisible.Value;
                        }
                        if (stagePropertyDescriptor.IsEditable.HasValue)
                        {
                            isEditable = isEditable && stagePropertyDescriptor.IsEditable.Value;
                        }

                        validators.AddRange(stagePropertyDescriptor.Validators);
                    }
                }

                yield return new PropertyDescriptor(propertyDescriptor.Name,
                    propertyDescriptor.DataType,
                    propertyDescriptor.Label,
                    propertyDescriptor.Hint,
                    propertyDescriptor.Description,
                    propertyDescriptor.PropertyType,
                    isVisible,
                    isEditable,
                    propertyDescriptor.InitialValue,
                    validators,
                    propertyDescriptor.ValueProvider);
            }
        }
    }

    public IEnumerable<LogEntryTypeDescriptor> GetCurrentLogEntryTypeDescriptors(WorkItem workItem)
        => _descriptors.TryGetValue(workItem.WorkItemType, out var descriptor)
            ? descriptor.Log.Types
            : Array.Empty<LogEntryTypeDescriptor>();

    public IEnumerable<CommandDescriptor> GetCurrentCommands(WorkItem workItem)
        => _descriptors.TryGetValue(workItem.WorkItemType, out var descriptor)
            ? EvaluateActiveStages(workItem, descriptor).SelectMany(stage => stage.Commands)
            : Array.Empty<CommandDescriptor>();

    private static IEnumerable<StageDescriptor> EvaluateActiveStages(WorkItem workItem, WorkItemDescriptor descriptor)
        => descriptor.Stages.Where(stage => EvaluateStageCondition(workItem, stage.Condition));

    private static bool EvaluateStageCondition(WorkItem workItem, ConditionDescriptor condition)
        => condition switch
        {
            PropertyValueConditionDescriptor pvcd => workItem.Properties.FirstOrDefault(p => p.Name == pvcd.PropertyName)?.Value == pvcd.ExpectedValue,
            _ => true,
        };
}
