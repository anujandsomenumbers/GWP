[System.Serializable]
public class Character : LevelEntity
{
    public string name; // Diplay name
    public string characterId; // Unique id for this character in particular

    public Character(string entityId, string name, string characterId) : base(entityId) 
    {
        this.name = name;
        this.characterId = characterId;
    }
}
