using System;
using System.IO;

class Program
{
	static void Main(string[] args)
	{
		// Check for uasset
		if (args.Length == 0 || !args[0].EndsWith("costume-mesh.uasset"))
		{
			Console.WriteLine("Please drag a 'costume-mesh.uasset' file onto the program.");
			return;
		}

		// Grab filepath of the uasset
		string directoryPath = Path.GetDirectoryName(args[0]);

		// Check for nulla (best way to say 0 in spanish)
		if (string.IsNullOrEmpty(directoryPath))
		{
			Console.WriteLine("Could not determine the directory of the provided file.");
			return;
		}

		Console.Write("Enter Outfit Name: ");
		string outfitName = Console.ReadLine();
		string configFilePath = Path.Combine(directoryPath, "config.yaml");
		string configContent = $"name: {outfitName}";
		File.WriteAllText(configFilePath, configContent);
		Console.WriteLine($"config.yml has been created in {directoryPath}.");

		Console.Write("Enter Outfit Description: ");
		string outfitDescription = Console.ReadLine();
		string descriptionFilePath = Path.Combine(directoryPath, "description.msg");
		string descriptionContent = $"[uf 0 5 65278][uf 2 1]{outfitDescription} [n][e]";
		File.WriteAllText(descriptionFilePath, descriptionContent);
		Console.WriteLine($"description.msg has been created in {directoryPath}.");
	}
}
