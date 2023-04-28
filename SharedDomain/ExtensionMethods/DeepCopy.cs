namespace SharedDomain.ExtensionMethods;

public static class DeepCopy
{
    public static T DeepCopyByXml<T>(this T self)
    {
        using (var ms = new MemoryStream())
        {
            XmlSerializer s = new XmlSerializer(typeof(T));
            s.Serialize(ms, self);
            ms.Position = 0;
            return (T)s.Deserialize(ms)!;
        }
    }

    public static T DeepCopyByJson<T>(this T self)
    {
        var json = JsonSerializer.Serialize(self);
        return JsonSerializer.Deserialize<T>(json)!;
    }
}

