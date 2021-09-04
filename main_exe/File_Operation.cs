using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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

}
