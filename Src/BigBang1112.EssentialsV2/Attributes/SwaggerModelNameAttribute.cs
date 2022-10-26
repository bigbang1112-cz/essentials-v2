namespace BigBang1112.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class SwaggerModelNameAttribute : Attribute
{
    public string Name { get; }

    public SwaggerModelNameAttribute(string name)
    {
        Name = name;
    }
}