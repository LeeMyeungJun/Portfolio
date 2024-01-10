#include "CAutoDoor.h"
#include "Global.h"
#include "Components/BoxComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Characters/CPlayer.h"



ACAutoDoor::ACAutoDoor()
{
	PrimaryActorTick.bCanEverTick = true;

	CHelpers::CreateComponent(this, &Scene, "Scene");
	CHelpers::CreateComponent(this, &Box, "Box", Scene);
	CHelpers::CreateComponent(this, &DoorFrame, "DoorFrame", Scene);
	CHelpers::CreateComponent(this, &Door, "Door", DoorFrame);

	UStaticMesh* doorFrame;
	CHelpers::GetAsset<UStaticMesh>(&doorFrame, "StaticMesh'/Game/DoorMesh/Props/SM_DoorFrame.SM_DoorFrame'");
	DoorFrame->SetStaticMesh(doorFrame);

	UStaticMesh* door;
	CHelpers::GetAsset<UStaticMesh>(&door, "StaticMesh'/Game/DoorMesh/Props/SM_Door.SM_Door'");
	Door->SetStaticMesh(door);

	Box->InitBoxExtent(FVector(150, 50, 100));
	Box->SetRelativeLocation(FVector(0, 0, 100));
	Box->SetCollisionProfileName("Trigger");

	Door->SetRelativeLocation(FVector(0, 45, 0));
}

void ACAutoDoor::BeginPlay()
{
	Super::BeginPlay();
	
	Box->SetHiddenInGame(bHiddenInGame);

	Box->OnComponentBeginOverlap.AddDynamic(this, &ACAutoDoor::OnBeginOverlap);
	Box->OnComponentEndOverlap.AddDynamic(this, &ACAutoDoor::OnEndOverlap);

	OnAutoDoorOpen.AddUFunction(this, "Open");
	OnAutoDoorClose.AddUFunction(this, "Close");
}

void ACAutoDoor::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	FRotator rotator = Door->GetRelativeRotation();

	if (bOpen)
		Door->SetRelativeRotation(FMath::Lerp(rotator, FRotator(0, Rotation, 0), Speed * 0.01f));
	else
		Door->SetRelativeRotation(FMath::Lerp(rotator, FRotator::ZeroRotator, Speed * 0.01f));
}

void ACAutoDoor::OnBeginOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
	CheckNull(OtherActor);
	CheckNull(OtherComp);

	if (OtherActor == this) return;
	if (OtherActor->GetClass()->IsChildOf(ACPlayer::StaticClass()) == false) return;

	if (OnAutoDoorOpen.IsBound())
		OnAutoDoorOpen.Broadcast(Cast<ACPlayer>(OtherActor));
}

void ACAutoDoor::OnEndOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex)
{
	CheckNull(OtherActor);
	CheckNull(OtherComp);

	if (OtherActor == this) return;
	if (OtherActor->GetClass()->IsChildOf(ACPlayer::StaticClass()) == false) return;

	if (OnAutoDoorClose.IsBound())
		OnAutoDoorClose.Broadcast();
}

void ACAutoDoor::Open(ACPlayer* InPlayer)
{
	FVector forward = GetActorForwardVector();
	FVector playerForward = InPlayer->GetActorForwardVector();

	float direction = FMath::Sign(forward | playerForward);
	Rotation = direction * MaxDegree;

	bOpen = true;
}

void ACAutoDoor::Close()
{
	bOpen = false;
}
