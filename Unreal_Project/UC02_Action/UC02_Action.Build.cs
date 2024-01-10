using UnrealBuildTool;

public class UC02_Action : ModuleRules
{
	public UC02_Action(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;
	
		PublicDependencyModuleNames.AddRange(new string[] 
		{
			"Core",
			"CoreUObject",
			"Engine",
			"InputCore",
			"AIModule",
			"GameplayTasks",
			"ProceduralMeshComponent",
			"RenderCore",
			"Example"
		});

		PublicIncludePaths.Add(ModuleDirectory);
	}
}
