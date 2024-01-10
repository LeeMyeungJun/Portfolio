#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "CButtonActor.generated.h"

UCLASS()
class UC02_ACTION_API ACButtonActor : public AActor
{
	GENERATED_BODY()

private:
	UPROPERTY(VisibleDefaultsOnly)
		class UStaticMeshComponent* Mesh;
	
public:	
	ACButtonActor();

	void ApplyColor(FLinearColor InColor);

protected:
	virtual void BeginPlay() override;

};
