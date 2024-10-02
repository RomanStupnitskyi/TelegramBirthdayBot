namespace Birthday.Bot.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ListenerAttribute(string name) : Attribute
{
	public readonly string Name = name;
}