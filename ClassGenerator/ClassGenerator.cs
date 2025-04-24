using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using Commons;

namespace ClassGenerator
{

    class ClassGeneratorJson
    {
        private static void Main(string[] args)
        {
            JsonSchema candidateSchema = JsonSchema.FromType<CandidateDTO>();
            candidateSchema.Title = candidateSchema.Title + "Generated";
            SaveCSharpFile(candidateSchema, "../../../../Commons/CSharp/CandidateDTOGenerated.cs");
            SaveJsonFile(candidateSchema, "../../../../Commons/JSON/CandidateDTOGenerated.json");
        }

        private static void SaveCSharpFile(JsonSchema schema, string filename)
        {
            CSharpGenerator generator = new CSharpGenerator(schema, new CSharpGeneratorSettings()
            {
                Namespace = "GeneratedClasses",
                ClassStyle = CSharpClassStyle.Poco,
            });

            string code = generator.GenerateFile();

            File.WriteAllText(filename, code);
        }

        private static void SaveJsonFile(JsonSchema schema, string filename)
        {
            File.WriteAllText(filename, schema.ToJson());
        }

        
    }
}