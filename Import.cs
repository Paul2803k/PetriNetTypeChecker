using System;
using NeTypeChecker.Net;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

namespace NeTypeChecker
{
    class Import
    {
        static string AttachInt(List<String> interfaces) {
            string output = "";
            foreach (string s in interfaces)
                output += " : " + s;
            return output;
        }

        static void AddClasses(Petri net, StreamWriter sw) {
            foreach (var t in net.Net_Types)
            {
                sw.WriteLine("    public " + t.Type + " " + t.Name + " " + AttachInt(t.Interfaces) + " {");
                foreach (var d in t.Declatations)
                {
                    sw.WriteLine("        public " + d.Type + " " + d.Names + " {get;} ");
                }
                /*
                // attach interfaces: must be inferred from the net
                foreach (var i in t.Interfaces)
                {
                    var el = net.Json_Types.Find(x => x.Name == i);
                    foreach (var e in el.Declatations)
                        sw.WriteLine("        public " + e.Type + " " + e.Names + " {get;} ");
                }
                */
                sw.WriteLine("    }");
            }
        }

        static void InferInterfaces(Petri net)
        {
            foreach (var place in net.Places) {
                var placeType = net.Json_Types.Find(el => el.Name == place.Type);
                var outArcs = net.Arcs.FindAll(el => el.From == place.Name);
                var inArcs = net.Arcs.FindAll(el => el.To == place.Name);

                foreach (var a in outArcs) {

                    string tName = a.To.Split(".")[0];
                    string tInput = a.To.Split(".")[1];

                    var transition = net.Transitions.Find(el => (el.Name == tName));
                    List<string> interfaces = new List<string>();
                    var inputsType = transition.Inputs.Find(el => el.Name == tInput && el.Type != place.Type);
                    
                    if (inputsType != null)
                        interfaces.Add(inputsType.Type);
          
                    var exists = net.Net_Types.Find(el => el.Name == place.Type);
                    exists.Interfaces.AddRange(interfaces);                  
                }
            }
            
        }

        static void PrepareNet(Petri net)
        {
            List<NetTypes> netTypes = new List<NetTypes>();
            net.Net_Types = netTypes;

            foreach (var t in net.Json_Types)
            {
                List<string> interfaces = new List<string>();
                net.Net_Types.Add(new NetTypes(t.Name, t.Type, interfaces, t.Declatations));
            }
        }

        static void MakeNet(Petri net, StreamWriter sw) {
            sw.WriteLine("    public class Net {");
            sw.WriteLine("        public void Check() {");

            foreach (var p in net.Places)
                sw.WriteLine("            " + p.Type + " " + p.Name + " = null;");

            foreach (var t in net.Transitions)
            {
                sw.WriteLine("            var " + t.Name + "_res = " + t.Name + " " + AttachInput(net, t) + ";");
                DeclareOutput(net, t, sw);
            }
            sw.WriteLine("        }");

            // declare functions
            foreach (var t in net.Transitions)
            {
                MakeFunction(t, sw);
            }

            sw.WriteLine("    }");
        }

        static string AttachInput(Petri net, Transition t)
        {
            var list = net.Arcs.FindAll(el => el.To.Contains(t.Name));
            int nInp = list.Count;

            string output = "( ";
            for (int i = 0; i < nInp; i++)
            {
                var el = list[i];

                if (i == nInp - 1)
                    output += el.From;
                else
                    output += el.From + ", ";
            }
            return output + " )";
        }
        static void DeclareOutput(Petri net, Transition t, StreamWriter sw)
        {
            var list = net.Arcs.FindAll(el => el.From.Contains(t.Name));
            int nOut = list.Count;
            if (nOut > 1){
                foreach (var a in list) {
                    sw.WriteLine("            " + a.To + " = " + t.Name + "_res;");
                }
            }
            else
                sw.WriteLine("            " + list[0].To + " = " + t.Name + "_res;");
        }

        static string AttachInputTypes(List<Inputs> inputs)
        {
            int nInp = inputs.Count;

            string output = "( ";
            for (int i = 0; i < nInp; i++)
            {
                var el = inputs[i];

                if (i == nInp - 1)
                    output += el.Type + " " + el.Name ;
                else
                    output += el.Type + " " + el.Name + ", ";

            }
            return output + " )";
        }

        static string AttachOutputTypes(List<Outputs> outputs)
        {
            int nOut = outputs.Count;

            if (nOut > 1)
            {
                string output = "Tuple< ";
                for (int i = 0; i < nOut; i++)
                {
                    var el = outputs[i];

                    if (i == nOut - 1)
                        output += el.Type;
                    else
                        output += el.Type +", ";

                }
                return output + " >";
            }
            else {
                return outputs[0].Type;
            }
        }

        static void SimpleFunction(Transition t, StreamWriter sw)
        {
            sw.WriteLine("        " + AttachOutputTypes(t.Outputs) + " " + t.Name + " " + AttachInputTypes(t.Inputs) + " {");
            sw.WriteLine("            " + "throw new Exception();");
            sw.WriteLine("        }");
        }


        static void MakeFunction (Transition t, StreamWriter sw)
        {
            int nOutputs = t.Outputs.Count;
            //int nInputs = t.Inputs.Count;

            if (nOutputs == 1) {
                SimpleFunction(t, sw);
            }

        }


        static void Main(string[] args)
        {
            string path = @"C:\Users\scatt\Desktop\Paolo\Università\Year 3\1) First Quarter\Thesis\NetTypeChecker\ConsoleApp3\";
            var net = JsonConvert.DeserializeObject<Petri>(File.ReadAllText(path + "net.json"));
            string JSONresult = JsonConvert.SerializeObject(net, Formatting.Indented);
            
            StreamWriter sw = new StreamWriter(path + "output.cs");

            sw.WriteLine("using System;");
            sw.WriteLine("namespace PetriTypeCheck {");

            //jsonType to NetTypes
            PrepareNet(net);
            InferInterfaces(net);


            AddClasses(net, sw);
            MakeNet(net, sw);
            
            sw.WriteLine("}");
            sw.Close();

            Console.WriteLine(JSONresult);
            Console.ReadKey();
        }
    }
}
