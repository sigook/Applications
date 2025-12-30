namespace Covenant.Common.Models;

public class SkillModel : ISkillModel
{
    public SkillModel()
    {
    }

    public SkillModel(string skill) => Skill = skill;

    public Guid? Id { get; set; }
    public string Skill { get; set; }
}
