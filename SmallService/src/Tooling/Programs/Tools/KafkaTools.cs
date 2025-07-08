using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tooling
{

    /// <summary>
    /// Generate JSON schema used for Kafka topics, JSON Schema is prefered over AVRO due to ease of integration and management of format.
    /// </summary>
    public static class KafkaTools
    {
        public static void GenerateJsonSchema<TObjectToTransposeToJsonSchmea>()
        {
            try
            {
                var parentObj = typeof(TObjectToTransposeToJsonSchmea);

                var parentObjName = parentObj.Name;

                Console.WriteLine($"Converted object: {parentObjName} into JSON Format");

                var schema = NJsonSchema.JsonSchema.FromType(parentObj);

                // Convert the schema to JSON string
                var json = schema.ToJson();

                string workingDirectory = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
                var outputFolder = $"{projectDirectory}\\Generated\\{parentObjName}";

                Directory.CreateDirectory(outputFolder);

                // Write the JSON schema to a file
                File.WriteAllText($@"{outputFolder}\{parentObjName}.json", json);

  
                Console.WriteLine($"See output JSON schema in: {outputFolder}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to convert to JSON schema: {ex.Message}");
            } 
        }
    }
}
