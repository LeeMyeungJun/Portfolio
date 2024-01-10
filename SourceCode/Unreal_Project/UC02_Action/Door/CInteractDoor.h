#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "CInteractDoor.generated.h"

UCLASS()
class UC02_ACTION_API ACInteractDoor : public AActor
{
	GENERATED_BODY()

private:
	UPROPERTY(EditAnywhere, Category = "Door")
		float Speed = 80.0f;

	UPROPERTY(EditAnywhere, Category = "Door")
		float MaxDegree = 90.0f;

	UPROPERTY(EditAnywhere, Category = "Door")
		bool bHiddenInGame;

private:
	UPROPERTY(VisibleDefaultsOnly)
		class USceneComponent* Scene;

	UPROPERTY(VisibleDefaultsOnly)
		class UBoxComponent* Box;

	UPROPERTY(VisibleDefaultsOnly)
		class UStaticMeshComponent* DoorFrame;

	UPROPERTY(VisibleDefaultsOnly)
		class UStaticMeshComponent* Door;
	
public:	
	ACInteractDoor();

protected:
	virtual void BeginPlay() override;

private:
	void Openning(float Delta);
	void Closing(float Delta);

public:	
	virtual void Tick(float DeltaTime) override;

	void Interact(const FVector& InForward);

private:
	bool bClosed = true;

	bool bOpening = false;
	bool bClosing = false;

	float Direction;
	float Degree;

	float AddRotation;
};
