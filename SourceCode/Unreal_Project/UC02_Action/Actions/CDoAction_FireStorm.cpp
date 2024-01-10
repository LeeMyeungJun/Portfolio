#include "CDoAction_FireStorm.h"
#include "Global.h"
#include "CAttachment.h"
#include "GameFramework/Character.h"
#include "Particles/ParticleSystemComponent.h"
#include "Components/BoxComponent.h"
#include "Components/CStateComponent.h"
#include "Components/CStatusComponent.h"
#include "Characters/CEnemy.h"

void ACDoAction_FireStorm::BeginPlay()
{
	Super::BeginPlay();

	for (AActor* actor : OwnerCharacter->Children)
	{
		if (actor->IsA<ACAttachment>() && actor->GetActorLabel().Contains("FireStorm"))
		{
			Box = CHelpers::GetComponent<UBoxComponent>(actor);
			break;
		}
	}
}

void ACDoAction_FireStorm::DoAction()
{
	CheckFalse(State->IsIdleMode());
	CheckTrue(bOnce);

	State->SetActionMode();
	bOnce = true;

	OwnerCharacter->PlayAnimMontage(Datas[0].AnimMontage, Datas[0].PlayRatio, Datas[0].StartSection);
	Datas[0].bCanMove ? Status->SetMove() : Status->SetStop();
}

void ACDoAction_FireStorm::Begin_DoAction()
{
	Attached = UGameplayStatics::SpawnEmitterAttached(Datas[0].Effect, Box, "");
	Attached->SetRelativeLocation(Datas[0].EffectTrasnform.GetLocation());
	Attached->SetRelativeScale3D(Datas[0].EffectTrasnform.GetScale3D());

	ACAttachment* attachment = Cast<ACAttachment>(Box->GetOwner());
	attachment->OnCollision();

	Angle = UKismetMathLibrary::RandomFloatInRange(0, 360);

	UKismetSystemLibrary::K2_SetTimer(this, "Hitted", HittedTime, true);
}

void ACDoAction_FireStorm::End_DoAction()
{
	Status->SetMove();
	State->SetIdleMode();

	FTimerDynamicDelegate timer;
	timer.BindUFunction(this, "Finish");
	UKismetSystemLibrary::K2_SetTimerDelegate(timer, DestroyTime, false);
}



void ACDoAction_FireStorm::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	CheckNull(OwnerCharacter);
	FVector location = OwnerCharacter->GetActorLocation();

	Angle += Speed * DeltaTime;

	if (FMath::IsNearlyEqual(Angle, 360.0f))
		Angle = 0.0f;

	FVector rotVector = FVector(Distance, 0, 0);
	FVector value = rotVector.RotateAngleAxis(Angle, FVector::UpVector);
	location += value;

	Box->SetWorldLocation(location);
}


void ACDoAction_FireStorm::OnAttachmentBeginOverlap(ACharacter* InAttacker, AActor* InAttackCauser, ACharacter* InOtherCharacter)
{
	HittedCharacters.AddUnique(InOtherCharacter);
}

void ACDoAction_FireStorm::OnAttachmentEndOverlap(ACharacter* InAttacker, AActor* InAttackCauser, ACharacter* InOtherCharacter)
{
	HittedCharacters.Remove(InOtherCharacter);
}

void ACDoAction_FireStorm::Hitted()
{
	FDamageEvent e;
	for (ACharacter* character : HittedCharacters)
	{
		bool bDead = false;
		ACEnemy* enemy = Cast<ACEnemy>(character);
		if (!!enemy)
		{
			UCStateComponent* state = CHelpers::GetComponent<UCStateComponent>(enemy);
			bDead = state->IsDeadMode();
		}

		if (!!character && bDead == false)
		{
			character->TakeDamage(Datas[0].Power, e, OwnerCharacter->GetController(), this);
			break;
		}
	}
}

void ACDoAction_FireStorm::Finish()
{
	if (!!Attached)
		Attached->DestroyComponent();
	bOnce = false;

	ACAttachment* attachment = Cast<ACAttachment>(Box->GetOwner());
	attachment->OffCollision();

	UKismetSystemLibrary::K2_ClearTimer(this, "Hitted");
}

void ACDoAction_FireStorm::Abort()
{
	Finish();
}