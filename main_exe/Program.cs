using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace main_exe
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Loading pretrained data...");

            Global.ep_index_6 = File_Operation.read_hash("./data/ep_index_6.data");
            Global.ep_index_7 = File_Operation.read_hash("./data/ep_index_7.data");
            Global.ep_index_8 = File_Operation.read_hash("./data/ep_index_8.data");
            Global.ep_index_9 = File_Operation.read_hash("./data/ep_index_9.data");
            Global.ep_index_10 = File_Operation.read_hash("./data/ep_index_10.data");
            Global.ep_index_10 = File_Operation.read_hash("./data/ep_index_11.data");
            Global.ep_index_10 = File_Operation.read_hash("./data/ep_index_12.data");

            Console.WriteLine("Making new dictionary...");

            Dictionary<int, int> ep_dict = new Dictionary<int, int>();

            foreach(int each in Global.ep_index_6)
            {
                ep_dict.Add(each, 2);
            }

            foreach (int each in Global.ep_index_7)
            {
                if (!(ep_dict.ContainsKey(each)))
                {
                    ep_dict.Add(each, 3);
                }
            }

            foreach (int each in Global.ep_index_8)
            {
                if (!(ep_dict.ContainsKey(each)))
                {
                    ep_dict.Add(each, 3);
                }
            }

            foreach (int each in Global.ep_index_9)
            {
                if (!(ep_dict.ContainsKey(each)))
                {
                    ep_dict.Add(each, 4);
                }
            }

            foreach (int each in Global.ep_index_10)
            {
                if (!(ep_dict.ContainsKey(each)))
                {
                    ep_dict.Add(each, 4);
                }
            }

            foreach (int each in Global.ep_index_11)
            {
                if (!(ep_dict.ContainsKey(each)))
                {
                    ep_dict.Add(each, 5);
                }
            }

            foreach (int each in Global.ep_index_12)
            {
                if (!(ep_dict.ContainsKey(each)))
                {
                    ep_dict.Add(each, 5);
                }
            }

            using (FileStream fs = new FileStream("ep_dict.data", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, ep_dict);
            }

            Console.WriteLine("Finished!");

            Console.ReadKey();
        }
    }
}
