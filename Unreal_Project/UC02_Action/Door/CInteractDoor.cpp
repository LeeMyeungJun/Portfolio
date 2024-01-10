#include "CInteractDoor.h"
#include "Global.h"
#include "Components/BoxComponent.h"
#include "Components/StaticMeshComponent.h"

ACInteractDoor::ACInteractDoor()
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

void ACInteractDoor::BeginPlay()
{
	Super::BeginPlay();
	
	Box->SetHiddenInGame(bHiddenInGame);
}

void ACInteractDoor::Openning(float Delta)
{
	AddRotation = Direction* Speed* Delta;

	if (FMath::IsNearlyEqual(Door->GetRelativeRotation().Yaw, Degree, 2.0f))
	{
		bOpening = false;
		Door->SetRelativeRotation(FRotator(0, Degree, 0));
	}
	else if (bOpening)
	{
		FRotator rotation = FRotator(0, AddRotation, 0);
		Door->AddRelativeRotation(rotation);
	}
}

void ACInteractDoor::Closing(float Delta)
{
	AddRotation = (Door->GetRelativeRotation().Yaw > 0.0f ? -Speed : +Speed) * Delta;

	if (FMath::IsNearlyZero(Door->GetRelativeRotation().Yaw, 2.0f))
	{
		bClosing = false;
		Door->SetRelativeRotation(FRotator::ZeroRotator);
	}
	else if (bClosing == true)
	{
		FRotator rotation = FRotator(0, AddRotation, 0);
		Door->AddRelativeRotation(rotation);
	}
}

void ACInteractDoor::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	if (bOpening) Openning(DeltaTime);
	if (bClosing) Closing(DeltaTime);
}

void ACInteractDoor::Interact(const FVector& InForward)
{
	float dot = InForward | Box->GetForwardVector();

	Direction = FMath::Sign(dot);

	Degree = Direction* MaxDegree;

	if (bClosed)
	{
		bClosed = false;
		bClosing = false;
		bOpening = true;
	}
	else
	{
		bClosed = true;
		bClosing = true;
		bOpening = false;
	}
}

