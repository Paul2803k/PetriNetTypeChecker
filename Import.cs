using System;
using NeTypeChecker.Net;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace NeTypeChecker
{
    class Import
    {
        // attachers
        static string AttachInterfaces(List<String> interfaces) {
            string output = "";
            int nInt = interfaces.Count;
            if (nInt != 0)
            {
                for (int i = 0; i < nInt; i++)
                {
                    var el = interfaces[i];

                    if (i == 0)
                        output += ": " + el;
                    else
                        output += ", " + el;
                }
            }
            return output;
        }

        static string AttachFuncInputs(Petri net, Transition t)
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

        static string AttachInputTypes(List<Inputs> inputs)
        {
            int nInp = inputs.Count;

            string output = "(";
            for (int i = 0; i < nInp; i++)
            {
                var el = inputs[i];

                if (i == nInp - 1)
                    output += el.Type + " " + el.Name;
                else
                    output += el.Type + " " + el.Name + ", ";

            }
            return output + ")";
        }

        static string AttachOutputTypes(List<Outputs> outputs)
        {
            int nOut = outputs.Count;

            if (nOut > 1)
            {
                string output = "Tuple<";
                for (int i = 0; i < nOut; i++)
                {
                    var el = outputs[i];

                    if (i == nOut - 1)
                        output += el.Type;
                    else
                        output += el.Type + ", ";

                }
                return output + ">";
            }
            else
            {
                return outputs[0].Type;
            }
        }

        static string AttachGenTypes(Dictionary<string, string> genTypes)
        {
            List<String> gentTypes = new List<String>();

            foreach (var el in genTypes)
                gentTypes.Add(el.Key);

            int nGen = gentTypes.Count;

            string output = "<";

            for (int i = 0; i < nGen; i++)
            {
                var el = gentTypes[i];

                if (i == nGen - 1)
                    output += el;
                else
                    output += el + ", ";

            }
            return output + ">";

        }

        static string AttachGenDependencies(Dictionary<string, string> genTypes)
        {
            string output = "";
            foreach (var el in genTypes)
            {
                output += "where " + el.Key + " : " + el.Value;
            }
            return output;
        }

        
        // variables declarations
        static void DeclareClasses(Petri net, StreamWriter sw) {
            foreach (var t in net.Net_Types)
            {
                sw.WriteLine("    public " + t.Type + " " + t.Name + " " + AttachInterfaces(t.Interfaces) + " {");
                foreach (var d in t.Declatations)
                {
                    sw.WriteLine("        public " + d.Type + " " + d.Names + " {get;} ");
                }
                foreach (var o in t.Originals) {
                    sw.WriteLine();
                    sw.WriteLine("        public static implicit operator " + o.Name + " (" + t.Name + " o) => throw new Exception();");
                }

                sw.WriteLine("    }");
            }
        }

        static void DeclareFunctOutputs(Petri net, Transition t, StreamWriter sw)
        {
            var list = net.Arcs.FindAll(el => el.From.Contains(t.Name));
            int nOut = list.Count;
            if (nOut > 1)
            {
                int count = 1;
                foreach (var a in list)
                {
                    sw.WriteLine("            " + a.To + " = " + t.Name + "_res.Item" + count + ";");//if the output arcs correspond to the outputs order
                    count++;
                }
            }
            else
                sw.WriteLine("            " + list[0].To + " = " + t.Name + "_res;");
        }

        
        // helper functions
        static string GetInpType(string name, Transition t) {
            return t.Inputs.Find(el => el.Name == name).Type;
        } 

        static bool IsInterface(string tInterface, Petri net) {
            return (net.Net_Types.Find(el => el.Name == tInterface && el.Type == "interface") != null) ;
        }

        
        // function declarations
        static void SimpleFunction(Transition t, StreamWriter sw)
        {
            sw.WriteLine("        " + AttachOutputTypes(t.Outputs) + " " + t.Name + " " + AttachInputTypes(t.Inputs) + " {");
            sw.WriteLine("            " + "throw new Exception();");
            sw.WriteLine("        }");
        }

        static void GenericFunction(Transition t, StreamWriter sw, bool genInp, bool genOut, Petri net)
        {

            Dictionary<string, string> genericTypes = new Dictionary<string, string>();

            foreach (var el in t.Inputs)
            {
                if (IsInterface(el.Type, net))
                {
                    genericTypes.Add(t.Name + "_genTypeIn_" + t.Inputs.IndexOf(el), el.Type);
                    el.Type = t.Name + "_genTypeIn_" + t.Inputs.IndexOf(el);
                }
                else if (el.Type == "Generic")
                {
                    genericTypes.Add(t.Name + "_genTypeIn_" + t.Inputs.IndexOf(el), "class");
                    el.Type = t.Name + "_genTypeIn_" + t.Inputs.IndexOf(el);
                }
            }
            foreach (var el in t.Outputs)
            {
                if (IsInterface(el.Type, net))
                {
                    genericTypes.Add(t.Name + "_genTypeIn_" + t.Outputs.IndexOf(el), el.Type);
                    el.Type = t.Name + "_genTypeOut_" + t.Outputs.IndexOf(el);
                }
                else if (el.Type == "Generic")
                {
                    genericTypes.Add(t.Name + "_genTypeIn_" + t.Outputs.IndexOf(el), "class");
                    el.Type = t.Name + "_genTypeOut_" + t.Outputs.IndexOf(el);
                }
            }

            t.Outputs.FindAll(el => el.Type.Contains("typeof")).ForEach(el => el.Type = GetInpType(el.Type.Substring(el.Type.IndexOf("(") + 1, el.Type.LastIndexOf(")") - el.Type.IndexOf("(") - 1), t));

            if (genInp || genOut)
                sw.WriteLine("        " + AttachOutputTypes(t.Outputs) + " " + t.Name + " " + AttachGenTypes(genericTypes) + " " + AttachInputTypes(t.Inputs) + " " + AttachGenDependencies(genericTypes) + " {");
            else
                sw.WriteLine("        " + AttachOutputTypes(t.Outputs) + " " + t.Name + " " + AttachInputTypes(t.Inputs) + " {");

            sw.WriteLine("            " + "throw new Exception();");
            sw.WriteLine("        }");
        }

        static void MakeFunction (Transition t, StreamWriter sw, Petri net)
        {
            var checkGenInp = (t.Inputs.Find(el => el.Type == "Generic" || IsInterface(el.Type, net)) != null);
            var checkGenOut = (t.Outputs.Find(el => el.Type == "Generic" || IsInterface(el.Type, net)) != null);
            var checkTypeOfOut = (t.Outputs.Find(el => el.Type.Contains("typeof")) != null);

            if (!checkGenInp && !checkGenOut && !checkTypeOfOut)
                SimpleFunction(t, sw);
            else 
                GenericFunction(t, sw, checkGenInp, checkGenOut, net);
        }

        
        // type derivation
        static void InferInterfaces(Petri net)
        {
            foreach (var place in net.Places)
            {
                var placeType = net.Json_Types.Find(el => el.Name == place.Type);
                var outArcs = net.Arcs.FindAll(el => el.From == place.Name);
                var inArcs = net.Arcs.FindAll(el => el.To == place.Name);

                foreach (var a in outArcs)
                {

                    string tName = a.To.Split(".")[0];
                    string tInput = a.To.Split(".")[1];

                    var transition = net.Transitions.Find(el => (el.Name == tName));
                    List<string> interfaces = new List<string>();
                    var inputsType = transition.Inputs.Find(el => el.Name == tInput && el.Type != place.Type && el.Type != "Generic");

                    if (inputsType != null)
                        interfaces.Add(inputsType.Type);

                    var exists = net.Net_Types.Find(el => el.Name == place.Type);
                    exists.Interfaces.AddRange(interfaces);
                }
            }

        }

       
        //main file structure
        static void PrepareNet(Petri net)
        {
            List<NetTypes> netTypes = new List<NetTypes>();
            net.Net_Types = netTypes;
            

            foreach (var t in net.Json_Types)
            {
                List<string> interfaces = new List<string>();
                net.Net_Types.Add(new NetTypes(t.Name, t.Type, interfaces, t.Declatations, new List<JsonTypes>()));
            }
        }

        static void MakeNet(Petri net, StreamWriter sw)
        {
            sw.WriteLine("    public class Net {");
            sw.WriteLine("        public void Check() {");

            // variable declarations
            foreach (var p in net.Places)
                sw.WriteLine("            " + p.Type + " " + p.Name + " = null;");

            //functions use
            foreach (var t in net.Transitions)
            {
                sw.WriteLine("            var " + t.Name + "_res = " + t.Name + " " + AttachFuncInputs(net, t) + ";");
                DeclareFunctOutputs(net, t, sw);
            }
            sw.WriteLine("        }");

            //functions declarations
            foreach (var t in net.Transitions)
            {
                MakeFunction(t, sw, net);
            }
            sw.WriteLine("    }");
        }

        static void MakeNewType(Petri net, List<string> names) {

            string name = "";
            string type = "class";
            List<string> interfaces = new List<string>();
            List<Declaration> declatations = new List<Declaration>();
            List<JsonTypes> originals = new List<JsonTypes>();

            foreach (var el in names) {
                
                name += el;

                interfaces.AddRange(net.Net_Types.Find(e => e.Name == el).Interfaces);
                interfaces = interfaces.Distinct().ToList();
                declatations.AddRange(net.Net_Types.Find(e => e.Name == el).Declatations);
                declatations = declatations.Distinct().ToList();
                originals.Add(net.Json_Types.Find(e => e.Name == el));
            }

            net.Net_Types.Add(new NetTypes(name, type, interfaces, declatations,originals));


        }
        
        static void BuildTypes(Petri net) {

            List<string> toBuild = new List<string>();

            foreach (var t in net.Transitions)
            {
                foreach (var outs in t.Outputs)
                {
                    if (outs.Type.Contains("+"))
                    {
                        toBuild.Add(outs.Type);
                        outs.Type = String.Concat(outs.Type.Split("+"));
                        
                    }
                }
            }

            foreach (var el in toBuild) {
                MakeNewType(net , new List<string>(el.Split("+")));
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
            BuildTypes(net);

            DeclareClasses(net, sw);
            MakeNet(net, sw);
            
            sw.WriteLine("}");
            sw.Close();

            Console.WriteLine(JSONresult);
            Console.ReadKey();
        }
    }
}
