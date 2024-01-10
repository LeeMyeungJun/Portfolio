#include "CCameraActor.h"
#include "Global.h"
#include "CCameraSpline.h"
#include "Components/SplineComponent.h"
#include "Camera/CameraComponent.h"

ACCameraActor::ACCameraActor()
{
	PrimaryActorTick.bCanEverTick = true;

	CHelpers::CreateComponent(this, &Camera, "Camera");

}



void ACCameraActor::BeginPlay()
{
	Super::BeginPlay();
	
	TArray<AActor*> actors;
	UGameplayStatics::GetAllActorsOfClass(GetWorld(), ACCameraSpline::StaticClass(), actors);
	for (AActor* actor : actors)
	{
		if (actor->IsA<ACCameraSpline>() && actor->GetName().Contains("BP_CCameraSpline"))
		{
			Spline = Cast<ACCameraSpline>(actor);
		}
	}

}

void ACCameraActor::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	if (Timeline.IsPlaying())
		Timeline.TickTimeline(DeltaTime);
}

void ACCameraActor::StartTimeline()
{
	CheckTrue(Timeline.IsPlaying());

	FOnTimelineFloat progress;
	progress.BindUFunction(this, "OnProgress");

	FOnTimelineEvent finish;
	finish.BindUFunction(this, "OnFinishProgress");

	Timeline = FTimeline();
	Timeline.AddInterpFloat(Spline->GetCurveFloat(), progress);
	Timeline.SetTimelineFinishedFunc(finish);
	Timeline.SetTimelineLengthMode(ETimelineLengthMode::TL_LastKeyFrame);
	Timeline.SetPlayRate(PlayRate);
	Timeline.PlayFromStart();

	APlayerController* controller = UGameplayStatics::GetPlayerController(GetWorld(), 0);
	CheckNull(controller);
	controller->SetViewTarget(this);
}

void ACCameraActor::OnProgress(float Value)
{
	USplineComponent* spline = Spline->GetSplineComponent();
	float legth = spline->GetSplineLength();

	FVector location = spline->GetLocationAtDistanceAlongSpline(Value * legth, ESplineCoordinateSpace::World);
	FRotator rotator = spline->GetRotationAtDistanceAlongSpline(Value * legth, ESplineCoordinateSpace::World);

	SetActorLocation(location);
	SetActorRotation(rotator);
}

void ACCameraActor::OnFinishProgress()
{
	Timeline.Stop();

	APlayerController* controller = UGameplayStatics::GetPlayerController(GetWorld(), 0);
	CheckNull(controller);

	ACharacter* character = UGameplayStatics::GetPlayerCharacter(GetWorld(), 0);
	controller->SetViewTarget(Cast<AActor>(character));
}