#include "CVertexActor.h"
#include "Global.h"
#include "ProceduralMeshComponent.h"


ACVertexActor::ACVertexActor()
{
	PrimaryActorTick.bCanEverTick = true;

	CHelpers::CreateComponent<UProceduralMeshComponent>(this, &Mesh, "Mesh");

	float w = 50, h = 50, d = 50;

	//Front
	Vertices.Add(FVector(-d, -w, -h));
	Vertices.Add(FVector(-d, -w, +h));
	Vertices.Add(FVector(-d, +w, -h));
	Vertices.Add(FVector(-d, +w, +h));
	AddRentangle(0);

	//Back
	Vertices.Add(FVector(+d, -w, -h));
	Vertices.Add(FVector(+d, +w, -h));
	Vertices.Add(FVector(+d, -w, +h));
	Vertices.Add(FVector(+d, +w, +h));
	AddRentangle(4);

	//Top
	Vertices.Add(FVector(-d, -w, +h));
	Vertices.Add(FVector(+d, -w, +h));
	Vertices.Add(FVector(-d, +w, +h));
	Vertices.Add(FVector(+d, +w, +h));
	AddRentangle(8);

	//Bottom
	Vertices.Add(FVector(-d, -w, -h));
	Vertices.Add(FVector(-d, +w, -h));
	Vertices.Add(FVector(+d, -w, -h));
	Vertices.Add(FVector(+d, +w, -h));
	AddRentangle(12);

	//Left
	Vertices.Add(FVector(+d, -w, -h));
	Vertices.Add(FVector(+d, -w, +h));
	Vertices.Add(FVector(-d, -w, -h));
	Vertices.Add(FVector(-d, -w, +h));
	AddRentangle(16);

	//Right
	Vertices.Add(FVector(-d, +w, -h));
	Vertices.Add(FVector(-d, +w, +h));
	Vertices.Add(FVector(+d, +w, -h));
	Vertices.Add(FVector(+d, +w, +h));
	AddRentangle(20);

	Mesh->CreateMeshSection
	(
		0,
		Vertices,
		Indices,
		TArray<FVector>(),
		TArray<FVector2D>(),
		TArray<FColor>(),
		TArray<FProcMeshTangent>(),
		true
	);
}

void ACVertexActor::BeginPlay()
{
	Super::BeginPlay();
	
}

void ACVertexActor::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	for (int32 i = 0; i < Vertices.Num(); i++)
		Vertices[i] += FVector(0, 0, Speed * DeltaTime);

	Mesh->UpdateMeshSection
	(
		0,
		Vertices,
		TArray<FVector>(),
		TArray<FVector2D>(),
		TArray<FColor>(),
		TArray<FProcMeshTangent>()
	);
}

void ACVertexActor::AddRentangle(int32 InStart)
{
	//210 312
	Indices.Add(InStart + 2);
	Indices.Add(InStart + 1);
	Indices.Add(InStart + 0);

	Indices.Add(InStart + 3);
	Indices.Add(InStart + 1);
	Indices.Add(InStart + 2);
}

