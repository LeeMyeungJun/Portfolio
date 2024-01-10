#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "CVertexActor.generated.h"

UCLASS()
class UC02_ACTION_API ACVertexActor : public AActor
{
	GENERATED_BODY()

private:
	UPROPERTY(VisibleDefaultsOnly)
		class UProceduralMeshComponent* Mesh;

	UPROPERTY(EditAnywhere)
		float Speed = 20.0;
	
public:	
	ACVertexActor();

protected:
	virtual void BeginPlay() override;

public:	
	virtual void Tick(float DeltaTime) override;

private:
	void AddRentangle(int32 InStart);

private:
	TArray<FVector> Vertices;
	TArray<int32> Indices;

};
