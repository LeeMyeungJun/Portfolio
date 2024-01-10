#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "CFileRW.generated.h"

USTRUCT(BlueprintType)
struct FSaveData
{
	GENERATED_BODY()

public:
	UPROPERTY(EditAnywhere)
		FName ID;

	UPROPERTY(EditAnywhere)
		float Health;

	friend FArchive& operator<<(FArchive& InArchive, FSaveData& InData)
	{
		return InArchive << InData.ID << InData.Health;
	}
};

UCLASS()
class UC02_ACTION_API ACFileRW : public AActor
{
	GENERATED_BODY()
	
public:	
	ACFileRW();

protected:
	virtual void BeginPlay() override;

private:
	void OnSave();
	void OnLoad();
};
