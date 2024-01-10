#include "CLodActor.h"
#include "Global.h"
#include "Components/StaticMeshComponent.h"
#include "Materials/MaterialInstanceConstant.h"

ACLodActor::ACLodActor()
{
	CHelpers::CreateComponent<UStaticMeshComponent>(this, &Mesh, "Mesh");

	UStaticMesh* mesh;
	CHelpers::GetAsset<UStaticMesh>(&mesh, "StaticMesh'/Game/StaticMeshes/Cube.Cube'");
	Mesh->SetStaticMesh(mesh);

	UMaterialInstanceConstant* material;
	CHelpers::GetAsset<UMaterialInstanceConstant>(&material, "MaterialInstanceConstant'/Game/Materials/Mi_VertexColor.Mi_VertexColor'");
	Mesh->SetMaterial(0, material);
}

void ACLodActor::BeginPlay()
{
	Super::BeginPlay();

	UKismetSystemLibrary::K2_SetTimer(this, "PaintVertices", 0.3f, true);
	
}

void ACLodActor::PaintVertices()
{
	Mesh->SetLODDataCount(1, Mesh->LODData.Num());

	UStaticMesh* mesh = Mesh->GetStaticMesh();

	FStaticMeshComponentLODInfo* lodInfo =  &Mesh->LODData[0];

	lodInfo->PaintedVertices.Empty();
	lodInfo->OverrideVertexColors = new FColorVertexBuffer();

	FStaticMeshLODResources* resource = &mesh->RenderData->LODResources[0];

	TArray<FColor> colors;
	colors.Reserve(resource->GetNumVertices() - 1);

	//FColor color = FColor::MakeRandomColor();
	for (int32 i = 0; i < resource->GetNumVertices(); i++)
		colors.Add(FColor::MakeRandomColor());

	lodInfo->OverrideVertexColors->InitFromColorArray(colors);

	BeginInitResource(lodInfo->OverrideVertexColors);
	Mesh->MarkRenderStateDirty();
}


