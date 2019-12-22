using System;
using System.Threading.Tasks;
using Violet.WorkItems.Provider;
using Violet.WorkItems.Types;
using Violet.WorkItems.Types.CommonSdlc;
using Xunit;

namespace Violet.WorkItems.Validation
{
    public class MandatoryValidatorTest
    {
        [Fact]
        public async Task MandatoryValidator_Validate_Success()
        {
            // arrange
            WorkItemManager manager = BuildManager();

            var properties = new Property[] {
                new Property("A", "String", "aa"),
                new Property("B", "String", "bb"),
            };

            // act
            var result = await manager.CreateAsync("FOO", "BAR", properties);

            // assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.CreatedWorkItem);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task MandatoryValidator_Validate_NoSideEffect()
        {
            // arrange
            WorkItemManager manager = BuildManager();

            var properties = new Property[] {
                new Property("A", "String", "aa"),
                new Property("B", "String", ""),
            };

            // act
            var result = await manager.CreateAsync("FOO", "BAR", properties);

            // assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.CreatedWorkItem);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task MandatoryValidator_Validate_Error()
        {
            // arrange
            WorkItemManager manager = BuildManager();

            var properties = new Property[] {
                new Property("A", "String", ""),
                new Property("B", "String", "bb"),
            };

            // act
            var result = await manager.CreateAsync("FOO", "BAR", properties);

            // assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.NotNull(result.CreatedWorkItem);
            Assert.Collection(result.Errors,
                em =>
                {
                    Assert.Equal(nameof(MandatoryValidator), em.Source);
                    Assert.Equal(string.Empty, em.ErrorCode);
                    Assert.Equal("FOO", em.ProjectCode);
                    Assert.Equal("1", em.Id);
                    Assert.Equal("A", em.Property);
                }
            );
        }

        private static WorkItemManager BuildManager()
        {
            return new WorkItemManager(new InMemoryDataProvider(), new InMemoryDescriptorProvider(
                new WorkItemDescriptor("BAR", new LogDescriptor(true, Array.Empty<LogEntryTypeDescriptor>()), new PropertyDescriptor[] {
                    new PropertyDescriptor("A", "String", PropertyType.UserInput, true, true, new ValidatorDescriptor[] {
                        new MandatoryValidatorDescriptor(),
                    }, null),
                    new PropertyDescriptor("B", "String", PropertyType.UserInput, true, true, new ValidatorDescriptor[] {
                    }, null),
                }, Array.Empty<StageDescriptor>())
            ));
        }
    }
}