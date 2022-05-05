using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightRewinder.Generators
{
    [Generator]
    public class PosStructGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var fields = FindingFieldNames.FindFieldNames(context, FindingFieldNames.PositionStructName);
            CreateSetStruct(context, fields);
            AddOperations(context, fields);
        }

        public void CreateSetStruct(GeneratorExecutionContext context, IEnumerable<(string type, string name)> fields)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(@"
using FlightRewinder.Classes;
using FlightRewinder.StructAttributes;
using Microsoft.FlightSimulator.SimConnect;
using System.Runtime.InteropServices;

namespace FlightRewinder.Structs
{
    public partial struct PositionSetStruct
    {");

            foreach ((string type, string name) in fields)
            {
                builder.Append($@" 
        public {type} {name};");
            }

            builder.Append(@" 
    }
}
");

            context.AddSource("GeneratedSetStruct", SourceText.From(builder.ToString(), Encoding.UTF8));
        }

        public void AddOperations(GeneratorExecutionContext context, IEnumerable<(string type, string name)> fields)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(@"
using FlightRewinder.Classes;
using System;
using FlightRewinder.Structs;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightRewinder.Structs
{
    public partial class PositionStructOperators
    {
        public static partial PositionSetStruct ToSet(PositionStruct values)
        => new PositionSetStruct() {
");
            foreach (var field in fields)
            {
                builder.Append($"\n{field.name} = values.{field.name},");
            }
            builder.Append(@" 
        };"
);
            builder.Append(@"
        public static PositionSetStruct Interpolate(PositionSetStruct oldPosition, PositionSetStruct newPosition, double interpolation)
        => new PositionSetStruct(){
");
            foreach (var field in fields)
            {
                switch (field.type)
                {
                    case "double":
                        builder.Append($"\n{field.name} = interpolation * newPosition.{field.name} + oldPosition.{field.name} * (1-interpolation),");
                        break;
                    case "int":
                        builder.Append($"\n{field.name} = (int) Math.Round(interpolation * newPosition.{field.name} + oldPosition.{field.name} * (1-interpolation)),");
                        break;
                    case "uint":
                        builder.Append($"\n{field.name} = (uint) Math.Round(interpolation * newPosition.{field.name} + oldPosition.{field.name} * (1-interpolation)),");
                        break;
                    default:
                        builder.Append($"\n{field.name} = oldPosition.{field.name}");
                        break;
                }
            }

            builder.Append("};");

            builder.Append(@"
        public static PositionSetStruct Scale(PositionSetStruct position, double factor)
        => new PositionSetStruct(){
");
            foreach (var field in fields)
            {
                switch (field.type)
                {
                    case "double":
                        builder.Append($"\n{field.name} = position.{field.name} * factor,");
                        break;
                    case "int":
                        builder.Append($"\n{field.name} = (int) (position.{field.name} * factor),");
                        break;
                    case "uint":
                        builder.Append($"\n{field.name} = (uint) (position.{field.name} * factor),");
                        break;
                    default:
                        builder.Append($"\n{field.name} = position.{field.name},");
                        break;
                }
            }

            builder.Append("};");

            builder.Append(@"
        public static PositionSetStruct Add(PositionSetStruct first, PositionSetStruct second)
            => new PositionSetStruct(){
");
            foreach (var field in fields)
            {
                switch (field.type)
                {
                    case "double":
                        builder.Append($"\n{field.name} = first.{field.name} + second.{field.name},");
                        break;
                    case "int":
                        builder.Append($"\n{field.name} = first.{field.name} + second.{field.name},");
                        break;
                    case "uint":
                        builder.Append($"\n{field.name} = first.{field.name} + second.{field.name},");
                        break;
                    default:

                        break;
                        
                }
            }
            builder.Append("\n};");


            builder.Append("\n}\n}");

            context.AddSource("Operators", builder.ToString());
        }
        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
