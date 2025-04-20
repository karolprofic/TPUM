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
            saveCSharpFile(candidateSchema, "../../../../Commons/CandidateDTOGenerated.cs");
        }

        private static void saveCSharpFile(JsonSchema schema, string filename)
        {
            CSharpGenerator generator = new CSharpGenerator(schema, new CSharpGeneratorSettings()
            {
                Namespace = "GeneratedClasses",
                ClassStyle = CSharpClassStyle.Poco,
            });

            string code = generator.GenerateFile();

            File.WriteAllText(filename, code);
        }
    }
}