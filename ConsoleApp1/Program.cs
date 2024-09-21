using System;
using System.IO;
using System.Text;
using YamlDotNet.RepresentationModel;

class Program
{
	static void Main(string[] args)
	{
		if (args.Length == 0 || !Directory.Exists(args[0]))
		{
			Console.WriteLine("Please drag a folder onto the program.");
			return;
		}

		string directoryPath = args[0];
		string copiedConfigPath = null;
		YamlMappingNode copiedConfig = null;

		if (string.IsNullOrEmpty(directoryPath))
		{
			Console.WriteLine("Could not determine the directory.");
			return;
		}

		Console.WriteLine("Input filepath to copy from another config.yaml (Leave blank to skip): ");
		copiedConfigPath = Console.ReadLine();

		if (!string.IsNullOrEmpty(copiedConfigPath) && File.Exists(copiedConfigPath))
		{
			copiedConfig = LoadYamlFile(copiedConfigPath);
		}

		Console.Write("Enter Outfit Name: ");
		string outfitName = Console.ReadLine();

		ConfigHandler configHandler = new ConfigHandler(directoryPath, copiedConfig);

		// Prompt for Mesh Paths
		Console.WriteLine("Input Base Mesh (Leave Blank if you're using base-mesh.uasset, enter filepath, or use shorthand asset:CHARACTER|TYPE|ID): ");
		string baseMesh = Console.ReadLine();

		Console.WriteLine("Input Costume Mesh (Leave Blank if you're using costume-mesh.uasset, enter filepath, or use shorthand asset:CHARACTER|TYPE|ID): ");
		string costumeMesh = Console.ReadLine();

		Console.WriteLine("Input Hair Mesh (Leave Blank if you're using hair-mesh.uasset, enter filepath, or use shorthand asset:CHARACTER|TYPE|ID): ");
		string hairMesh = Console.ReadLine();

		Console.WriteLine("Input Face Mesh (Leave Blank if you're using face-mesh.uasset, enter filepath, or use shorthand asset:CHARACTER|TYPE|ID): ");
		string faceMesh = Console.ReadLine();

		// Prompt for allout paths
		Console.WriteLine("Input Allout Normal Path (Leave Blank if you're using allout-normal.uasset, otherwise enter path for it): ");
		string normalPath = Console.ReadLine();

		Console.WriteLine("Input Allout Normal Mask Path (Leave Blank if you're using allout-normal-mask.uasset, otherwise enter path for it): ");
		string normalMaskPath = Console.ReadLine();

		Console.WriteLine("Input Allout Special Path (Leave Blank if you're using allout-special.uasset, otherwise enter path for it): ");
		string specialPath = Console.ReadLine();

		Console.WriteLine("Input Allout Special Mask Path (Leave Blank if you're using allout-special-mask.uasset, otherwise enter path for it): ");
		string specialMaskPath = Console.ReadLine();

		Console.WriteLine("Input Allout Text Path (Leave Blank if you're using allout-text.uasset, otherwise enter path for it): ");
		string textPath = Console.ReadLine();

		Console.WriteLine("Input Allout PLG Path (Leave Blank if you're using allout-plg.uasset, otherwise enter path for it): ");
		string plgPath = Console.ReadLine();

		configHandler.CreateConfig(outfitName, baseMesh, costumeMesh, hairMesh, faceMesh, normalPath, normalMaskPath, specialPath, specialMaskPath, textPath, plgPath);

		Console.Write("Enter Outfit Description: ");
		string outfitDescription = Console.ReadLine();

		DescriptionHandler descriptionHandler = new DescriptionHandler(directoryPath);
		descriptionHandler.CreateDescription(outfitDescription);
	}

	static YamlMappingNode LoadYamlFile(string filepath)
	{
		using (var reader = new StreamReader(filepath))
		{
			var yaml = new YamlStream();
			yaml.Load(reader);

			var root = (YamlMappingNode)yaml.Documents[0].RootNode;
			return root;
		}
	}
}

class ConfigHandler
{
	private string _directoryPath;
	private YamlMappingNode _copiedConfig;

	public ConfigHandler(string directoryPath, YamlMappingNode copiedConfig = null)
	{
		_directoryPath = directoryPath;
		_copiedConfig = copiedConfig;
	}

	public void CreateConfig(string outfitName, string baseMesh, string costumeMesh, string hairMesh, string faceMesh, string normalPath, string normalMaskPath, string specialPath, string specialMaskPath, string textPath, string plgPath)
	{
		string configFilePath = Path.Combine(_directoryPath, "config.yaml");
		StringBuilder configContent = new StringBuilder();

		configContent.AppendLine($"name: {outfitName}\n");

		string GetCopiedValue(string key, string subKey)
		{
			if (_copiedConfig != null && _copiedConfig.Children.ContainsKey(new YamlScalarNode(key)))
			{
				var section = (YamlMappingNode)_copiedConfig.Children[new YamlScalarNode(key)];
				if (section.Children.ContainsKey(new YamlScalarNode(subKey)))
				{
					return section.Children[new YamlScalarNode(subKey)].ToString();
				}
			}
			return null;
		}

		baseMesh = string.IsNullOrEmpty(baseMesh) ? GetCopiedValue("base", "mesh_path") : baseMesh;
		if (!string.IsNullOrEmpty(baseMesh)) configContent.AppendLine($"base:\n  mesh_path: {baseMesh}\n");

		costumeMesh = string.IsNullOrEmpty(costumeMesh) ? GetCopiedValue("costume", "mesh_path") : costumeMesh;
		if (!string.IsNullOrEmpty(costumeMesh)) configContent.AppendLine($"costume:\n  mesh_path: {costumeMesh}\n");

		hairMesh = string.IsNullOrEmpty(hairMesh) ? GetCopiedValue("hair", "mesh_path") : hairMesh;
		if (!string.IsNullOrEmpty(hairMesh)) configContent.AppendLine($"hair:\n  mesh_path: {hairMesh}\n");

		faceMesh = string.IsNullOrEmpty(faceMesh) ? GetCopiedValue("face", "mesh_path") : faceMesh;
		if (!string.IsNullOrEmpty(faceMesh)) configContent.AppendLine($"face:\n  mesh_path: {faceMesh}\n");

		normalPath = string.IsNullOrEmpty(normalPath) ? GetCopiedValue("allout", "normal_path") : normalPath;
		normalMaskPath = string.IsNullOrEmpty(normalMaskPath) ? GetCopiedValue("allout", "normal_mask_path") : normalMaskPath;
		specialPath = string.IsNullOrEmpty(specialPath) ? GetCopiedValue("allout", "special_path") : specialPath;
		specialMaskPath = string.IsNullOrEmpty(specialMaskPath) ? GetCopiedValue("allout", "special_mask_path") : specialMaskPath;
		textPath = string.IsNullOrEmpty(textPath) ? GetCopiedValue("allout", "text_path") : textPath;
		plgPath = string.IsNullOrEmpty(plgPath) ? GetCopiedValue("allout", "plg_path") : plgPath;

		if (!string.IsNullOrEmpty(normalPath) || !string.IsNullOrEmpty(normalMaskPath) || !string.IsNullOrEmpty(specialPath) || !string.IsNullOrEmpty(specialMaskPath) || !string.IsNullOrEmpty(textPath) || !string.IsNullOrEmpty(plgPath))
		{
			configContent.AppendLine("allout:");
			if (!string.IsNullOrEmpty(normalPath)) configContent.AppendLine($"  normal_path: {normalPath}");
			if (!string.IsNullOrEmpty(normalMaskPath)) configContent.AppendLine($"  normal_mask_path: {normalMaskPath}");
			if (!string.IsNullOrEmpty(specialPath)) configContent.AppendLine($"  special_path: {specialPath}");
			if (!string.IsNullOrEmpty(specialMaskPath)) configContent.AppendLine($"  special_mask_path: {specialMaskPath}");
			if (!string.IsNullOrEmpty(textPath)) configContent.AppendLine($"  text_path: {textPath}");
			if (!string.IsNullOrEmpty(plgPath)) configContent.AppendLine($"  plg_path: {plgPath}");
		}

		File.WriteAllText(configFilePath, configContent.ToString());
		Console.WriteLine($"config.yaml has been created in {_directoryPath}.");
	}
}

class DescriptionHandler
{
	private string _directoryPath;

	public DescriptionHandler(string directoryPath)
	{
		_directoryPath = directoryPath;
	}

	public void CreateDescription(string outfitDescription)
	{
		string descriptionFilePath = Path.Combine(_directoryPath, "description.msg");
		string descriptionContent = $"[uf 0 5 65278][uf 2 1]{outfitDescription} [n][e]";
		File.WriteAllText(descriptionFilePath, descriptionContent);
		Console.WriteLine($"description.msg has been created in {_directoryPath}.");
	}
}
