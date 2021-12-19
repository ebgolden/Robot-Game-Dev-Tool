using TinyJson;

public class DevPartManager
{
    public PartData getPartDataFromJSON(string partJSON)
    {
        string partString = partJSON.Substring(1, partJSON.Length - 2);
        partString = partString.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
        if (partString[0] == '[')
            partString = partString.Substring(1);
        if (partString[partString.Length - 1] == ']')
            partString = partString.Substring(0, partString.Length - 1);
        partString = "{" + partString;
        return partString.FromJson<PartData>();
    }
}