namespace Birthday.Bot.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class HandlerAttribute(string name) : Attribute
{
	public readonly string Name = name;
}