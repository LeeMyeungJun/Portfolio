#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "CCameraSpline.generated.h"

UCLASS()
class UC02_ACTION_API ACCameraSpline : public AActor
{
	GENERATED_BODY()

private:
	UPROPERTY(VisibleAnywhere)
		class UCurveFloat* CurveFloat;

	UPROPERTY(VisibleAnywhere)
		class USplineComponent* Spline;


public:
	FORCEINLINE class UCurveFloat* GetCurveFloat() { return CurveFloat; }
	FORCEINLINE class USplineComponent* GetSplineComponent() { return Spline; }
	
public:	
	ACCameraSpline();



};
