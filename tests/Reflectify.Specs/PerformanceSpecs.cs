using System.Linq;
using Xunit;

namespace Reflectify.Specs;

public class PerformanceSpecs
{
    [Fact]
    public void Fact()
    {
        // Arrange
        var objects = Enumerable.Range(1, 1000).Select(Create).ToArray();

        // Act
        for (int i = 0; i < 100000; i++)
        {
            foreach (object o in objects)
            {
                foreach (var p in o.GetType().GetProperties(MemberKind.Public))
                {
                    var nestedValue = p.GetValue(o);
                    nestedValue!.GetType().GetProperty("SomeUnknownProperty");
                }
            }
        }

        // Assert
    }

    private object Create(int index) => new
    {
        Value = index,
        Name = $"Item-{index}-{new string((char)('A' + (index % 26)), 50)}",
        Description = $"Description for item number {index} with extra padding {new string('x', 100)}",
        Category = $"Category-{index % 10}",
        Nested = new
        {
            Id = index * 100,
            Label = $"Nested-{index}-{new string('z', 50)}",
            SubNested = new { Code = index * 1000, Data = $"Data-{index}-{new string('y', 80)}" }
        },
        Meta = new
        {
            K1 = index,
            K2 = index * 2,
            K3 = index * 3,
            K4 = index * 4,
            K5 = index * 5
        }
    };
}
