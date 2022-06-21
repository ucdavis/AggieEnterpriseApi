using StrawberryShake.Serialization;

namespace AggieEnterpriseApi.Serializers;

public class PositiveIntSerializer : ScalarSerializer<int>
{
    public PositiveIntSerializer()
        : base(
            // the name of the scalar
            "PositiveInt")
    {
    }
}

public class NonNegativeIntSerializer : ScalarSerializer<int>
{
    public NonNegativeIntSerializer()
        : base(
            // the name of the scalar
            "NonNegativeInt")
    {
    }
}

public class NonPositiveIntSerializer : ScalarSerializer<int>
{
    public NonPositiveIntSerializer()
        : base(
            // the name of the scalar
            "NonPositiveInt")
    {
    }
}

public class NonNegativeFloatSerializer : ScalarSerializer<decimal>
{
    public NonNegativeFloatSerializer()
        : base(
            // the name of the scalar
            "NonNegativeFloat")
    {
    }
}
