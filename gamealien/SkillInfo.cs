public struct SkillInfo
{
    public string Name;
    public string CostText;
    public string DamageText;
    public string Description;

    public SkillInfo(string name, string damageText, string costText, string description)
    {
        Name = name;
        DamageText = damageText;
        CostText = costText;
        Description = description;
    }
}