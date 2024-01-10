#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "Components/TimelineComponent.h"
#include "CCameraActor.generated.h"

UCLASS()
class UC02_ACTION_API ACCameraActor : public AActor
{
	GENERATED_BODY()

private:
	UPROPERTY(VisibleDefaultsOnly)
		class UCameraComponent* Camera;

	UPROPERTY(EditAnywhere)
		float PlayRate = 0.25f;
	
public:	
	ACCameraActor();

public:
	UFUNCTION(BlueprintCallable)
		void StartTimeline();

private:
	UFUNCTION()
		void OnProgress(float Value);

	UFUNCTION(BlueprintCallable)
		void OnFinishProgress();

protected:
	virtual void BeginPlay() override;

public:	
	virtual void Tick(float DeltaTime) override;

private:
	FTimeline Timeline;
	class ACCameraSpline* Spline;

};
