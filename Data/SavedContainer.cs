using System.Collections.Generic;
[System.Serializable]
public class SavedContainer 
{
    public List<SavedPlatform> savedPlatforms;

    public SavedContainer()
    {
        savedPlatforms = new List<SavedPlatform>(); 
    }
}
