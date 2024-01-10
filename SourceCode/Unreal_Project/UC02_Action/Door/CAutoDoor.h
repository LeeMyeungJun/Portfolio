#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "CAutoDoor.generated.h"

DECLARE_MULTICAST_DELEGATE_OneParam(FAutoDoorOpen, class ACPlayer*);
DECLARE_MULTICAST_DELEGATE(FAutoDoorClose);

UCLASS()
class UC02_ACTION_API ACAutoDoor : public AActor
{
	GENERATED_BODY()

private:
	UPROPERTY(EditAnywhere, Category = "Door")
		float Speed = 2.0f;

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
	FAutoDoorOpen OnAutoDoorOpen;
	FAutoDoorClose OnAutoDoorClose;

private:
	UFUNCTION()
		void OnBeginOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);

	UFUNCTION()
		void OnEndOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex);
	
	UFUNCTION()
		void Open(class ACPlayer* InPlayer);

	UFUNCTION()
		void Close();

public:	
	ACAutoDoor();

protected:
	virtual void BeginPlay() override;

public:	
	virtual void Tick(float DeltaTime) override;

private:
	float Rotation;
	bool bOpen = false;
};
