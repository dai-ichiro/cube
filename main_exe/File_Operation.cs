using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class File_Operation
{
    public static int[,] read_array(string path)
    {
        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            BinaryFormatter bf = new BinaryFormatter();
            return (int[,])bf.Deserialize(fs);
        }
    }

    public static HashSet<int> read_hash(string path)
    {
        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            BinaryFormatter bf = new BinaryFormatter();
            return (HashSet<int>)bf.Deserialize(fs);
        }
    }

}
