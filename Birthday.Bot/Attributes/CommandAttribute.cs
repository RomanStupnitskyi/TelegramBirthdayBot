namespace Birthday.Bot.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class CommandAttribute(
	string name,
	string description,
	string[]? aliases = null,
	bool isHidden = false,
	bool isOwnerOnly = false,
	bool isAdminOnly = false,
	bool allowGroup = true,
	bool allowPrivate = true,
	bool allowChannel = true
	) : Attribute
{
	public readonly string Name = name;
	public readonly string Description = description;
	public readonly string[] Aliases = aliases ?? [];
	public readonly bool IsHidden = isHidden;
	public readonly bool IsOwnerOnly = isOwnerOnly;
	public readonly bool IsAdminOnly = isAdminOnly;
	public readonly bool AllowGroup = allowGroup;
	public readonly bool AllowPrivate = allowPrivate;
	public readonly bool AllowChannel = allowChannel;
}