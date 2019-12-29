using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Violet.WorkItems.Types;

namespace Violet.WorkItems.Validation
{
    public class CompletenessValidator : IValidator
    {
        public IEnumerable<PropertyDescriptor> PropertyDescriptors { get; }

        public CompletenessValidator(IEnumerable<PropertyDescriptor> propertyDescriptors)
        {
            PropertyDescriptors = propertyDescriptors ?? throw new System.ArgumentNullException(nameof(propertyDescriptors));
        }

        public Task<IEnumerable<ErrorMessage>> ValidatePropertyAsync(WorkItem workItem, IEnumerable<PropertyChange> appliedChanges)
        {
            if (workItem is null)
            {
                throw new System.ArgumentNullException(nameof(workItem));
            }

            var errors = new List<ErrorMessage>();
            foreach (var propertyDescriptor in PropertyDescriptors)
            {
                var property = workItem.Properties.FirstOrDefault(p => p.Name == propertyDescriptor.Name);

                if (property is null)
                {
                    errors.Add(new ErrorMessage(nameof(CompletenessValidator), string.Empty, $"WorkItem does not have property '{propertyDescriptor.Name}' as described in type '{workItem.WorkItemType}.", workItem.ProjectCode, workItem.Id, propertyDescriptor.Name));
                }
            }

            return Task.FromResult((IEnumerable<ErrorMessage>)errors);
        }
    }
}