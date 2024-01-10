#pragma once

#include "CoreMinimal.h"
#include "Actions/CDoAction.h"
#include "CDoAction_FireStorm.generated.h"

UCLASS()
class UC02_ACTION_API ACDoAction_FireStorm : public ACDoAction
{
	GENERATED_BODY()

private:
	UPROPERTY(EditDefaultsOnly)
		float HittedTime = 0.1f;

	UPROPERTY(EditDefaultsOnly)
		float DestroyTime = 5.0f;

	UPROPERTY(EditDefaultsOnly)
		float Speed = 300.0f;

	UPROPERTY(EditDefaultsOnly)
		float Distance = 150.0f;

	UPROPERTY(VisibleDefaultsOnly)
		class UParticleSystemComponent* Attached;

protected:
	void BeginPlay() override;

protected:
	virtual void DoAction() override;
	virtual void Begin_DoAction() override;
	virtual void End_DoAction() override;
	virtual void Abort() override;

	virtual void Tick(float DeltaTime) override;

private:
	UFUNCTION()
		void Hitted();

	UFUNCTION()
		void Finish();

private:
	virtual void OnAttachmentBeginOverlap(class ACharacter* InAttacker, class AActor* InAttackCauser, class ACharacter* InOtherCharacter) override;
	virtual void OnAttachmentEndOverlap(class ACharacter* InAttacker, class AActor* InAttackCauser, class ACharacter* InOtherCharacter) override;

private:
	class UBoxComponent* Box;
	bool bOnce;

	TArray<class ACharacter*> HittedCharacters;
	float Angle;
};
