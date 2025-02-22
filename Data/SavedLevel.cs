using System.Collections.Generic;

[System.Serializable]
public class SavedLevel
{
    public List<SavedContainer> containers;

    public SavedLevel(List<SavedContainer> containers)
    {
        this.containers = containers;
    }

}
